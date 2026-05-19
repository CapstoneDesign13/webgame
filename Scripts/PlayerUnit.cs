using UnityEngine;
using System.Collections.Generic;

public class PlayerUnit : CharacterBase
{
    public UIManager ui;
    public ComboManager combo;

    public int ActionPoints = 3;
    public bool engaged = false;

    public StatEntry statEntry = new StatEntry();

    public List<Active> actives = new List<Active>();
    public List<Passive> passives = new List<Passive>();

    public List<string> actionHistory = new List<string>();
    public List<Vector3> path = new List<Vector3>();

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

    public void RegisterAction(string action)
    {
        actionHistory.Add(action);
        statEntry += combo.Pmatch(actionHistory, passives);
    }

    public void DoMove(Vector2Int dir)
    {
        if (ActionPoints <= 0) return;

        if (TryMove(dir))
        {
            path.Add(transform.position);
            ActionPoints--;
            RegisterAction("Move");
            ui.Refresh();
        }
    }

    public void DoAttack()
    {
        if (ActionPoints <= 0) return;
        
        if (TryAttack())
        {
            ActionPoints--;
            RegisterAction("Attack");
            ui.Refresh();
            
            if (RunManager.Instance != null)
            {
                RunManager.Instance.TryClearBattle();
            }
        }
    }

    public void DoDefense()
    {
        if (ActionPoints <= 0) return;

        Debug.Log("Player Defending");
        ActionPoints--;
        RegisterAction("Defense");
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
                    if(target.Answer());
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
}