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

    public static List<Vector3> IterateThruShapeCorners(Shape shape, Vector3 origin)
    {
        List<Vector3> points = shape.type switch
        {
            ShapeType.Square => SquareCorners(shape.a),
            ShapeType.Cross => CrossCorners(shape.a, shape.b),
            ShapeType.Diamond => DiamondCorners(shape.a),
            ShapeType.Custom => CustomCorners(shape.cells),
            _ => new List<Vector3>()
        };

        // origin 적용
        for (int i = 0; i < points.Count; i++)
        {
            points[i] += origin;
        }

        // 마지막에 시작점 다시 추가해서 닫기
        if (points.Count > 0)
        {
            points.Add(points[0]);
        }

        return points;
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

    private static List<Vector3> SquareCorners(int size)
    {
        int r = size / 2;

        return new List<Vector3>
        {
            new Vector3(-r, -r, 0),
            new Vector3( r, -r, 0),
            new Vector3( r,  r, 0),
            new Vector3(-r,  r, 0),
        };
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

    private static List<Vector3> CrossCorners(int arm, int thickness)
    {
        int t = thickness;

        return new List<Vector3>
        {
            new Vector3(-t, -arm, 0),
            new Vector3( t, -arm, 0),

            new Vector3( t, -t, 0),
            new Vector3( arm, -t, 0),

            new Vector3( arm,  t, 0),
            new Vector3( t,  t, 0),

            new Vector3( t,  arm, 0),
            new Vector3(-t,  arm, 0),

            new Vector3(-t,  t, 0),
            new Vector3(-arm,  t, 0),

            new Vector3(-arm, -t, 0),
            new Vector3(-t, -t, 0),
        };
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

    private static List<Vector3> DiamondCorners(int radius)
    {
        return new List<Vector3>
        {
            new Vector3( 0, -radius, 0),
            new Vector3( radius, 0, 0),
            new Vector3( 0, radius, 0),
            new Vector3(-radius, 0, 0),
        };
    }

    private static List<Vector3> CustomCorners(List<Vector2Int> cells)
    {
        if (cells == null || cells.Count == 0)
            return new List<Vector3>();

        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (var c in cells)
        {
            minX = Mathf.Min(minX, c.x);
            maxX = Mathf.Max(maxX, c.x);

            minY = Mathf.Min(minY, c.y);
            maxY = Mathf.Max(maxY, c.y);
        }

        return new List<Vector3>
        {
            new Vector3(minX, minY, 0),
            new Vector3(maxX, minY, 0),
            new Vector3(maxX, maxY, 0),
            new Vector3(minX, maxY, 0),
        };
    }
}