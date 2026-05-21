using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatEntry
{
    public int atk;
    public int def;

    public static StatEntry operator +(StatEntry a, StatEntry b)
    {
        return new StatEntry
        {
            atk = a.atk + b.atk,
            def = a.def + b.def
        };
    }
}

[System.Serializable]
public abstract class Combo : IHasID
{
    public string id;
    string IHasID.id => id;
    public string name;
    public string description;
    public List<string> command_chains;
    public StatEntry stat_bonuses;
}

[System.Serializable]
public class Active : Combo
{
    public List<Vector2Int> range_coordinates;
}

[System.Serializable]
public class Passive : Combo
{

}

public class ComboManager : MonoBehaviour
{
    private bool Match(List<string> input, List<string> commandChains, int startIndex = 0)
    {
        if (startIndex < 0) return false;
        if (startIndex + commandChains.Count > input.Count) return false;

        for (int i = 0; i < commandChains.Count; i++)
        {
            if (!string.Equals(
                    input[startIndex + i],
                    commandChains[i],
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    public StatEntry Pmatch(List<string> input, List<Passive> patterns)
    {
        StatEntry sum = new StatEntry();

        foreach (var pattern in patterns)
        {
            int startIndex = input.Count - pattern.command_chains.Count;

            // 길이가 부족하면 스킵
            if (startIndex < 0)
                continue;

            // input의 끝부분만 검사
            if (Match(input, pattern.command_chains, startIndex))
            {
                sum += pattern.stat_bonuses;
                Debug.Log($"{pattern.name}: 조건 만족");
            }
        }

        return sum;
    }

    public Active Lmatch(List<string> input, List<Active> patterns)
    {
        Active bestMatch = null;

        foreach (var pattern in patterns)
        {
            if (Match(input, pattern.command_chains))
            {
                if (bestMatch == null || pattern.command_chains.Count > bestMatch.command_chains.Count)
                    bestMatch = pattern;

                Debug.Log($"{pattern.name}: 조건 만족");
            }
        }

        return bestMatch;
    }
}