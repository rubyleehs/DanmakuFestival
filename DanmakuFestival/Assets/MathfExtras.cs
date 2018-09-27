using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MathfExtras : MonoBehaviour
{
    public Vector3 AlterVector(Vector3 vector, float angleChange, float magnitudeChange) //works
    {
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angleChange), ((vector.magnitude + magnitudeChange) / vector.magnitude) * Vector3.one);
        vector = matrix.MultiplyPoint3x4(vector);
        return vector;
    }

    public Vector3 GetBezierCurvePoint(List<Vector3> bezierNodes, float t)
    {
        if (bezierNodes == null || bezierNodes.Count == 0)
        {
            //Debug.Log("Cannot Form Bezier Curve With No Point, returning Vector3.Zero");
            return Vector3.zero;
        }
        if (bezierNodes.Count == 1)
        {
            //Debug.Log("Cannot Form Bezier Curve With Single Point, Returning Original Point");
            return bezierNodes[0];
        }
        if (t > 1 || t < 0)
        {
            //Debug.Log("Start/End Points of Bezier Outside Range. t value should be between 0 and 1. May have error");
        }

        List<Vector3> _finalPoints = new List<Vector3>();
        _finalPoints.AddRange(bezierNodes);
        List<Vector3> _nodes = new List<Vector3>();

        while (_finalPoints.Count > 1)
        {
            _nodes.Clear();
            _nodes.AddRange(_finalPoints);
            _finalPoints.Clear();
            for (int i = 0; i < _nodes.Count - 1; i++)
            {
                _finalPoints.Add(Vector3.Lerp(_nodes[i], _nodes[i + 1], t));
            }

        }
        return _finalPoints[0];
    }
}
