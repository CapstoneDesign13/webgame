using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Soldout
{
    public static (Combo, PoolType) pair = (null, PoolType.None);
}

public enum PoolType
{
    Active,
    Passive,
    None
}

public class MerchantPanel : FullScreenPanel
{
    public List<(Combo, PoolType)> instock;
    public MerchantCardUI[] childs;

    private void Awake()
    {
        instock = new List<(Combo, PoolType)>()
        {
            Soldout.pair,
            Soldout.pair,
            Soldout.pair,
        };

        for (int i = 0; i < 3; i++)
        {
            childs[i].Setup(instock[i]);
        }
    }

    public List<(string key, PoolType type)> GetRandomKeys(
    Dictionary<string, Active> active,
    Dictionary<string, Passive> passive,
    int count)
    {
        var combined = active.Keys.Select(k => (k, PoolType.Active))
            .Concat(passive.Keys.Select(k => (k, PoolType.Passive)))
            .ToList();

        combined = combined.OrderBy(x => Random.value).ToList();

        return combined.Take(count).ToList();
    }

    public void Refill()
    {
        var modDB = ModDatabase.Instance;

        List<(string key, PoolType type)> keys =
            GetRandomKeys(modDB.activePool, modDB.passivePool, 3);

        instock = keys.Select(x =>
        {
            return x.type == PoolType.Active
                ? ((Combo)modDB.activePool[x.key], PoolType.Active)
                : ((Combo)modDB.passivePool[x.key], PoolType.Passive);
        }).ToList();

        for (int i = 0; i < 3; i++)
        {
            childs[i].Setup(instock[i]);
        }
    }

    public void Escape()
    {
        this.gameObject.SetActive(false);
    }
    
    public void NextStage()
    {
        Escape();
        
        if (RunManager.Instance != null)
        {
            RunManager.Instance.GoNextBattleRoom();
        }
        else
        {
            Debug.LogWarning("RunManager가 씬에 없습니다.");
        }
    }

    public void Purchase((Combo, PoolType)pair)
    {
        int i = instock.FindIndex(x => x == pair);
        instock[i] = Soldout.pair;
        childs[i].Setup(Soldout.pair);
    }
}