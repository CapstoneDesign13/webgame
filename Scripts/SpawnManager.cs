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
            setting.defense
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
                enemy = Instantiate(KingPrefab, transform);
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
            setting.defense
        );
        SpriteRenderer spr = enemy.GetComponent<SpriteRenderer>();
        Sprite cache = ModDatabase.Instance.GetPic(enemy.name + "S");
        spr.sprite = cache;
        map.PlaceUnit(enemy, startPosition);
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
            setting.defense
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