using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGen : MathfExtras {

    //======== Start Values ========
    private Vector2Int roomBoundarySize;
    private Vector2Int regionSize;
    private int chunkPerRegionCount;
    private int centerFlipRadius;
    private int wallDigIterationCount;
    private Vector2Int center;

    //========= Runtime Values ========
    private int[,] regionData;
    private int[,] roomWallData;
    private int[,] roomData;
    private int[,] roomDijkstraMap;

    private List<Vector3Int> poi;
    private List<Vector2Int> dijkstraPointsToCheck;
    private List<Vector2Int> portLocationCandidates;
    private Transform[,] visualsTransforms;
    private List<Transform> wallPorts;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateNewRoom();
        }
    }

    private void GenerateNewRoom()
    {
        RandomizeStartValues();
        GenerateNewRegion();
        CombineRegionsData();
        DigWalls(wallDigIterationCount);
        BuildSurroundingWalls();
        CreateDijkstraMap(poi);
        RemoveInaccessibleAreas();
        IdentifyRoomWallsType();
        FindWallPortsCandidates();
        CreateRoomWallsVisuals();
        CreateWallPortsVisuals(3);
    }

    private void CreateRoomWallsVisuals()
    {
        ClearRoomVisuals();
        for (int y = 0; y < roomBoundarySize.y; y++)
        {
            for (int x = 0; x < roomBoundarySize.x; x++)
            {
                if(roomWallData[x,y] >= 1)
                {
                    Transform _wallParent = Instantiate(GameManager.roomGenerationFields.wallsParentsGO, new Vector3(x, y, 0) * GameManager.roomGenerationFields.roomScale, Quaternion.identity, this.transform).transform;
                    visualsTransforms[x, y] = _wallParent;
                    if (roomWallData[x, y] == 1) Instantiate(GameManager.roomGenerationFields.wallsColumnGO, _wallParent);
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Transform _wallComponent;
                            if (roomWallData[x, y] % primes[i] == 0) _wallComponent = Instantiate(GameManager.roomGenerationFields.wallsWallsGO, _wallParent).transform;
                            else _wallComponent = Instantiate(GameManager.roomGenerationFields.wallsWallsCapGO, _wallParent).transform;

                            _wallComponent.localRotation = Quaternion.Euler(Vector3.forward * i * -90);
                            _wallComponent.localScale = Vector3.one * GameManager.roomGenerationFields.roomScale;
                        }
                    }
                }
            }
        }
    }

    private void ClearRoomVisuals()
    {
        Debug.Log(visualsTransforms);
        if (visualsTransforms != null)
        {
            Debug.Log("Deleting");
            for (int y = 0; y < visualsTransforms.GetLength(1) ; y++)
            {
                for (int x = 0; x < visualsTransforms.GetLength(0) ; x++)
                {
                    if (visualsTransforms[x, y] != null) Destroy(visualsTransforms[x, y].gameObject);//
                }
            }
        }
        visualsTransforms = new Transform[roomBoundarySize.x, roomBoundarySize.y];
        wallPorts = new List<Transform>();
    }

    private void RemoveInaccessibleAreas()
    {
        for (int y = 0; y < roomBoundarySize.y; y++)
        {
            for (int x = 0; x < roomBoundarySize.x; x++)
            {
                if(roomDijkstraMap[x,y] == 10000)
                {
                    roomWallData[x, y] = -1;
                }
            }
        }
    }

    private void IdentifyRoomWallsType()//1 = square/column, 11 = wall end;
    {
        for (int y = 0; y < roomBoundarySize.y; y++)
        {
            for (int x = 0; x < roomBoundarySize.x; x++)
            {
                if (roomWallData[x, y] == 1)
                {
                    /*
                    int _wallCount = GetWallsAroundPoint(x, y);
                    switch (_wallCount)
                    {
                        case 1:
                            roomWallData[x, y] = 1;
                            break;
                        case 8:
                            roomWallData[x, y] = 11;
                            break;
                        default:
                            break;
                    }
                    */
                    if (roomWallData[x, y] == 1)
                    {
                        int _wallType = 1;
                        if (y + 1 < roomBoundarySize.y && roomWallData[x, y + 1] > 0) _wallType *= 2;
                        if (x + 1 < roomBoundarySize.x && roomWallData[x + 1, y] > 0) _wallType *= 3;
                        if (y > 0 && roomWallData[x, y - 1] > 0) _wallType *= 5;
                        if (x > 0 && roomWallData[x - 1, y] > 0) _wallType *= 7;
                        if (_wallType == 2 || _wallType == 3 || _wallType == 5 || _wallType == 7 || _wallType == 210) roomWallData[x, y] = 1;
                        else roomWallData[x, y] = _wallType;
                    }
                }
            }
        }
    }

    private void FindWallPortsCandidates()
    {
        for (int y = 1; y < roomBoundarySize.y - 1; y++)
        {
            for (int x = 1; x < roomBoundarySize.x - 1; x++)
            {
                if (roomWallData[x, y] == 0)
                {
                    int adjacentWallCount = 0;
                    if (roomWallData[x, y + 1] > 0) adjacentWallCount++;
                    if (roomWallData[x, y - 1] > 0) adjacentWallCount++;
                    if (roomWallData[x - 1, y] > 0) adjacentWallCount++;
                    if (roomWallData[x + 1, y] > 0) adjacentWallCount++;
                    if (adjacentWallCount == 3) portLocationCandidates.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    private void CreateWallPortsVisuals(int _count)//wall port id = 11;
    {
        for (int i = 0; i < _count; i++)
        {
            Vector2Int _chosenCoord;
            if (portLocationCandidates.Count != 0)
            {
                int _index = Random.Range(0, portLocationCandidates.Count);
                _chosenCoord = portLocationCandidates[_index];
                portLocationCandidates.RemoveAt(_index);
            }
            else
            {
                _chosenCoord = GetRandomEmptyLocation();
            }
            roomWallData[_chosenCoord.x, _chosenCoord.y] = 11;
            int _rot = 0;
            if (roomWallData[_chosenCoord.x +1, _chosenCoord.y] == 0) _rot = 1;
            else if (roomWallData[_chosenCoord.x, _chosenCoord.y - 1] == 0) _rot = 2;
            else if (roomWallData[_chosenCoord.x -1, _chosenCoord.y ] == 0) _rot = 3;

            wallPorts.Add(Instantiate(GameManager.roomGenerationFields.wallPortGo, new Vector2(_chosenCoord.x * GameManager.roomGenerationFields.roomScale, _chosenCoord.y * GameManager.roomGenerationFields.roomScale), Quaternion.Euler(Vector3.forward * -90 * _rot), this.transform).transform);
            visualsTransforms[_chosenCoord.x, _chosenCoord.y] = wallPorts[i];
        }
    }

    private Vector2Int GetRandomEmptyLocation()
    {
        Vector2Int _lot;
        while (true)
        {
            _lot = new Vector2Int(Random.Range(0, roomBoundarySize.x), Random.Range(0, roomBoundarySize.y));
            if (roomWallData[_lot.x, _lot.y] == 0) return _lot;
        }
    }

    private void CombineRegionsData()
    {
        roomWallData = new int[roomBoundarySize.x, roomBoundarySize.y];
        for (int y = 0; y < roomBoundarySize.y; y++)
        {
            for (int x = 0; x < roomBoundarySize.x; x++)
            {
                roomWallData[x, y] = 1;
            }
        }
        for (int y = 0; y < regionSize.y ; y++)
        {
            for (int x = 0; x < regionSize.x ; x++)
            {
                roomWallData[x, y] += regionData[x, y];
                roomWallData[roomBoundarySize.x - x - 1, y] += regionData[x, y];
                roomWallData[x, roomBoundarySize.y - y - 1] += regionData[x, y];
                roomWallData[roomBoundarySize.x - x - 1, roomBoundarySize.y - y - 1] += regionData[x, y];
            }
        }
        for (int dy = -centerFlipRadius; dy <= centerFlipRadius; dy++)
        {
            for (int dx = -centerFlipRadius; dx <= centerFlipRadius; dx++)
            {
                roomWallData[center.x + dx, center.y + dy]++;
            }
        }

        for (int y = 0; y < roomBoundarySize.y; y++)
        {
            for (int x = 0; x < roomBoundarySize.x; x++)
            {
                if (roomWallData[x, y] % 2 == 1)
                {
                    roomWallData[x, y] = 1;
                }
                else roomWallData[x, y] = 0;
            }
        }
    }

    private void DigWalls(int iterationCount)
    {
        for (int i = 0; i < iterationCount; i++)
        {
            for (int y = 1; y < roomBoundarySize.y - 1; y++)
            {
                for (int x = 1; x < roomBoundarySize.x - 1; x++)
                {
                    if (roomWallData[x, y] == 1)//is wall
                    {
                        int _wallCount = GetWallsAroundPoint(x, y);

                        if (_wallCount == 2 || _wallCount == 8 || _wallCount == 5) roomWallData[x, y]++;
                    }
                }
            }

            for (int y = 1; y < roomBoundarySize.y - 1; y++)
            {
                for (int x = 1; x < roomBoundarySize.x - 1; x++)
                {
                    if (roomWallData[x, y] != 1) roomWallData[x, y] = 0;
                }
            }
        }
    }

    private void BuildSurroundingWalls()
    {
        for (int x = 0; x < roomBoundarySize.x; x++)
        {
            roomWallData[x, 0] = 1;
            roomWallData[x, roomBoundarySize.y - 1] = 1;
        }
        for (int y = 0; y < roomBoundarySize.y; y++)
        {
            roomWallData[0, y] = 1;
            roomWallData[roomBoundarySize.x - 1, y] = 1;
        }
    }

    private int GetWallsAroundPoint(int x, int y)
    {
        int _wallCount = 0;
        for (int dy = -1; dy <= 1; dy++)//change <= to < for ruin generation
        {
            if (y + dy < 0 || y + dy >= roomBoundarySize.y) _wallCount+= 3; //Do nothing
            else
            {
                for (int dx = -1; dx <= 1; dx++)//change <= to < for ruin generation
                {
                    if (x + dx < 0 || x + dx >= roomBoundarySize.x) _wallCount += 3;//do nothing;
                    else if (roomWallData[x + dx, y + dy] >= 1) _wallCount++;
                }
            }
        }
        return _wallCount;
    }

    private void RandomizeStartValues()
    {
        roomBoundarySize = new Vector2Int(RandomValue(GameManager.roomGenerationFields.roomBoundarySize) * 2 + 1, RandomValue(GameManager.roomGenerationFields.roomBoundarySize) * 2 + 1);
        regionSize = new Vector2Int(Mathf.FloorToInt(roomBoundarySize.x * 0.5f) + RandomValue(GameManager.roomGenerationFields.regionOverlap), Mathf.FloorToInt(roomBoundarySize.y * 0.5f) + RandomValue(GameManager.roomGenerationFields.regionOverlap));
        chunkPerRegionCount = RandomValue(GameManager.roomGenerationFields.chunkPerRegionCount);
        centerFlipRadius = RandomValue(GameManager.roomGenerationFields.centerFlipRadius);
        wallDigIterationCount = RandomValue(GameManager.roomGenerationFields.wallDigIterationCount);
        portLocationCandidates = new List<Vector2Int>();

        center = new Vector2Int((roomBoundarySize.x - 1) / 2, (roomBoundarySize.y - 1) / 2);
        poi = new List<Vector3Int>();
        poi.Add(new Vector3Int(center.x, center.y ,0));
    }

    private void GenerateNewRegion()
    {
        regionData = new int[regionSize.x, regionSize.y];

        for (int y = 0; y < regionSize.y ; y++)
        {
            for (int x = 0; x < regionSize.x ; x++)
            {
                regionData[x, y] = 0;
            }
        }
        for (int n = 0; n < chunkPerRegionCount; n++)
        {
            AddRandomChunkToRegion();
        }
    }

    private void AddRandomChunkToRegion()
    {
        Vector2Int _pos1 = GetRandomPosInRegion();
        Vector2Int _pos2 = GetRandomPosInRegion();
        int _tempInt;

        if(_pos1.x > _pos2.x)
        {
            _tempInt = _pos1.x;
            _pos1.x = _pos2.x;
            _pos2.x = _tempInt;
        }
        if (_pos1.y > _pos2.y)
        {
            _tempInt = _pos1.y;
            _pos1.y = _pos2.y;
            _pos2.y = _tempInt;
        }

        for (int y = _pos1.y; y < _pos2.y; y++)
        {
            for (int x = _pos1.x; x < _pos2.x; x++)
            {
                regionData[x,y]++;
            }
        }

    }

    private Vector2Int GetRandomPosInRegion()
    {
        return new Vector2Int(Random.Range(0, regionSize.x - 1), Random.Range(0, regionSize.y - 1));
    }

    private void CreateDijkstraMap(List<Vector3Int> _poi)// Vector3Int(x,y, value)
    {
        dijkstraPointsToCheck = new List<Vector2Int>();
        roomDijkstraMap = new int[roomBoundarySize.x, roomBoundarySize.y];

        for (int y = 0; y < roomBoundarySize.y; y++)
        {
            for (int x = 0; x < roomBoundarySize.x; x++)
            {
                roomDijkstraMap[x, y] = 10000;
            }
        }
        for (int i = 0; i < _poi.Count; i++)
        {
            roomDijkstraMap[_poi[i].x, _poi[i].y] = _poi[i].z;
            dijkstraPointsToCheck.Add(new Vector2Int(_poi[i].x, _poi[i].y));
        }
        while (dijkstraPointsToCheck.Count > 0)
        {
            DijkstraFloodToAdjacentCell(dijkstraPointsToCheck[0]);
            dijkstraPointsToCheck.RemoveAt(0);
        }
    }

    private void DijkstraFloodToAdjacentCell(Vector2Int _coord)
    {
        if(roomWallData[_coord.x,_coord.y] == 1)
        {
            roomDijkstraMap[_coord.x, _coord.y] = -9999;
            return;
        }
        else if (roomWallData[_coord.x, _coord.y] == 0)
        {
            if(roomDijkstraMap[_coord.x,_coord.y +1] > roomDijkstraMap[_coord.x, _coord.y] + 1)
            {
                dijkstraPointsToCheck.Add(new Vector2Int(_coord.x, _coord.y + 1));
                roomDijkstraMap[_coord.x, _coord.y + 1] = roomDijkstraMap[_coord.x, _coord.y] + 1;
            }
            if (roomDijkstraMap[_coord.x - 1, _coord.y] > roomDijkstraMap[_coord.x, _coord.y] + 1)
            {
                dijkstraPointsToCheck.Add(new Vector2Int(_coord.x -1, _coord.y));
                roomDijkstraMap[_coord.x - 1, _coord.y] = roomDijkstraMap[_coord.x, _coord.y] + 1;
            }
            if (roomDijkstraMap[_coord.x + 1, _coord.y] > roomDijkstraMap[_coord.x, _coord.y] + 1)
            {
                dijkstraPointsToCheck.Add(new Vector2Int(_coord.x + 1, _coord.y));
                roomDijkstraMap[_coord.x + 1, _coord.y] = roomDijkstraMap[_coord.x, _coord.y] + 1;
            }
            if (roomDijkstraMap[_coord.x, _coord.y - 1] > roomDijkstraMap[_coord.x, _coord.y] + 1)
            {
                dijkstraPointsToCheck.Add(new Vector2Int(_coord.x, _coord.y - 1));
                roomDijkstraMap[_coord.x, _coord.y - 1] = roomDijkstraMap[_coord.x, _coord.y] + 1;
            }
        }        
    }
}
