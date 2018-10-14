using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathFinder : MathfExtras {

    public RoomGen room;

    private Vector2[,] cellFCost;
    private List<Vector2Int> discoveredCells;

    public Vector2 GetAlertedMovePosition(Vector2Int _curIndex)
    {
        if (room.roomDijkstraMap == null) return Vector3.zero;
        Vector2Int _targetIndex = _curIndex;
        int count = 0;
        while (room.roomDijkstraMap[_targetIndex.x, _targetIndex.y] != 0)
        {
            count++;
            _targetIndex = DijkstraGetLowestAdjacentCell(_curIndex);
            if (count < 100 && HasDirectLineOFSight(this.transform.position, new Vector2(_targetIndex.x, _targetIndex.y) * GameManager.roomGenerationFields.roomScale + new Vector2(room.transform.position.x, room.transform.position.y), 2.2f))
            {
                _curIndex = _targetIndex;
            }
            else return new Vector2(_curIndex.x, _curIndex.y) * GameManager.roomGenerationFields.roomScale + new Vector2(room.transform.position.x, room.transform.position.y);
        }
        return PlayerManager.player.position;
        
    }

    public bool HasDirectLineOFSight(Vector2 _pos1, Vector2 _pos2, float _radius)
    {
        float _magnitude = Mathf.Sqrt(Vector2.SqrMagnitude(_pos2 - _pos1));
        if (_radius == 0) return !Physics2D.Raycast(_pos1, _pos2 - _pos1, _magnitude, LayerMask.GetMask("Wall"));
        else return !(Physics2D.Raycast(_pos1 + Vector2.up * _radius, _pos2 - _pos1, _magnitude, LayerMask.GetMask("Wall")) || Physics2D.Raycast(_pos1 - Vector2.up * _radius, _pos2 - _pos1, _magnitude, LayerMask.GetMask("Wall")) || Physics2D.Raycast(_pos1 + Vector2.left * _radius, _pos2 - _pos1, _magnitude, LayerMask.GetMask("Wall")) || Physics2D.Raycast(_pos1 + Vector2.right * _radius, _pos2 - _pos1, _magnitude, LayerMask.GetMask("Wall")));
        
    }

    public Vector2Int DijkstraGetLowestAdjacentCell(Vector2Int _coord)
    {
        int x = _coord.x;
        int y = _coord.y;
        int lowestValue = 10000;
        List<Vector2Int> _lowestAdjCells = new List<Vector2Int>();

        if (room.roomDijkstraMap[_coord.x, _coord.y + 1] < lowestValue && room.roomDijkstraMap[_coord.x, _coord.y + 1] >= 0)
        {
            lowestValue = room.roomDijkstraMap[_coord.x, _coord.y + 1];
        }
        if (room.roomDijkstraMap[_coord.x, _coord.y - 1] < lowestValue && room.roomDijkstraMap[_coord.x, _coord.y - 1] >= 0)
        {
            lowestValue = room.roomDijkstraMap[_coord.x, _coord.y - 1];
        }
        if (room.roomDijkstraMap[_coord.x + 1, _coord.y] < lowestValue && room.roomDijkstraMap[_coord.x + 1, _coord.y] >= 0)
        {
            lowestValue = room.roomDijkstraMap[_coord.x + 1, _coord.y];
        }
        if (room.roomDijkstraMap[_coord.x - 1, _coord.y] < lowestValue && room.roomDijkstraMap[_coord.x - 1, _coord.y] >= 0)
        {
            lowestValue = room.roomDijkstraMap[_coord.x - 1, _coord.y];
        }
        if (room.roomDijkstraMap[_coord.x, _coord.y + 1] == lowestValue)
        {
            _lowestAdjCells.Add(new Vector2Int(_coord.x, _coord.y + 1));
        }
        if (room.roomDijkstraMap[_coord.x, _coord.y - 1] == lowestValue)
        {
            _lowestAdjCells.Add(new Vector2Int(_coord.x, _coord.y - 1));
        }
        if (room.roomDijkstraMap[_coord.x + 1, _coord.y] == lowestValue)
        {
            _lowestAdjCells.Add(new Vector2Int(_coord.x + 1, _coord.y));
        }
        if (room.roomDijkstraMap[_coord.x - 1, _coord.y] == lowestValue)
        {
            _lowestAdjCells.Add(new Vector2Int(_coord.x - 1, _coord.y));
        }


        if (_lowestAdjCells.Count > 1 && (_coord.x + _coord.y) % 2 == 1) return _lowestAdjCells[1];
        else return _lowestAdjCells[0];
    }

    public Vector2Int ToRoomIndexLocation(Vector3 _position)
    {
        _position -= room.transform.position;

        int x = Mathf.RoundToInt(_position.x / GameManager.roomGenerationFields.roomScale);
        int y = Mathf.RoundToInt(_position.y / GameManager.roomGenerationFields.roomScale);
        return new Vector2Int(x,y);
    }

    public List<Vector2Int> AStarPathFromTo(Vector2Int _from, Vector2Int _to)
    {
        cellFCost = new Vector2[room.roomBoundarySize.x, room.roomBoundarySize.y];
        for (int y = 0; y < room.roomBoundarySize.y; y++)
        {
            for (int x = 0; x < room.roomBoundarySize.x; x++)
            {
                cellFCost[x, y] = new Vector2(10000, 0);
            }
        }
        cellFCost[_from.x, _from.y] = new Vector2((_to - _from).magnitude * GameManager.roomGenerationFields.roomScale,0);
        discoveredCells = new List<Vector2Int>();
        discoveredCells.Add(_from);

        float _lowestCost = 10000;
        bool _toFound = false;
        //int count = 0;
        while (!_toFound)
        {
            _lowestCost = GetDiscoveredCellsLowestCost();
            for (int i = 0; i < discoveredCells.Count; i++)
            {
                if (discoveredCells[i] == _to)
                {
                    _toFound = true;
                    break;
                }
                if (cellFCost[discoveredCells[i].x, discoveredCells[i].y].x == _lowestCost)
                {
                    //Debug.Log(discoveredCells[i]);
                    DiscoverAdjacentCells(new Vector2Int(discoveredCells[i].x, discoveredCells[i].y), _to);
                    discoveredCells.RemoveAt(i);
                    break;
                }
            }
        }
        //build back the path.

        List<Vector2Int> _path = new List<Vector2Int>();
        Vector2Int _checkCell = _to;
        while (_checkCell != _from)
        {
            _path.Add(_checkCell);
            switch ((int)cellFCost[_checkCell.x, _checkCell.y].y)
            {
                case 1:
                    _checkCell += Vector2Int.up;
                    break;
                case 2:
                    _checkCell += Vector2Int.right;
                    break;
                case 3:
                    _checkCell += Vector2Int.down;
                    break;
                case 4:
                    _checkCell += Vector2Int.left;
                    break;
                default:
                    Debug.Log("Error");
                    break;
            }
        }
        _path.Reverse();
        return _path;
    }

    private float GetFCostValue(int _x, int _y)
    {
        if (_x <0 ||  _y < 0 || _x >= room.roomBoundarySize.x || _y >= room.roomBoundarySize.y) return 10000;

        return cellFCost[_x, _y].x;
    }

    private void DiscoverAdjacentCells(Vector2Int _coord, Vector2Int _targetCoord)
    {
        float _thisCellFCost = cellFCost[_coord.x, _coord.y].x;
        DiscoverCell(_coord.x, _coord.y + 1, _thisCellFCost + 1, 3, _targetCoord);
        DiscoverCell(_coord.x + 1, _coord.y, _thisCellFCost + 1, 4, _targetCoord);
        DiscoverCell(_coord.x, _coord.y - 1, _thisCellFCost + 1, 1, _targetCoord);
        DiscoverCell(_coord.x - 1, _coord.y, _thisCellFCost + 1, 2, _targetCoord);
    }

    private void DiscoverCell(int _x, int _y ,float _travelCost, int _fromDir, Vector2Int _targetCoord)
    {
        if (_x < 0 || _y < 0 || _x >= room.roomBoundarySize.x || _y >= room.roomBoundarySize.y || room.roomData[_x,_y] != 0) return;
        Vector2Int _coord = new Vector2Int(_x, _y);
        float _fCost = _travelCost + (_targetCoord - _coord).magnitude;

        //Debug.Log(_x + " , " + _y + " || " + cellFCost.GetLength(0) + " , " + cellFCost.GetLength(1));
        if(cellFCost[_x, _y].x > _fCost)
        {
            cellFCost[_x, _y] = new Vector2(_fCost,_fromDir);
            discoveredCells.Add(_coord);
        }
    }

    private float GetDiscoveredCellsLowestCost()
    {
        float lowestCost = 10000;
        for (int i = 0; i < discoveredCells.Count; i++)
        {
            if (lowestCost > cellFCost[discoveredCells[i].x, discoveredCells[i].y].x) lowestCost = cellFCost[discoveredCells[i].x, discoveredCells[i].y].x;
        }
        return lowestCost;
    }
}
