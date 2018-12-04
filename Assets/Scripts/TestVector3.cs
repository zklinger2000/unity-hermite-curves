using UnityEngine;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class TestVector3 : MonoBehaviour
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
        TangentOne.Translate(Mathf.Cos(_animCounter) * 0.1f, Mathf.Sin(_animCounter) * 0.05f, -Mathf.Sin(_animCounter) * 0.1f, Space.Self);
        TangentTwo.Translate(Mathf.Sin(_animCounter) * 0.1f, Mathf.Cos(_animCounter) * 0.03f, Mathf.Cos(_animCounter) * 0.1f, Space.Self);
        // Build the line segments of the curve
        for (var i = 0; i < Segments; i++)
        {
            step = i * stepLength;
            // Use the Hermite class to get the start and end points of the current segment
            var prevPosition = Hermite.GetVector3AtStep(StartPosition.position, EndPosition.position,
                (TangentOne.position - StartPosition.position) * TangentOneWeight,
                -(TangentTwo.position - EndPosition.position) * TangentTwoWeight,
                step);
            var nextPosition = Hermite.GetVector3AtStep(StartPosition.position, EndPosition.position,
                (TangentOne.position - StartPosition.position) * TangentTwoWeight,
                -(TangentTwo.position - EndPosition.position) * TangentTwoWeight,
                step + stepLength);
            // Move the segment to the starting point
            _lines[i].transform.position = prevPosition;
            // Rotate towards the endpoint
            _lines[i].transform.LookAt(nextPosition, Vector3.up);
            // Scale the cube to match length
            _lines[i].transform.localScale = new Vector3(0.2f, 0.2f, (prevPosition - nextPosition).magnitude);
        }
        // Debug Draw methods only show up when Editor is playing and paused
        Hermite.DrawVector3(StartPosition.position, EndPosition.position, TangentOne.position,
            -TangentTwo.position, Segments);
        Debug.DrawLine(StartPosition.position, TangentOne.position, Color.cyan);
        Debug.DrawLine(EndPosition.position, TangentTwo.position, Color.magenta);
    }
}
