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
    private Dictionary<string, int> spawnedCounts;

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
        spawnedCounts = new Dictionary<string, int>();

        occupied.Add(playerPosition);

        if (ModDatabase.Instance == null)
        {
            Debug.LogError("ModDatabase.Instance가 없습니다.");
            return results;
        }

        if (!ModDatabase.Instance.enemyPool.TryGetValue(stage, out SpawnPool pool))
        {
            Debug.LogError("스폰풀이 없습니다: " + stage);
            return results;
        }

        // 1. Guaranteed 적을 먼저 배치한다.
        PlaceGuaranteedEnemies(pool, results);

        // 2. 남은 난이도 예산 안에서 일반 랜덤 적을 배치한다.
        int safety = 1000;

        while (currentDifficulty < target && safety-- > 0)
        {
            EnemyData enemy = GetRandomEnemy(pool);

            if (enemy == null)
            {
                break;
            }

            PlacementTile tile = GetValidTile(enemy);

            if (tile == null)
            {
                continue;
            }

            int score = CalculateScore(enemy, tile);

            if (currentDifficulty + score > target + 5)
            {
                continue;
            }

            AddPlacement(enemy, tile, score, results);
        }

        return results;
    }

    private void PlaceGuaranteedEnemies(SpawnPool pool, List<PlacementResult> results)
    {
        foreach (EnemyData enemy in pool)
        {
            if (!enemy.Guaranteed)
            {
                continue;
            }

            if (!CanSpawnMore(enemy))
            {
                continue;
            }

            PlacementTile tile = GetValidTile(enemy);

            if (tile == null)
            {
                Debug.LogWarning("Guaranteed 적을 배치할 타일이 없습니다: " + enemy.enemy_id);
                continue;
            }

            int score = CalculateScore(enemy, tile);

            // Guaranteed 적은 난이도 예산을 초과하더라도 배치한다.
            AddPlacement(enemy, tile, score, results);
        }
    }

    private void AddPlacement(
        EnemyData enemy,
        PlacementTile tile,
        int score,
        List<PlacementResult> results
    )
    {
        occupied.Add(tile.Position);
        currentDifficulty += score;

        if (!spawnedCounts.ContainsKey(enemy.enemy_id))
        {
            spawnedCounts[enemy.enemy_id] = 0;
        }

        spawnedCounts[enemy.enemy_id]++;

        results.Add(new PlacementResult
        {
            Enemy = enemy,
            Tile = tile,
            Score = score
        });
    }

    private bool CanSpawnMore(EnemyData enemy)
    {
        if (enemy == null)
        {
            return false;
        }

        int maxCount = GetMaxCount(enemy);

        if (!spawnedCounts.TryGetValue(enemy.enemy_id, out int currentCount))
        {
            currentCount = 0;
        }

        return currentCount < maxCount;
    }

    private int GetMaxCount(EnemyData enemy)
    {
        if (enemy.MaxCount <= 0)
        {
            return int.MaxValue;
        }

        return enemy.MaxCount;
    }

    private int CalculateScore(EnemyData e, PlacementTile t)
    {
        return Mathf.RoundToInt(e.PowerScore * 0.7f + t.Score * 0.3f);
    }

    private PlacementTile GetValidTile(EnemyData enemy)
    {
        List<PlacementTile> pool = enemy.MustBeInPalace ? palaceTiles : allTiles;

        List<PlacementTile> candidates = new();

        foreach (PlacementTile tile in pool)
        {
            if (!occupied.Contains(tile.Position))
            {
                candidates.Add(tile);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    private void GenerateBoard()
    {
        allTiles.Clear();
        palaceTiles.Clear();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                PlacementTile tile = new PlacementTile
                {
                    Position = new Vector2Int(x, y),
                    IsPalace = IsPalace(x, y),
                    Score = Evaluate(x, y)
                };

                allTiles.Add(tile);

                if (tile.IsPalace)
                {
                    palaceTiles.Add(tile);
                }
            }
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

    private EnemyData GetRandomEnemy(SpawnPool pool)
    {
        List<EnemyData> candidates = new();

        foreach (EnemyData enemy in pool)
        {
            if (CanSpawnMore(enemy))
            {
                candidates.Add(enemy);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }
}