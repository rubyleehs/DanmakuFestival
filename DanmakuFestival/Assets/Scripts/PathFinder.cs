using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathFinder : MathfExtras {

    public RoomGen room;
    public List<Vector2Int> dijkstraMovePath;
    //public List<Vector2> optimizedMovePath;

    public Vector2 GetTargetMovePosition(Vector2Int _curIndex)
    {
        if (room.roomDijkstraMap == null) return Vector3.zero;
        Vector2Int _targetIndex = _curIndex;
        while (room.roomDijkstraMap[_targetIndex.x,_targetIndex.y] != 0)
        {
            _targetIndex = DijkstraGetLowestAdjacentCell(_curIndex);
            if (HasDirectLineOFSight(this.transform.position, new Vector2(_targetIndex.x, _targetIndex.y) * GameManager.roomGenerationFields.roomScale + new Vector2(room.transform.position.x, room.transform.position.y)))
            {
                _curIndex = _targetIndex;
            }
            else
            {
                return new Vector2(_curIndex.x, _curIndex.y) * GameManager.roomGenerationFields.roomScale + new Vector2(room.transform.position.x, room.transform.position.y);
            }
        }
        return PlayerManager.player.position;

        /*
        optimizedMovePath = new List<Vector2>();
        optimizedMovePath.Add(this.transform.position);
        for (int i = 0; i < dijkstraMovePath.Count; i++)
        {
            optimizedMovePath.Add(new Vector2(dijkstraMovePath[i].x,dijkstraMovePath[i].y) * GameManager.roomGenerationFields.roomScale);
        }
        Vector2 _anchorPos = optimizedMovePath[0];
        for (int i = 2; i < optimizedMovePath.Count; i++)
        {
            if(HasDirectLineOFSight(_anchorPos, optimizedMovePath[i]))
            {
                optimizedMovePath.RemoveAt(i - 1);
                i--;
            }
            else
            {
                _anchorPos = optimizedMovePath[i];
            }        
    
        }
        */
    }

    private bool HasDirectLineOFSight(Vector2 _pos1, Vector2 _pos2) // NO LAYERMASK YET
    {
        float _magnitude = Mathf.Sqrt(Vector2.SqrMagnitude(_pos2 - _pos1));
        return !Physics2D.Raycast(_pos1, _pos2 - _pos1, _magnitude,LayerMask.GetMask("Wall"));
    }

    public void CreateDijkstraMovePath(Vector2Int _startLot)
    {
        dijkstraMovePath = new List<Vector2Int>();
        dijkstraMovePath.Add(_startLot);
        Vector2Int _curLot = _startLot;
        while (room.roomDijkstraMap[_curLot.x, _curLot.y] != 0)
        {
            _curLot = DijkstraGetLowestAdjacentCell(_curLot);
            dijkstraMovePath.Add(_curLot);         
        }
    }

    public Vector2Int DijkstraGetLowestAdjacentCell(Vector2Int _coord)
    {
        int x = _coord.x;
        int y = _coord.y;
        int lowestValue = 10000;
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
        List<Vector2Int> _lowestAdjCells = new List<Vector2Int>();
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
}
