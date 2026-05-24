using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class ModDatabase : ScriptableObject
{
    public static ModDatabase Instance { get; private set; }

    public Dictionary<string, PicPath> picpathDatabase = new Dictionary<string, PicPath>();
    public Dictionary<string, UnitSpawnSetting> unitspawnDatabase = new Dictionary<string, UnitSpawnSetting>();
    public Dictionary<string, SpawnPool> enemyPool = new Dictionary<string, SpawnPool>();
    public Dictionary<string, Active> activePool = new Dictionary<string, Active>();
    public Dictionary<string, Passive> passivePool = new Dictionary<string, Passive>();
    public Dictionary<string, DropTable> droptable = new Dictionary<string, DropTable>();
    public Dictionary<string, StatusEffectData> statusEffectDatabase = new Dictionary<string, StatusEffectData>();
    public Dictionary<string, PubDialog> dialogDB = new Dictionary<string, PubDialog>();

    public void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("ModDatabase instance already exists!");
        }
    }

    public Sprite GetPic(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;
        return ModCache.Get(picpathDatabase, id);
    }
}