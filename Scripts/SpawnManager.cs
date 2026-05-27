using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public UIManager ui;
    public WindowManager window;
    public DialogManager dialog;
    [Header("Unit Prefabs")]
    [SerializeField] private PlayerUnit playerPrefab;
    [SerializeField] private EnemyUnit enemyPrefab;
    [SerializeField] private HorseUnit HorsePrefab;
    [SerializeField] private ChariotUnit ChariotPrefab;
    [SerializeField] private CannonUnit CannonPrefab;
    [SerializeField] private ElephantUnit ElephantPrefab;
    [SerializeField] private GuardUnit GuardPrefab;
    [SerializeField] private KingUnit KingPrefab;
    [SerializeField] private DemonicKingPhase1Unit DemonicKingPhase1Prefab;
    [SerializeField] private DemonicKingPhase2Unit DemonicKingPhase2Prefab;
    [SerializeField] private MerchantUnit MerchantPrefab;
    [SerializeField] private Transform unitRoot;

    public void SpawnPlayer(UnitSpawnSetting setting, Vector2Int startPosition)
    {
        var map = MapManager.Instance;
        map.Player = Instantiate(playerPrefab, transform);
        map.Player.SetupStats(
            setting.displayName,
            Team.Ally,
            setting.hp,
            setting.attack,
            setting.defense,
            setting.on_hit_status_id
        );
        map.Player.ui = ui;
        SpriteRenderer spr = map.Player.GetComponent<SpriteRenderer>();
        Sprite cache = ModDatabase.Instance.GetPic(map.Player.name + "N");
        spr.sprite = cache;
        map.PlaceUnit(map.Player, startPosition);
    }

    public void SpawnEnemy(UnitSpawnSetting setting, Vector2Int startPosition)
    {
        var map = MapManager.Instance;
        EnemyUnit enemy;
        switch (setting.type)
        {
            case PieceType.Horse:
                enemy = Instantiate(HorsePrefab, transform);
                break;
            case PieceType.Chariot:
                enemy = Instantiate(ChariotPrefab, transform);
                break;
            case PieceType.Cannon:
                enemy = Instantiate(CannonPrefab, transform);
                break;
            case PieceType.Elephant:
                enemy = Instantiate(ElephantPrefab, transform);
                break;
            case PieceType.Guard:
                enemy = Instantiate(GuardPrefab, transform);
                break;
            case PieceType.King:
                if (setting.id == "demonic_king_phase1")
                {
                    enemy = Instantiate(DemonicKingPhase1Prefab, transform);
                }
                else if (setting.id == "demonic_king_phase2")
                {
                    enemy = Instantiate(DemonicKingPhase2Prefab, transform);
                }
                else
                {
                    enemy = Instantiate(KingPrefab, transform);
                }
                break;
            default:
                enemy = Instantiate(enemyPrefab, transform);
                break;
        }
        enemy.SetupStats(
            setting.displayName,
            Team.Enemy,
            setting.hp,
            setting.attack,
            setting.defense,
            setting.on_hit_status_id
        );
        enemy.SetUnitId(setting.id);
        SpriteRenderer spr = enemy.GetComponent<SpriteRenderer>();
        Sprite cache = ModDatabase.Instance.GetPic(enemy.name + "S");
        spr.sprite = cache;
        map.PlaceUnit(enemy, startPosition);
    }

    private EnemyUnit CreateEnemyBySetting(UnitSpawnSetting setting)
    {
        if (setting.id == "demonic_king_phase1")
        {
            if (DemonicKingPhase1Prefab == null)
            {
                Debug.LogError("DemonicKingPhase1Prefab이 SpawnManager에 연결되어 있지 않습니다.");
                return null;
            }
            return Instantiate(DemonicKingPhase1Prefab, transform);
        }
        if (setting.id == "demonic_king_phase2")
        {
            if (DemonicKingPhase2Prefab == null)
            {
                Debug.LogError("DemonicKingPhase2Prefab이 SpawnManager에 연결되어 있지 않습니다.");
                return null;
            }
            return Instantiate(DemonicKingPhase2Prefab, transform);
        }
        switch (setting.type)
        {
            case PieceType.Horse:
                return Instantiate(HorsePrefab, transform);
            case PieceType.Chariot:
                return Instantiate(ChariotPrefab, transform);
            case PieceType.Cannon:
                return Instantiate(CannonPrefab, transform);
            case PieceType.Elephant:
                return Instantiate(ElephantPrefab, transform);
            case PieceType.Guard:
                return Instantiate(GuardPrefab, transform);
            case PieceType.King:
                return Instantiate(KingPrefab, transform);
            default:
                return Instantiate(enemyPrefab, transform);
        }
    }

    public void SpawnNeutral(UnitSpawnSetting setting, Vector2Int startPosition, DialogType type)
    {
        var map = MapManager.Instance;
        MerchantUnit merchant = Instantiate(MerchantPrefab, transform);
        merchant.SetupStats(
            setting.displayName,
            Team.Neutral,
            setting.hp,
            setting.attack,
            setting.defense,
            setting.on_hit_status_id
        );
        merchant.type = type;
        if (type != DialogType.None)
            merchant.SetDialogHandler(dialog);
        SpriteRenderer spr = merchant.GetComponent<SpriteRenderer>();
        Sprite cache = ModDatabase.Instance.GetPic(merchant.name + "S");
        spr.sprite = cache;
        map.PlaceUnit(merchant, startPosition);
    }
}