using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MathfExtras {

    public Transform lineDrawer;
    public Vector2 secondaryRads;

    public float primaryRad;

    public float primaryRotSpeed;
    public float secondaryRotSpeed;

   //private List<Vector2> drawPoints;

    private float primaryRot = 0;
    private float secondaryRot = 0;
    private Vector2 secondaryOrigin;

	void Update () {
        primaryRot += primaryRotSpeed * Time.deltaTime;
        secondaryRot += secondaryRotSpeed * Time.deltaTime;

        secondaryOrigin = AlterVector(Vector3.up * primaryRad, primaryRot, 0);
        lineDrawer.position = this.transform.position + (Vector3)secondaryOrigin + AlterVector(secondaryRads, secondaryRot, 0);
	}
}

