using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MathfExtras : MonoBehaviour
{
    public readonly static int[] primes = new int[10] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };

    public Vector3 AlterVector(Vector3 vector, float angleChange) //works
    {
        if (angleChange == 0 || angleChange == float.NaN) return vector;
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angleChange), Vector3.one);
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

    public int RandomValue(Vector2Int _range)
    {
        return Random.Range(_range.x, _range.y +1);
    }
    public float RandomValue(Vector2 _range)
    {
        return Random.Range(_range.x, _range.y);
    }

    public float GetLookRotAngle(Vector2 _from, Vector2 _to)
    {
        Vector2 _dir = new Vector2(_to.x - _from.x, _to.y - _from.y);
        float _lookRotAngle = Mathf.Rad2Deg * Mathf.Atan(_dir.y / _dir.x) + 90;
        if (_dir.x == 0)
        {
            if (_dir.y > 0) _lookRotAngle = 0;
            else _lookRotAngle = 180;
        }
        if (_dir.x > 0) _lookRotAngle += 180;
        return _lookRotAngle;
    }
}
