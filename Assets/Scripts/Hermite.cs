using System;
using UnityEngine;

public class Hermite {

    //===================
    // GetVector2AtStep
    //-------------------
    // Returns a two dimensional vector at a point along a curve.  The 'step' is where you are along the curve, zero
    // being the starting position and one being the end point. [0...1]
    // You must also provide two tangent vectors.
    //-------------------
    //      p1:  The starting point of the curve
    //      t1:  The tangent (e.g. direction and speed) to how the curve leaves the starting point
    //      p2:  The endpoint of the curve
    //      t2:  The tangent (e.g. direction and speed) to how the curve meets the endpoint
    //    step:  A position along the curve from 0 to 1 inclusive (e.g. halfway would be 0.5f)
    //===================
    public static Vector2 GetVector2AtStep(Vector2 p1, Vector2 p2, Vector2 t1, Vector2 t2, float step)
    {
        float h1 = 2 * (float)Math.Pow(step, 3) - 3 * (float)Math.Pow(step, 2) + 1;
        float h2 = -2 * (float)Math.Pow(step, 3) + 3 * (float)Math.Pow(step, 2);
        float h3 = (float)Math.Pow(step, 3) - 2 * (float)Math.Pow(step, 2) + step;
        float h4 = (float)Math.Pow(step, 3) - (float)Math.Pow(step, 2);
        // multiply and sum all functions together to build the interpolated point along the curve.
        return (h1 * p1) + (h2 * p2) + (h3 * t1) + (h4 * t2);
    }

    //===================
    // GetVector3AtStep
    //-------------------
    // Returns a three dimensional vector at a point along a curve.  The 'step' is where you are along the curve, zero
    // being the starting position and one being the end point. [0...1]
    // You must also provide two tangent vectors.
    // This builds the 3D curve by combining two 2D Hermite curves together, one for the XY plane and one for XZ.
    //-------------------
    //      p1:  The starting point of the curve
    //      t1:  The tangent (e.g. direction and speed) to how the curve leaves the starting point
    //      p2:  The endpoint of the curve
    //      t2:  The tangent (e.g. direction and speed) to how the curve meets the endpoint
    //    step:  A position along the curve from 0 to 1 inclusive (e.g. halfway would be 0.5f)
    //===================
    public static Vector3 GetVector3AtStep(Vector3 p1, Vector3 p2, Vector3 t1, Vector3 t2, float step)
    {
        // XY Plane
        Vector2 xyPlane = GetVector2AtStep(new Vector2(p1.x, p1.y), new Vector2(p2.x, p2.y), new Vector2(t1.x, t1.y),
            new Vector2(t2.x, t2.y), step);
        // XZ Plane
        Vector2 xzPlane = GetVector2AtStep(new Vector2(p1.x, p1.z), new Vector2(p2.x, p2.z), new Vector2(t1.x, t1.z),
            new Vector2(t2.x, t2.z), step);
        // Output combined Vector3
        return new Vector3(xyPlane.x, xyPlane.y, xzPlane.y);
    }

    //==============
    // DrawVector2
    //--------------
    // Using Debug.DrawLine(), this method uses a for loop to build the segments of a Hermite curve.  It can only be
    // seen in the Editor, in the 3D scene view, when paused.  This does NOT build a visible curve in the live game.
    //==============
    public static void DrawVector2(Vector2 p1, Vector2 p2, Vector2 t1, Vector2 t2, int segments)
    {
        Vector2 prevPoint;                                        // Start point of segment
        Vector2 nextPoint;                                        // End point of segment
        Color[] colors = new Color[2] {Color.white, Color.red};   // Colors array for display purposes
        int colorCounter = 0;                                     // Counter for looping through Colors array
        float stepLength;
        float step;

        for (int i = 0; i < segments; i++)
        {
            stepLength = 1.0f / segments;
            step = i * stepLength;
            prevPoint = GetVector2AtStep(p1, p2, t1, t2, step);
            nextPoint = GetVector2AtStep(p1, p2, t1, t2, step + stepLength);
            Debug.DrawLine(new Vector3(prevPoint.x, prevPoint.y, 0), new Vector3(nextPoint.x, nextPoint.y, 0), colors[colorCounter % colors.Length]);
            colorCounter++;
        }
    }

    //==============
    // DrawVector3
    //--------------
    // Just like the three dimensional version of the "vector at step" method above, this combines two 2D Hermite curves
    // to build up the 3D curve to display.
    //==============
    public static void DrawVector3(Vector3 p1, Vector3 p2, Vector3 t1, Vector3 t2, int segments)
    {
        Vector3 prevPoint;                                        // Start point of segment
        Vector3 nextPoint;                                        // End point of segment
        Color[] colors = new Color[2] {Color.white, Color.red};   // Colors array for display purposes
        int colorCounter = 0;                                     // Counter for looping through Colors array
        float stepLength;
        float step;

        for (int i = 0; i < segments; i++)
        {
            stepLength = 1.0f / segments;
            step = i * stepLength;
            prevPoint = GetVector3AtStep(p1, p2, t1, t2, step);
            nextPoint = GetVector3AtStep(p1, p2, t1, t2, step + stepLength);
            Debug.DrawLine(prevPoint, nextPoint, colors[colorCounter % colors.Length]);
            colorCounter++;
        }
    }
}
