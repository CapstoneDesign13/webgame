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
    영천 = 1 << 2,
    객잔 = 1 << 3,
    약방 = 1 << 4,
    무작위 = 1 << 5,

    전부 = 상인 | 전투 | 영천 | 객잔 | 약방 | 무작위
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
    public StageManager stage;

    public Option flag;

    [SerializeField] private GameObject[] choices;

    public TMP_Text title;
    public TMP_Text content;
    public TMP_Text[] other;

    private Option[] optionOrder =
    {
        Option.상인,
        Option.전투,
        Option.영천,
        Option.객잔,
        Option.약방,
        Option.무작위
    };

    void TryAddFlag(Option option, int chance)
    {
        if (UnityEngine.Random.Range(0, 20) < chance)
        {
            flag |= option;
        }
    }

    void RandomizeFlag()
    {
        flag = Option.None;

        TryAddFlag(Option.상인, 3);
        TryAddFlag(Option.전투, 7);
        TryAddFlag(Option.영천, 3);
        TryAddFlag(Option.객잔, 3);

        if (!stage.pharmacyhappened)
        {
            TryAddFlag(Option.약방, 7);
        }

        TryAddFlag(Option.무작위, 3);

        if (flag == Option.None)
        {
            flag = Option.무작위;
        }
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
            for (int i = 0; i < optionOrder.Length * 2; i++)
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