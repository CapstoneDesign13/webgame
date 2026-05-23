using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Effect
{
    회복30,
    임시공격,
    임시방어,
    회복50,
    ALLorNOTHING,
    영구공격,
    영구방어,
    영구체력,
    사천당가_만천우침,
    소림사_지진나한,
    무당파_검성,
    하오문_혼선교란자,
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
    public TMP_Text content;
    public ChoiceCardUI[] cards;

    public void Setup(string title_txt, string content_txt, List<SelcEntry> entries)
    {
        title.text = title_txt;
        content.text = content_txt;
        for (int i = 0; i < 3; i++)
        {
            if (i >= entries.Count)
            {
                cards[i].gameObject.SetActive(false);
                continue;
            }
            cards[i].gameObject.SetActive(true);
            var entry = entries[i];
            var pair = new List<(Action<int>, int)>();
            int amount;
            foreach (var effect in entry.effects)
                switch (effect)
                {
                    case Effect.회복30:
                        amount = (int)(player.MaxHP * 0.3f + 0.5f);
                        pair.Add((Heal, amount));
                        break;
                    case Effect.임시공격:
                        pair.Add((Atk, 1));
                        break;
                    case Effect.임시방어:
                        pair.Add((Def, 1));
                        break;
                    case Effect.회복50:
                        amount = (int)(player.MaxHP * 0.5f + 0.5f);
                        pair.Add((Heal, amount));
                        break;
                    case Effect.ALLorNOTHING:
                        pair.Add((ALLorNOTHING, 0));
                        break;
                    case Effect.영구공격:
                        pair.Add((AtkE, 1));
                        break;
                    case Effect.영구방어:
                        pair.Add((DefE, 2));
                        break;
                    case Effect.영구체력:
                        pair.Add((MaxHPE, -2));
                        break;
                    case Effect.사천당가_만천우침:
                        pair.Add((ChangePlayer, 0));
                        break;
                    case Effect.소림사_지진나한:
                        pair.Add((ChangePlayer, 1));
                        break;
                    case Effect.무당파_검성:
                        pair.Add((ChangePlayer, 2));
                        break;
                    case Effect.하오문_혼선교란자:
                        pair.Add((ChangePlayer, 3));
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

    public void AtkE(int amount)
    {
        player.Train(new StatEntry()
        {
            atk = amount,
        });
    }

    public void DefE(int amount)
    {
        player.Train(new StatEntry()
        {
            def = amount,
        });
    }

    public void MaxHPE(int amount)
    {
        player.MaxHP += amount;
        player.HP += amount;
        if (player.HP <= 0)
        {
            player.HP = 1;
        }
    }

    public void ALLorNOTHING(int _)
    {
        int d2 = UnityEngine.Random.Range(1,2);
        if (d2 == 2)
        {
            player.HP += player.MaxHP;
        }
        else
        {
            player.HP -= 2;
        }
    }

    public PlayerSettingSO[] settings;

    public void ChangePlayer(int index)
    {
        player.SetPlayer(settings[index].data);
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