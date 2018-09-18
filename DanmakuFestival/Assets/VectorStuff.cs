using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VectorStuff : MonoBehaviour
{
    public Vector3 AlterVector(Vector3 vector, float angleChange, float magnitudeChange) //works
    {
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angleChange), ((vector.magnitude + magnitudeChange) / vector.magnitude) * Vector3.one);
        vector = matrix.MultiplyPoint3x4(vector);
        return vector;
    }
}
