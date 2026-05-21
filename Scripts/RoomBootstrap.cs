using System;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Battle,
    Merchant,
    Pub
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

            case RoomType.Pub:
                LoadPub();
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

    public SpawnEntry[] merchantSet;

    public void LoadMerchant()
    {
        PlayerUnit currentPlayer = player != null ? player : MapManager.Instance.Player;

        /*var settings = new List<(UnitSpawnSetting, Vector2Int, DialogType?)>()
        {
            (new UnitSpawnSetting("상회 상인", Clan.비전투, PieceType.Soldier, 50, 12, 3), new Vector2Int(5, 5), DialogType.상인),
            (new UnitSpawnSetting("상회 백운상회", Clan.비전투, PieceType.Soldier, 50, 12, 3), new Vector2Int(3, 5), null),
        };*/

        foreach (var r in merchantSet)
        {
            spawn.SpawnNeutral(r.setting, r.pos, r.type);
        }

        currentPlayer.engaged = false;
    }

    public SpawnEntry[] pubSet;

    public void LoadPub()
    {
        PlayerUnit currentPlayer = player != null ? player : MapManager.Instance.Player;

        /*var settings = new List<(UnitSpawnSetting, Vector2Int, DialogType?)>()
        {
            (new UnitSpawnSetting("객잔 간판", Clan.비전투, PieceType.Soldier, 50, 12, 3), new Vector2Int(1, 1), null),
            (new UnitSpawnSetting("객잔 술 항아리", Clan.비전투, PieceType.Soldier, 50, 12, 3), new Vector2Int(1, 4), null),
            (new UnitSpawnSetting("객잔 카운터", Clan.비전투, PieceType.Soldier, 50, 12, 3), new Vector2Int(1, 7), null),
            (new UnitSpawnSetting("객잔 주인", Clan.비전투, PieceType.Soldier, 50, 12, 3), new Vector2Int(4, 1), DialogType.객잔),
            (new UnitSpawnSetting("객잔 테이블", Clan.비전투, PieceType.Soldier, 50, 12, 3), new Vector2Int(4, 4), null),
        };*/

        foreach (var r in pubSet)
        {
            try
            {
                spawn.SpawnNeutral(r.setting, r.pos, r.type);
            }
            catch (Exception e)
            {
                Debug.LogError($"Spawn failed at {r.pos}: {e}");
            }
        }

        currentPlayer.engaged = false;
    }
}