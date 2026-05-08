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
        Instance = this;
        grid = new CharacterBase[width, height];
    }

    public bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public bool IsOccupied(Vector2Int pos)
    {
        return grid[pos.x, pos.y] != null;
    }

    public void PlaceUnit(CharacterBase unit, Vector2Int pos)
    {
        if (!IsInside(pos)) return;

        grid[pos.x, pos.y] = unit;
        unit.Position = pos;
    }

    public void MoveUnit(CharacterBase unit, Vector2Int newPos)
    {
        if (!IsInside(newPos)) return;
        if (IsOccupied(newPos)) return;

        grid[unit.Position.x, unit.Position.y] = null;
        grid[newPos.x, newPos.y] = unit;
        unit.Position = newPos;

        Debug.Log(unit.name + " moved to " + newPos);
    }

    public CharacterBase GetUnit(Vector2Int pos)
    {
        if (!IsInside(pos)) return null;
        return grid[pos.x, pos.y];
    }

    public bool IsInPalace(Vector2Int pos)
    {
        // 궁: x 3~5, y 0~2 / 7~9
        bool bottom = pos.x >= 3 && pos.x <= 5 && pos.y >= 0 && pos.y <= 2;
        bool top = pos.x >= 3 && pos.x <= 5 && pos.y >= 7 && pos.y <= 9;
        return bottom || top;
    }
}