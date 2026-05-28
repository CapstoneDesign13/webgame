using System;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Battle = 2,
    Merchant = 1,
    Pub = 8,
    Pond = 4,
    Pharmacy = 16,
    Title,
    BossBattle1,
    BossBattle2
}

public class RoomBootstrap : MonoBehaviour
{
    public BackgroundManager background;
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

    public void LoadRoomBackground(string picid)
    {
        background.SetPic(picid);
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
                CurrentStageKey = "Pub";
                break;

            case RoomType.Pond:
                LoadNonBattle(pondSet);
                break;

            case RoomType.Pharmacy:
                LoadNonBattle(pharmacySet);
                CurrentStageKey = "Pharmacy";
                break;

            case RoomType.Title:
                LoadNonBattle(titleSet);
                break;

            case RoomType.BossBattle1:
                LoadBattle(BossSet1);
                break;

            case RoomType.BossBattle2:
                LoadBattle(BossSet2);
                CurrentStageKey = "BossBattle2";
                break;
        }

        LoadRoomBackground(CurrentStageKey);
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

    public void LoadNonBattle(RawEntry[] spawns)
    {
        PlayerUnit currentPlayer = player != null ? player : MapManager.Instance.Player;

        foreach (var r in spawns)
        {
            try
            {
                ModDatabase.Instance.unitspawnDatabase.TryGetValue(r.id, out UnitSpawnSetting setting);
                spawn.SpawnNeutral(setting, r.pos, r.type);
            }
            catch (Exception e)
            {
                Debug.LogError($"Spawn failed at {r.pos}: {e}");
            }
        }

        currentPlayer.engaged = false;
    }

    RawEntry[] merchantSet = new RawEntry[2]
    {
        new RawEntry() { id = "trader_league", pos = new Vector2Int(3, 5) },
        new RawEntry() { id = "trader", pos = new Vector2Int(5, 5), type = DialogType.상인 }
    };
    RawEntry[] pubSet = new RawEntry[5]
    {
        new RawEntry() { id = "pub_sign", pos = new Vector2Int(1, 1) },
        new RawEntry() { id = "pub_drink_pot", pos = new Vector2Int(1, 4) },
        new RawEntry() { id = "pub_owner", pos = new Vector2Int(4, 1), type = DialogType.객잔 },
        new RawEntry() { id = "pub_counter", pos = new Vector2Int(1, 7) },
        new RawEntry() { id = "pub_table", pos = new Vector2Int(4, 4) },
    };
    RawEntry[] titleSet = new RawEntry[5]
    {
        new RawEntry() { id = "player_3", pos = new Vector2Int(4, 3), type = DialogType.무당파_검성 },
        new RawEntry() { id = "player_1", pos = new Vector2Int(4, 7), type = DialogType.사천당가_만천우침 },
        new RawEntry() { id = "player_2", pos = new Vector2Int(6, 5), type = DialogType.소림사_지진나한 },
        new RawEntry() { id = "player_4", pos = new Vector2Int(2, 5), type = DialogType.하오문_혼선교란자 },
        new RawEntry() { id = "pub_table", pos = new Vector2Int(4, 5) },
    };
    RawEntry[] BossSet1 = new RawEntry[5]
    {
        new RawEntry() { id = "demonic_king_phase1", pos = new Vector2Int(4, 8) },
        new RawEntry() { id = "demonic_king_phase1_aura", pos = new Vector2Int(3, 7) },
        new RawEntry() { id = "demonic_king_phase1_aura", pos = new Vector2Int(5, 7) },
        new RawEntry() { id = "demonic_king_phase1_aura", pos = new Vector2Int(3, 9) },
        new RawEntry() { id = "demonic_king_phase1_aura", pos = new Vector2Int(5, 9) },
    };
    RawEntry[] BossSet2 = new RawEntry[3]
    {
        new RawEntry() { id = "demonic_king_phase2", pos = new Vector2Int(4, 8) },
        new RawEntry() { id = "demonic_king_phase2_aura", pos = new Vector2Int(3, 9) },
        new RawEntry() { id = "demonic_king_phase2_aura", pos = new Vector2Int(5, 9) },
    };
    RawEntry[] pondSet = new RawEntry[1]
    {
        new RawEntry() { id = "pond_pond", pos = new Vector2Int(4, 5), type = DialogType.영천 }
    };
    RawEntry[] pharmacySet = new RawEntry[1]
    {
        new RawEntry() { id = "pharmacy_pharmacy", pos = new Vector2Int(4, 5), type = DialogType.약방 }
    };
}

[System.Serializable]
public class RawEntry
{
    public string id;
    public Vector2Int pos;
    public DialogType type;
}