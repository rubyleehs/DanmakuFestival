using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RoomGenerationFields{

    public GameObject wallsParentsGO;
    public GameObject wallsColumnGO;
    public GameObject wallsWallsGO;
    public GameObject wallsWallsCapGO;
    public GameObject floorPortGo;
    public GameObject floorLockDownButtonGO;

    public Vector2Int numberOfRooms;

    public float roomScale;

    public Vector2Int roomBoundarySize;
    public Vector2Int regionOverlap;
    public Vector2Int chunkPerRegionCount;
    public Vector2Int centerFlipRadius;
    public Vector2Int wallDigIterationCount;

    public GameObject guardGO;
    public Vector2Int guardCount;
    public Vector2Int guardPatrolPathNodeCount;

    public Material wallMat;
    public Color wallCoolColor;
    public Color wallAlertColor;

    public Color moveColor;
    public Color warningColor;
    public Color dangerColor;
    public Color infoColor;
}
