using System;
using System.Linq;
using TMPro;
using UnityEngine;

[Flags]
public enum Option
{
    None = 0,
    상인 = 1 << 0,
    전투 = 1 << 1,
    휴식 = 1 << 2,
    무작위 = 1 << 3,

    전부 = 상인 | 전투 | 휴식 | 무작위
}

public class Oracle : IHasID
{
    public string id;
    string IHasID.id => id;
    public string[] desc;
    public string[] choices;
}

public class OraclePanel : FullScreenPanel
{
    public Option flag;

    [SerializeField] private GameObject[] choices;

    public TMP_Text title;
    public TMP_Text content;
    public TMP_Text[] other;

    private Option[] optionOrder =
    {
        Option.상인,
        Option.전투,
        Option.휴식,
        Option.무작위
    };

    void RandomizeFlag()
    {
        flag = (Option)UnityEngine.Random.Range(1, 8);
    }

    public override void Refresh()
    {
        RandomizeFlag();
        var oracle_pool = ModDatabase.Instance.oracleDB.Values.ToList();
        int rand = UnityEngine.Random.Range(0, oracle_pool.Count);
        Oracle oracle = oracle_pool[rand];
        if (oracle != null)
        {
            title.text = oracle.id;
            content.text = string.Join("\n", oracle.desc);
            for (int i = 0; i < 8; i++)
            {
                other[i].text = oracle.choices[i];
            }
        }
        for (int i = 0; i < choices.Length; i++)
        {
            bool active = flag.HasFlag(optionOrder[i]);
            choices[i].SetActive(active);
        }
    }
}