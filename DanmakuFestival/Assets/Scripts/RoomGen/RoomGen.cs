using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGen : MathfExtras {

    public Transform testGuard;
    //======== Start Values ========
    private Vector2Int roomBoundarySize;
    private Vector2Int regionSize;
    private int chunkPerRegionCount;
    private int centerFlipRadius;
    private int wallDigIterationCount;
    private Vector2Int center;

    //========= Runtime Values ========
    private int[,] regionData;
    private int[,] roomData;
    public int[,] roomDijkstraMap;

    private List<Vector3Int> poi;
    private List<Vector2Int> dijkstraPointsToCheck;
    private List<Vector2Int> poiLocationCandidates;
    private Transform[,] visualsTransforms;
    private List<Transform> floorPorts;
    private List<Transform> floorLockDownButtons;

    private float timeSinceLastDijkstraUpdate = 0;

    private void Start()
    {
        GenerateNewRoom();
        PlayerManager.player.position = new Vector3(center.x, center.y, 0) * GameManager.roomGenerationFields.roomScale;

        Vector2Int testPos = GetRandomPOICandidateLocation();
        testGuard.position = new Vector3(testPos.x, testPos.y, 0) * GameManager.roomGenerationFields.roomScale;
    }

    private void Update()
    {
        timeSinceLastDijkstraUpdate += Time.deltaTime * GameManager.timeScale;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateNewRoom();
            PlayerManager.player.position = new Vector3(center.x,center.y,0) * GameManager.roomGenerationFields.roomScale;

            Vector2Int testPos = GetRandomPOICandidateLocation();
            testGuard.position = new Vector3(testPos.x, testPos.y, 0) * GameManager.roomGenerationFields.roomScale;
        }
        if (timeSinceLastDijkstraUpdate > GameManager.dijkstraUpdateInterval)
        {
            timeSinceLastDijkstraUpdate = 0;

            Vector2Int _playerIndexLot = ToRoomIndexLocation(PlayerManager.player.position);
            poi[0] = new Vector3Int(_playerIndexLot.x, _playerIndexLot.y, 0);
            CreateDijkstraMap();
        }
    }

    private void GenerateNewRoom()
    {
        RandomizeStartValues();
        GenerateNewRegion();
        CombineRegionsData();
        DigWalls(wallDigIterationCount);
        BuildSurroundingWalls();
        CreateDijkstraMap();
        RemoveInaccessibleAreas();
        IdentifyRoomWallsType();
        FindPOICandidates();
        CreateRoomWallsVisuals();
        CreateFloorPortsVisuals(3);
        CreateFloorLockDownButtonVisuals(1);
    }

    private void CreateRoomWallsVisuals()
    {
        ClearRoomVisuals();
        for (int y = 0; y < roomBoundarySize.y; y++)
        {
            for (int x = 0; x < roomBoundarySize.x; x++)
            {
                if(roomData[x,y] >= 1)
                {
                    Transform _wallParent = Instantiate(GameManager.roomGenerationFields.wallsParentsGO, new Vector3(x, y, 0) * GameManager.roomGenerationFields.roomScale, Quaternion.identity, this.transform).transform;
                    visualsTransforms[x, y] = _wallParent;
                    if (roomData[x, y] == 1) Instantiate(GameManager.roomGenerationFields.wallsColumnGO, _wallParent);
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Transform _wallComponent;
                            if (roomData[x, y] % primes[i] == 0) _wallComponent = Instantiate(GameManager.roomGenerationFields.wallsWallsGO, _wallParent).transform;
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
        floorPorts = new List<Transform>();
        floorLockDownButtons = new List<Transform>();
    }

    private void RemoveInaccessibleAreas()
    {
        for (int y = 0; y < roomBoundarySize.y; y++)
        {
            for (int x = 0; x < roomBoundarySize.x; x++)
            {
                if(roomDijkstraMap[x,y] == 10000)
                {
                    roomData[x, y] = -1;
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
                if (roomData[x, y] == 1)
                {
                    /*
                    int _wallCount = GetWallsAroundPoint(x, y);
                    switch (_wallCount)
                    {
                        case 1:
                            roomData[x, y] = 1;
                            break;
                        case 8:
                            roomData[x, y] = 11;
                            break;
                        default:
                            break;
                    }
                    */
                    if (roomData[x, y] == 1)
                    {
                        int _wallType = 1;
                        if (y + 1 < roomBoundarySize.y && roomData[x, y + 1] > 0) _wallType *= 2;
                        if (x + 1 < roomBoundarySize.x && roomData[x + 1, y] > 0) _wallType *= 3;
                        if (y > 0 && roomData[x, y - 1] > 0) _wallType *= 5;
                        if (x > 0 && roomData[x - 1, y] > 0) _wallType *= 7;
                        if (_wallType == 2 || _wallType == 3 || _wallType == 5 || _wallType == 7 || _wallType == 210) roomData[x, y] = 1;
                        else roomData[x, y] = _wallType;
                    }
                }
            }
        }
    }

    private void FindPOICandidates()
    {
        for (int y = 1; y < roomBoundarySize.y - 1; y++)
        {
            for (int x = 1; x < roomBoundarySize.x - 1; x++)
            {
                if (roomData[x, y] == 0)
                {
                    int adjacentWallCount = 0;
                    if (roomData[x, y + 1] > 0) adjacentWallCount++;
                    if (roomData[x, y - 1] > 0) adjacentWallCount++;
                    if (roomData[x - 1, y] > 0) adjacentWallCount++;
                    if (roomData[x + 1, y] > 0) adjacentWallCount++;
                    if (adjacentWallCount == 3) poiLocationCandidates.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    private void CreateFloorPortsVisuals(int _count)//wall port id = 11;
    {
        for (int i = 0; i < _count; i++)
        {
            Vector2Int _coord = GetRandomPOICandidateLocation();
            
            roomData[_coord.x, _coord.y] = 11;
            int _rot = 0;
            if (roomData[_coord.x +1, _coord.y] == 0) _rot = 1;
            else if (roomData[_coord.x, _coord.y - 1] == 0) _rot = 2;
            else if (roomData[_coord.x -1, _coord.y ] == 0) _rot = 3;

            floorPorts.Add(Instantiate(GameManager.roomGenerationFields.floorPortGo, new Vector2(_coord.x * GameManager.roomGenerationFields.roomScale, _coord.y * GameManager.roomGenerationFields.roomScale), Quaternion.Euler(Vector3.forward * -90 * _rot), this.transform).transform);
            visualsTransforms[_coord.x, _coord.y] = floorPorts[i];
        }
    }

    private void CreateFloorLockDownButtonVisuals(int _count)//lockdown button ID = 13;
    {
        for (int i = 0; i < _count; i++)
        {
            Vector2Int _coord = GetRandomPOICandidateLocation();

            roomData[_coord.x, _coord.y] = 13;
            int _rot = 0;
            if (roomData[_coord.x + 1, _coord.y] == 0) _rot = 1;
            else if (roomData[_coord.x, _coord.y - 1] == 0) _rot = 2;
            else if (roomData[_coord.x - 1, _coord.y] == 0) _rot = 3;

            floorLockDownButtons.Add(Instantiate(GameManager.roomGenerationFields.floorLockDownButtonGO, new Vector2(_coord.x * GameManager.roomGenerationFields.roomScale, _coord.y * GameManager.roomGenerationFields.roomScale), Quaternion.Euler(Vector3.forward * -90 * _rot), this.transform).transform);
            visualsTransforms[_coord.x, _coord.y] = floorLockDownButtons[i];//
        }
    }

    private Vector2Int GetRandomPOICandidateLocation()
    {
        Vector2Int _poiCandidate;
        if (poiLocationCandidates.Count != 0)
        {
            int _index = Random.Range(0, poiLocationCandidates.Count);
            _poiCandidate = poiLocationCandidates[_index];
            poiLocationCandidates.RemoveAt(_index);
        }
        else
        {
            _poiCandidate = GetRandomEmptyLocation();
        }
        return _poiCandidate;
    }

    private Vector2Int GetRandomEmptyLocation()
    {
        Vector2Int _lot;
        while (true)
        {
            _lot = new Vector2Int(Random.Range(0, roomBoundarySize.x), Random.Range(0, roomBoundarySize.y));
            if (roomData[_lot.x, _lot.y] == 0) return _lot;
        }
    }

    private void CombineRegionsData()
    {
        roomData = new int[roomBoundarySize.x, roomBoundarySize.y];
        for (int y = 0; y < roomBoundarySize.y; y++)
        {
            for (int x = 0; x < roomBoundarySize.x; x++)
            {
                roomData[x, y] = 1;
            }
        }
        for (int y = 0; y < regionSize.y ; y++)
        {
            for (int x = 0; x < regionSize.x ; x++)
            {
                roomData[x, y] += regionData[x, y];
                roomData[roomBoundarySize.x - x - 1, y] += regionData[x, y];
                roomData[x, roomBoundarySize.y - y - 1] += regionData[x, y];
                roomData[roomBoundarySize.x - x - 1, roomBoundarySize.y - y - 1] += regionData[x, y];
            }
        }
        for (int dy = -centerFlipRadius; dy <= centerFlipRadius; dy++)
        {
            for (int dx = -centerFlipRadius; dx <= centerFlipRadius; dx++)
            {
                roomData[center.x + dx, center.y + dy]++;
            }
        }

        for (int y = 0; y < roomBoundarySize.y; y++)
        {
            for (int x = 0; x < roomBoundarySize.x; x++)
            {
                if (roomData[x, y] % 2 == 1)
                {
                    roomData[x, y] = 1;
                }
                else roomData[x, y] = 0;
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
                    if (roomData[x, y] == 1)//is wall
                    {
                        int _wallCount = GetWallsAroundPoint(x, y);

                        if (_wallCount == 2 || _wallCount == 8 || _wallCount == 5) roomData[x, y]++;
                    }
                }
            }

            for (int y = 1; y < roomBoundarySize.y - 1; y++)
            {
                for (int x = 1; x < roomBoundarySize.x - 1; x++)
                {
                    if (roomData[x, y] != 1) roomData[x, y] = 0;
                }
            }
        }
    }

    private void BuildSurroundingWalls()
    {
        for (int x = 0; x < roomBoundarySize.x; x++)
        {
            roomData[x, 0] = 1;
            roomData[x, roomBoundarySize.y - 1] = 1;
        }
        for (int y = 0; y < roomBoundarySize.y; y++)
        {
            roomData[0, y] = 1;
            roomData[roomBoundarySize.x - 1, y] = 1;
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
                    else if (roomData[x + dx, y + dy] >= 1) _wallCount++;
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
        poiLocationCandidates = new List<Vector2Int>();

        center = new Vector2Int((roomBoundarySize.x - 1) / 2, (roomBoundarySize.y - 1) / 2);
        poi = new List<Vector3Int>();
        poi.Add(new Vector3Int(center.x,center.y,0));
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

    private void CreateDijkstraMap()// Vector3Int(x,y, value)
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
        for (int i = 0; i < poi.Count; i++)
        {
            roomDijkstraMap[poi[i].x, poi[i].y] = poi[i].z;
            dijkstraPointsToCheck.Add(new Vector2Int(poi[i].x, poi[i].y));
        }
        while (dijkstraPointsToCheck.Count > 0)
        {
            DijkstraFloodToAdjacentCell(dijkstraPointsToCheck[0]);
            dijkstraPointsToCheck.RemoveAt(0);
        }
    }

    private void DijkstraFloodToAdjacentCell(Vector2Int _coord)
    {
        if(roomData[_coord.x,_coord.y] != 0)
        {
            roomDijkstraMap[_coord.x, _coord.y] = -9999;
            return;
        }
        else if (roomData[_coord.x, _coord.y] == 0)
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

    public Vector2Int ToRoomIndexLocation(Vector3 _position)
    {
        _position -= this.transform.position;

        int x = Mathf.RoundToInt(_position.x / GameManager.roomGenerationFields.roomScale);
        int y = Mathf.RoundToInt(_position.y / GameManager.roomGenerationFields.roomScale);
        return new Vector2Int(x, y);
    }
}
