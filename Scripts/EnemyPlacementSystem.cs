using System.Collections.Generic;
using UnityEngine;

public class EnemyPlacementSystem : MonoBehaviour
{
    [Header("Difficulty")]
    [SerializeField] private int baseC = 7;
    [SerializeField] private int baseK = 15;
    [SerializeField] private int step = 1;

    [Header("Board")]
    [SerializeField] private int width = 9;
    [SerializeField] private int height = 10;

    private List<PlacementTile> allTiles = new();
    private List<PlacementTile> palaceTiles = new();
    private HashSet<Vector2Int> occupied;

    private int currentDifficulty;

    private void Awake()
    {
        GenerateBoard();
    }

    public List<PlacementResult> GenerateStage(string stage, Vector2Int playerPosition)
    {
        int target = baseC * step + baseK;
        currentDifficulty = 0;

        List<PlacementResult> results = new();
        occupied = new HashSet<Vector2Int>();
        occupied.Add(playerPosition);

        int safety = 1000;

        while (currentDifficulty < target && safety-- > 0)
        {
            var enemy = GetRandomEnemy(stage);
            var tile = GetValidTile(enemy);

            if (tile == null) continue;

            int score = CalculateScore(enemy, tile);

            if (currentDifficulty + score > target + 5)
                continue;

            occupied.Add(tile.Position);
            currentDifficulty += score;

            results.Add(new PlacementResult
            {
                Enemy = enemy,
                Tile = tile,
                Score = score
            });
        }

        return results;
    }

    private int CalculateScore(EnemyData e, PlacementTile t)
    {
        return Mathf.RoundToInt(e.PowerScore * 0.7f + t.Score * 0.3f);
    }

    private PlacementTile GetValidTile(EnemyData enemy)
    {
        var pool = enemy.MustBeInPalace ? palaceTiles : allTiles;
        var candidates = new List<PlacementTile>();

        foreach (var t in pool)
        {
            if (!occupied.Contains(t.Position))
                candidates.Add(t);
        }

        if (candidates.Count == 0) return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    private void GenerateBoard()
    {
        allTiles.Clear();
        palaceTiles.Clear();

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var tile = new PlacementTile
                {
                    Position = new Vector2Int(x, y),
                    IsPalace = IsPalace(x, y),
                    Score = Evaluate(x, y)
                };

                allTiles.Add(tile);

                if (tile.IsPalace)
                    palaceTiles.Add(tile);
            }
    }

    private int Evaluate(int x, int y)
    {
        int center = 4 - Mathf.Abs(4 - x);
        int forward = y;
        int palace = IsPalace(x, y) ? 3 : 0;

        return center + forward + palace;
    }

    private bool IsPalace(int x, int y)
    {
        return x >= 3 && x <= 5 && (y <= 2 || y >= 7);
    }

    private EnemyData GetRandomEnemy(string stage)
        => ModDatabase.Instance.enemyPool[stage][Random.Range(0, ModDatabase.Instance.enemyPool[stage].Count)];
}