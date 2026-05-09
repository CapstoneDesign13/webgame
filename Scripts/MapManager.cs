using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public int width = 9;
    public int height = 10;

    private CharacterBase[,] grid;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        grid = new CharacterBase[width, height];
    }

    public bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public bool IsOccupied(Vector2Int pos)
    {
        if (!IsInside(pos))
        {
            return false;
        }

        return grid[pos.x, pos.y] != null;
    }

    public bool CanMoveTo(Vector2Int pos)
    {
        return IsInside(pos) && !IsOccupied(pos);
    }

    public bool PlaceUnit(CharacterBase unit, Vector2Int pos)
    {
        if (unit == null)
        {
            Debug.LogWarning("PlaceUnit 실패: unit이 null입니다.");
            return false;
        }

        if (!IsInside(pos))
        {
            Debug.LogWarning("PlaceUnit 실패: 보드 밖 위치입니다. " + pos);
            return false;
        }

        if (IsOccupied(pos))
        {
            Debug.LogWarning("PlaceUnit 실패: 이미 점유된 위치입니다. " + pos);
            return false;
        }

        grid[pos.x, pos.y] = unit;
        unit.Position = pos;

        return true;
    }

    public bool MoveUnit(CharacterBase unit, Vector2Int newPos)
    {
        if (unit == null)
        {
            return false;
        }

        if (!IsInside(newPos))
        {
            return false;
        }

        if (IsOccupied(newPos))
        {
            return false;
        }

        Vector2Int oldPos = unit.Position;

        if (IsInside(oldPos) && grid[oldPos.x, oldPos.y] == unit)
        {
            grid[oldPos.x, oldPos.y] = null;
        }

        grid[newPos.x, newPos.y] = unit;
        unit.Position = newPos;

        Debug.Log(unit.name + " moved to " + newPos);
        return true;
    }

    public CharacterBase GetUnit(Vector2Int pos)
    {
        if (!IsInside(pos))
        {
            return null;
        }

        return grid[pos.x, pos.y];
    }

    public void RemoveUnit(CharacterBase unit)
    {
        if (unit == null)
        {
            return;
        }

        Vector2Int pos = unit.Position;

        if (IsInside(pos) && grid[pos.x, pos.y] == unit)
        {
            grid[pos.x, pos.y] = null;
        }

        unit.Position = new Vector2Int(-1, -1);
    }

    public void ClearUnitOccupationOnly(CharacterBase unit)
    {
        if (unit == null)
        {
            return;
        }

        Vector2Int pos = unit.Position;

        if (IsInside(pos) && grid[pos.x, pos.y] == unit)
        {
            grid[pos.x, pos.y] = null;
        }
    }

    public bool ForceSetUnitPosition(CharacterBase unit, Vector2Int targetPosition)
    {
        if (unit == null)
        {
            return false;
        }

        if (!IsInside(targetPosition))
        {
            Debug.LogWarning("ForceSetUnitPosition 실패: 보드 밖 위치입니다. " + targetPosition);
            return false;
        }

        ClearUnitOccupationOnly(unit);

        grid[targetPosition.x, targetPosition.y] = unit;
        unit.Position = targetPosition;

        Debug.Log(unit.name + " force moved to " + targetPosition);
        return true;
    }

    public bool IsInPalace(Vector2Int pos)
    {
        bool bottom = pos.x >= 3 && pos.x <= 5 && pos.y >= 0 && pos.y <= 2;
        bool top = pos.x >= 3 && pos.x <= 5 && pos.y >= 7 && pos.y <= 9;

        return bottom || top;
    }

    public bool IsInsidePalace(Vector2Int pos)
    {
        return IsInPalace(pos);
    }

    public bool IsAdjacent4(Vector2Int a, Vector2Int b)
    {
        int distance = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        return distance == 1;
    }

    public bool IsCardinalDirection(Vector2Int direction)
    {
        return Mathf.Abs(direction.x) + Mathf.Abs(direction.y) == 1;
    }

    public bool IsDiagonalDirection(Vector2Int direction)
    {
        return Mathf.Abs(direction.x) == 1 && Mathf.Abs(direction.y) == 1;
    }
}
