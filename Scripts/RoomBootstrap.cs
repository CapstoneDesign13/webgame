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

    public RoomType CurrentType { get; private set; }
    public string CurrentStageKey { get; private set; }

    public void BootstrapLoad()
    {
        LoadRoom(new RoomDefinition
        {
            type = type,
            stageKey = "stage1"
        });
    }

    public void LoadRoom(RoomDefinition room)
    {
        CurrentType = room.type;
        CurrentStageKey = room.stageKey;

        switch (room.type)
        {
            case RoomType.Battle:
                LoadBattle(room.stageKey);
                break;

            case RoomType.Merchant:
                LoadMerchant();
                break;
        }
    }

    public void LoadBattle(string stageKey)
    {
        if (string.IsNullOrEmpty(stageKey))
        {
            stageKey = "stage1";
        }

        if (!ModDatabase.Instance.enemyPool.ContainsKey(stageKey))
        {
            Debug.LogError($"스폰풀이 없습니다: {stageKey}");
            return;
        }

        PlayerUnit currentPlayer = player != null ? player : MapManager.Instance.Player;

        var results = placement.GenerateStage(
            stageKey,
            currentPlayer.CurrentGridPosition
        );

        foreach (var r in results)
        {
            r.Deconstruct(out UnitSpawnSetting setting, out Vector2Int pos);
            spawn.SpawnEnemy(setting, pos);
        }

        currentPlayer.engaged = true;
    }

    public void LoadMerchant()
    {
        PlayerUnit currentPlayer = player != null ? player : MapManager.Instance.Player;

        var settings = new List<(UnitSpawnSetting, Vector2Int)>()
        {
            (new UnitSpawnSetting("상회 상인", Clan.비전투, PieceType.Soldier, 50, 12, 3), new Vector2Int(5, 5)),
            (new UnitSpawnSetting("상회 백운상회", Clan.비전투, PieceType.Soldier, 50, 12, 3), new Vector2Int(3, 5)),
        };

        foreach (var r in settings)
        {
            spawn.SpawnNeutral(r.Item1, r.Item2);
        }

        currentPlayer.engaged = false;
    }
}