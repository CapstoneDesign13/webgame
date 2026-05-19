using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlacementTile
{
    public Vector2Int Position;
    public int Score;
    public bool IsPalace;
}

public class PlacementResult
{
    public EnemyData Enemy;
    public PlacementTile Tile;
    public int Score;

    public void Deconstruct(
    out UnitSpawnSetting setting,
    out Vector2Int position)
    {
        ModDatabase.Instance.unitspawnDatabase
            .TryGetValue(Enemy.enemy_id, out setting);

        position = Tile.Position;
    }

    public static (UnitSpawnSetting, Vector2Int) Unzip(PlacementResult result)
    {
        ModDatabase.Instance.unitspawnDatabase.TryGetValue(result.Enemy.enemy_id, out UnitSpawnSetting setting);
        return (setting, result.Tile.Position);
    }
}

[JsonObject]
[System.Serializable]
public class SpawnPool : IHasID, IEnumerable<EnemyData>
{
    public string id;
    string IHasID.id => id;
    public int Count { get => list.Count; }
    public List<EnemyData> list;
    public EnemyData this[int key] => list[key];

    public IEnumerator<EnemyData> GetEnumerator()
    => list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}

[System.Serializable]
public class EnemyData
{
    public string enemy_id;
    public int PowerScore;
    public bool MustBeInPalace;

    public static bool PalaceBound(PieceType type)
    {
        return type switch
        {
            PieceType.Guard or PieceType.King => true,
            _ => false,
        };
    }
}