using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DropInstance
{
    public string id;
    public PoolType type;
    public int price;

    static DropInstance()
    {

    }

    public static DropInstance soldout = new DropInstance()
    {
        id = "soldout",
        type = PoolType.None,
        price = 0,
    };
}

[JsonObject]
[System.Serializable]
public class DropTable : IHasID, IEnumerable<DropInstance>
{
    public string id;
    string IHasID.id => id;
    public int Count { get => list.Count; }
    public List<DropInstance> list;
    public DropInstance this[int key] => list[key];

    public void Shuffle()
    {
        System.Random rand = new System.Random();
        // Fisher-Yates Shuffle
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public void Remove(DropInstance drop)
    {
        list.Remove(drop);
    }

    public IEnumerator<DropInstance> GetEnumerator()
    => list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}

public static class Soldout
{
    public static (Combo, PoolType) pair = (null, PoolType.None);
}

[JsonConverter(typeof(StringEnumConverter))]
public enum PoolType
{
    Active,
    Passive,
    None
}

public class MerchantPanel : FullScreenPanel
{
    public EconomyManager economy;
    public StageManager stage;

    public TMP_Text moneyTxt;
    public List<DropInstance> instock;
    public MerchantCardUI[] childs;

    private void Awake()
    {
        instock = new List<DropInstance>()
        {
            DropInstance.soldout,
            DropInstance.soldout,
            DropInstance.soldout,
        };

        for (int i = 0; i < 3; i++)
        {
            childs[i].Setup(instock[i]);
        }
    }

    public override void Refresh()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        moneyTxt.text = $"소지 은자:{economy.Money}";
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
        var list = modDB.droptable["stage1"];
        if (list.Count < 3)
            return;

        list.Shuffle();

        var result = list
        .Take(3)
        .ToList();

        instock = result;

        for (int i = 0; i < 3; i++)
            childs[i].Setup(instock[i]);
    }

    public void Escape()
    {
        this.gameObject.SetActive(false);
    }
    
    public void NextStage()
    {
        Escape();
        
        if (stage != null)
        {
            stage.OpenOracle();
        }
        else if (RunManager.Instance != null)
        {
            RunManager.Instance.GoNextBattleRoom();
        }
        else
        {
            Debug.LogWarning("RunManager가 씬에 없습니다.");
        }
    }

    public void Purchase(DropInstance drop)
    {
        if (economy.TrySpend(drop.price))
        {
            Refresh();
            int i = instock.FindIndex(x => x.id == drop.id);
            instock[i] = DropInstance.soldout;
            childs[i].Setup(instock[i]);

            var modDB = ModDatabase.Instance;
            modDB.droptable["stage1"].Remove(drop);
        }
    }
}