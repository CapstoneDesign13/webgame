using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Battle,
    Merchant
}

public class RoomBootstrap : MonoBehaviour
{
    public EnemyPlacementSystem placement;
    public SpawnManager spawn;
    public PlayerUnit player;

    public RoomType type = RoomType.Battle;

    public void BootstrapLoad()
    {
        switch (type)
        {
            case RoomType.Battle:
                LoadBattle();
                break;
            case RoomType.Merchant:
                LoadMerchant();
                break;
        }
        
    }

    public void LoadBattle()
    {
        var results = placement.GenerateStage("stage1", MapManager.Instance.Player.CurrentGridPosition);

        foreach (var r in results)
        {
            r.Deconstruct(out UnitSpawnSetting setting, out Vector2Int pos);
            spawn.SpawnEnemy(setting, pos);
        }

        player.engaged = true;
    }

    public void LoadMerchant()
    {
        var settings = new List<(UnitSpawnSetting, Vector2Int)>()
        {
            (new UnitSpawnSetting("상회 상인", Clan.비전투, PieceType.Soldier, 50, 12, 3), new Vector2Int(5, 5)),
            (new UnitSpawnSetting("상회 백운상회", Clan.비전투, PieceType.Soldier, 50, 12, 3), new Vector2Int(3, 5)),
        };

        foreach (var r in settings)
        {
            spawn.SpawnNeutral(r.Item1, r.Item2);
        }

        player.engaged = false;
    }
}