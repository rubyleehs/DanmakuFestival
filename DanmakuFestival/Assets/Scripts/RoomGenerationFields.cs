using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RoomGenerationFields{

    public GameObject wallsParentsGO;
    public GameObject wallsColumnGO;
    public GameObject wallsWallsGO;
    public GameObject wallsWallsCapGO;
    public GameObject wallPortGo;

    public float roomScale;

    public Vector2Int roomBoundarySize;
    public Vector2Int regionOverlap;
    public Vector2Int chunkPerRegionCount;
    public Vector2Int centerFlipRadius;
    public Vector2Int wallDigIterationCount;
}
