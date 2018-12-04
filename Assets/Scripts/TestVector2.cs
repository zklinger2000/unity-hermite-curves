using UnityEngine;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class TestVector2 : MonoBehaviour
{
    [SerializeField] public Transform StartPosition;        // the starting point
    [SerializeField] public Transform EndPosition;          // the endpoint
    [SerializeField] public Transform TangentOne;           // the outgoing start tangent
    [SerializeField] public float TangentOneWeight = 1.0f;  // a weight scalar for tangent two
    [SerializeField] public Transform TangentTwo;           // the incoming end tangent
    [SerializeField] public float TangentTwoWeight = 1.0f;  // a weight scalar for tangent two
    [SerializeField] public Material MaterialOne;           // material for line links
    [SerializeField] public Material MaterialTwo;           // material for line links
    [SerializeField] public int Segments;                   // segments in the curve

    private Vector2 _startPosition;                         // Vector2 version of StartPosition
    private Vector2 _endPosition;                           // Vector2 version of EndPosition
    private Vector2 _tangentOne;                            // Vector2 version of TangentOne
    private Vector2 _tangentTwo;                            // Vector2 version of TangentTwo
    private List<GameObject> _lines;                        // List of line segments for displaying curve
    private float _animCounter;                             // counter for animating tangents

    // Use this for initialization
    private void Start()
    {
        _lines = new List<GameObject>();
        for (var i = 0; i < Segments; i++)
        {
            _lines.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
            // Set the material of each segment
            var material = i % 2 == 0 ?  MaterialOne : MaterialTwo;
            _lines[i].GetComponent<Renderer>().material = material;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        float stepLength = 1.0f / Segments;
        float step;

        _animCounter += Time.deltaTime;
        // Animate the tangents to move around in cycles
        TangentOne.Translate(Mathf.Cos(_animCounter) * 0.1f, 0, 0, Space.Self);
        TangentTwo.Translate(Mathf.Sin(_animCounter) * 0.1f, 0, 0, Space.Self);
        // Turn GameObject positions into Vector2 objects
        _startPosition = new Vector2(StartPosition.position.x, StartPosition.position.y);
        _endPosition = new Vector2(EndPosition.position.x, EndPosition.position.y);
        _tangentOne = new Vector2(TangentOne.position.x, TangentOne.position.y);
        _tangentTwo = new Vector2(TangentTwo.position.x, TangentTwo.position.y);
        // Build the line segments of the curve
        for (var i = 0; i < Segments; i++)
        {
            step = i * stepLength;
            var prevPosition = Hermite.GetVector2AtStep(_startPosition, _endPosition,
                (_tangentOne - _startPosition) * TangentOneWeight,
                -(_tangentTwo - _endPosition) * TangentTwoWeight,
                step);
            var nextPosition = Hermite.GetVector2AtStep(_startPosition, _endPosition,
                (_tangentOne - _startPosition) * TangentTwoWeight,
                -(_tangentTwo - _endPosition) * TangentTwoWeight,
                step + stepLength);
            _lines[i].transform.position = new Vector3(prevPosition.x, prevPosition.y, 0);
            _lines[i].transform.LookAt(new Vector3(nextPosition.x, nextPosition.y, 0), Vector3.up);
            _lines[i].transform.localScale = new Vector3(0.2f, 0.2f, (prevPosition - nextPosition).magnitude);
        }
        // Debug Draw methods only show up when Editor is playing and paused
        Hermite.DrawVector2(_startPosition, _endPosition, (_tangentOne - _startPosition), -(_tangentTwo - _endPosition), Segments);
        Debug.DrawLine(_startPosition, _tangentOne, Color.cyan);
        Debug.DrawLine(_endPosition, _tangentTwo, Color.magenta);
    }
}
