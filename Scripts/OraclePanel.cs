using System;
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

public class OraclePanel : FullScreenPanel
{
    public Option flag;

    [SerializeField] private GameObject[] choices;

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
        for (int i = 0; i < choices.Length; i++)
        {
            bool active = flag.HasFlag(optionOrder[i]);
            choices[i].SetActive(active);
        }
    }
}