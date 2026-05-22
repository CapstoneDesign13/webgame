using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using UnityEngine;

[JsonConverter(typeof(StringEnumConverter))]
public enum ShapeType
{
    Square,
    Cross,
    Diamond,
    Custom
}

[System.Serializable]
public class Shape
{
    public ShapeType type;

    public int a;
    public int b;

    // Custom
    public List<Vector2Int> cells;
}

public static class ShapeUtils
{
    public static HashSet<Vector2Int> IterateThruShape(Shape shape)
    {
        switch (shape.type)
        {
            case ShapeType.Square:
                return Square(shape.a);

            case ShapeType.Cross:
                return Cross(shape.a, shape.b);

            case ShapeType.Diamond:
                return Diamond(shape.a);

            case ShapeType.Custom:
                return shape.cells != null
                    ? new HashSet<Vector2Int>(shape.cells)
                    : new HashSet<Vector2Int>();

            default:
                return new HashSet<Vector2Int>();
        }
    }

    // -------------------------
    // Square
    // -------------------------
    private static HashSet<Vector2Int> Square(int size)
    {
        var result = new HashSet<Vector2Int>();
        int r = size / 2;

        for (int y = -r; y <= r; y++)
        {
            for (int x = -r; x <= r; x++)
            {
                result.Add(new Vector2Int(x, y));
            }
        }

        return result;
    }

    // -------------------------
    // Cross
    // -------------------------
    private static HashSet<Vector2Int> Cross(int arm, int thickness)
    {
        var result = new HashSet<Vector2Int>();

        for (int y = -arm; y <= arm; y++)
        {
            for (int x = -arm; x <= arm; x++)
            {
                bool inVertical = Mathf.Abs(x) <= thickness && Mathf.Abs(y) <= arm;
                bool inHorizontal = Mathf.Abs(y) <= thickness && Mathf.Abs(x) <= arm;

                if (inVertical || inHorizontal)
                {
                    result.Add(new Vector2Int(x, y));
                }
            }
        }

        return result;
    }

    // -------------------------
    // Diamond
    // -------------------------
    private static HashSet<Vector2Int> Diamond(int radius)
    {
        var result = new HashSet<Vector2Int>();

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) <= radius)
                {
                    result.Add(new Vector2Int(x, y));
                }
            }
        }

        return result;
    }
}