using System;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Battle,
    Merchant,
    Pub,
    Pond,
    Parmacy,
    Title,
    BossBattle1,
    BossBattle2
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
                LoadNonBattle(merchantSet);
                break;

            case RoomType.Pub:
                LoadNonBattle(pubSet);
                break;

            case RoomType.Pond:
                LoadNonBattle(pondSet);
                break;

            case RoomType.Parmacy:
                LoadNonBattle(parmacySet);
                break;

            case RoomType.Title:
                LoadNonBattle(titleSet);
                break;

            case RoomType.BossBattle1:
                LoadBattle(BossSet1);
                break;

            case RoomType.BossBattle2:
                LoadBattle(BossSet2);
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

    public void LoadBattle(RawEntry[] spawns)
    {
        PlayerUnit currentPlayer = player != null ? player : MapManager.Instance.Player;

        foreach (var r in spawns)
        {
            try
            {
                ModDatabase.Instance.unitspawnDatabase.TryGetValue(r.id, out UnitSpawnSetting setting);
                spawn.SpawnEnemy(setting, r.pos);
            }
            catch (Exception e)
            {
                Debug.LogError($"Spawn failed at {r.pos}: {e}");
            }
        }

        currentPlayer.engaged = true;
    }

    public void LoadBattle(SpawnEntry[] spawns)
    {
        PlayerUnit currentPlayer = player != null ? player : MapManager.Instance.Player;

        foreach (var r in spawns)
        {
            try
            {
                spawn.SpawnEnemy(r.setting.data, r.pos);
            }
            catch (Exception e)
            {
                Debug.LogError($"Spawn failed at {r.pos}: {e}");
            }
        }

        currentPlayer.engaged = true;
    }

    public void LoadNonBattle(SpawnEntry[] spawns)
    {
        PlayerUnit currentPlayer = player != null ? player : MapManager.Instance.Player;

        foreach (var r in spawns)
        {
            try
            {
                spawn.SpawnNeutral(r.setting.data, r.pos, r.type);
            }
            catch (Exception e)
            {
                Debug.LogError($"Spawn failed at {r.pos}: {e}");
            }
        }

        currentPlayer.engaged = false;
    }

    public SpawnEntry[] merchantSet;
    public SpawnEntry[] pubSet;
    public SpawnEntry[] pondSet;
    public SpawnEntry[] parmacySet;
    public SpawnEntry[] titleSet;
    RawEntry[] BossSet1 = new RawEntry[3]
    {
        new RawEntry() { id = "demonic_king_phase1", pos = new Vector2Int(4, 8) },
        new RawEntry() { id = "demonic_king_phase1_aura", pos = new Vector2Int(3, 9) },
        new RawEntry() { id = "demonic_king_phase1_aura", pos = new Vector2Int(5, 9) },
    };
    RawEntry[] BossSet2 = new RawEntry[3]
    {
        new RawEntry() { id = "demonic_king_phase2", pos = new Vector2Int(4, 8) },
        new RawEntry() { id = "demonic_king_phase2_aura", pos = new Vector2Int(3, 9) },
        new RawEntry() { id = "demonic_king_phase2_aura", pos = new Vector2Int(5, 9) },
    };
}

[System.Serializable]
public class RawEntry
{
    public string id;
    public Vector2Int pos;
    public DialogType type;
}