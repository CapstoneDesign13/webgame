using UnityEngine;
using System.Collections.Generic;

public class PlayerUnit : CharacterBase
{
    public UIManager ui;
    public ComboManager combo;

    public int duelturn = 5;
    public int ActionPoints = 3;
    public bool engaged = false;

    public StatEntry statEntry = new StatEntry();

    public override int Attack { get { return _Attack + statEntry.atk; } }
    public override int Defense { get { return _Defense + statEntry.def; } }

    public List<Active> actives = new List<Active>();
    public List<Passive> passives = new List<Passive>();

    public List<string> actionHistory = new List<string>();
    public List<Vector3> path = new List<Vector3>();

    public void LearnA(Active active)
    {
        actives.Add(active);
    }

    public void LearnP(Passive passive)
    {
        passives.Add(passive);
    }

    public void Learn((Combo, PoolType) pair)
    {
        switch (pair.Item2)
        {
            case PoolType.Active:
                actives.Add((Active)pair.Item1);
                break;
            case PoolType.Passive:
                passives.Add((Passive)pair.Item1);
                break;
        }
    }

    public void ResetTurn()
    {
        ActionPoints = 3;
        actionHistory.Clear();
        path.Clear();
        path.Add(transform.position);
        //������ ����� ����� ���� ����
        statEntry.def = 0;
    }

    public void AtTurnEnd()
    {
        var active = combo.Lmatch(actionHistory, actives);
        if (active != null)
        {
            statEntry += active.stat_bonuses;
            foreach (var cord in active.range_coordinates)
            {
                TryAttackGrid(cord);
            }
        }
        //공격력 증가는 턴 종료시까지 유지
        statEntry.atk = 0;

        ui.Refresh();
    }

    public void RegisterAction(string action)
    {
        if (!engaged)
            return;
        ActionPoints--;
        actionHistory.Add(action);
        statEntry += combo.Pmatch(actionHistory, passives);
    }

    public void DoMove(Vector2Int dir)
    {
        if (ActionPoints <= 0) return;

        if (TryMove(dir))
        {
            path.Add(transform.position);
            RegisterAction("Move");
            ui.Refresh();
        }
    }

    public void DoAttack()
    {
        if (ActionPoints <= 0) return;

        RegisterAction("Z");

        TryAttack();

        ui.Refresh();
            
        if (RunManager.Instance != null)
        {
            RunManager.Instance.TryClearBattle();
        }
    }

    public void DoDefense()
    {
        if (ActionPoints <= 0) return;

        statEntry += new StatEntry()
        {
            def = _Defense,
        };
        RegisterAction("X");
        ui.Refresh();
    }

    public bool TryTalk()
    {
        Vector2Int targetPos;
        for (int x = -1; x <= 1; x += 1)
        {
            for (int y = -1; y <= 1; y += 1)
            {
                targetPos = CurrentGridPosition + new Vector2Int(x, y);
                var target = MapManager.Instance.GetUnitAt(targetPos);

                if (target != null && target.team != this.team)
                {
                    if(target.Answer())
                        return true;
                }
            }
        }
        return false;
    }

    public void PrimaryAction()
    {
        if (engaged)
            DoAttack();
        else
            TryTalk();
    }

    public void SecondaryAction()
    {
        if (engaged)
            DoDefense();
    }

    public void SetPlayer(UnitSpawnSetting setting)
    {
        SetupStats(
            setting.displayName,
            Team.Ally,
            setting.hp,
            setting.attack,
            setting.defense
        );
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        Sprite cache = ModDatabase.Instance.GetPic(setting.sprite_id + "N");
        spr.sprite = cache;
    }
}