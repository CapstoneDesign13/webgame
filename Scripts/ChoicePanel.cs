using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Effect
{
    회복,
    임시공격,
    임시방어,
}

[System.Serializable]
public class SelcEntry
{
    public string content;
    public string desc;
    public Effect[] effects;
}

public class ChoicePanel : FullScreenPanel
{
    public PlayerUnit player;
    public TMP_Text title;
    public ChoiceCardUI[] cards;
    public List<SelcEntry> choices;

    public void Setup(string title_txt, List<SelcEntry> entries)
    {
        title.text = title_txt;
        choices = entries;
        for (int i = 0; i < 3; i++)
        {
            var entry = entries[i];
            var pair = new List<(Action<int>, int)>();
            foreach (var effect in entry.effects)
                switch (effect)
                {
                    case Effect.회복:
                        int amount = (int)(player.MaxHP * 0.3f + 0.5f);
                        pair.Add((Heal, amount));
                        break;
                    case Effect.임시공격:
                        pair.Add((Atk, 1));
                        break;
                    case Effect.임시방어:
                        pair.Add((Def, 1));
                        break;
                }
            cards[i].parent = this;
            cards[i].Setup(entry.content, entry.desc, pair);
        }
    }

    public void Heal(int amount)
    {
        player.HP += amount;
        if (player.HP > player.MaxHP)
            player.HP = player.MaxHP;
    }

    public void Atk(int amount)
    {
        player.nextBattle.atk += amount;
    }

    public void Def(int amount)
    {
        player.nextBattle.def += amount;
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
}