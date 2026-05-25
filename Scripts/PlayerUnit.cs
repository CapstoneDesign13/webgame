using UnityEngine;
using System.Collections.Generic;

public class PlayerUnit : CharacterBase
{
    public UIManager ui;
    public ComboManager combo;
    public LineAnimator line;

    public int duelturn = 10;
    public int ActionPoints = 3;
    public bool engaged = false;

    public StatEntry nextBattle = new StatEntry();
    public StatEntry statEntry = new StatEntry();

    public override int Attack { get { return Mathf.Max(0, _Attack + nextBattle.atk + statEntry.atk + StatusAttackModifier); } }
    public override int Defense { get { return Mathf.Max(0, _Defense + nextBattle.def + statEntry.def + StatusDefenseModifier); } }
    public override bool Camo { get { return nextBattle.camo || statEntry.camo;  }  }

    public List<Active> actives = new List<Active>();
    public List<Passive> passives = new List<Passive>();

    public List<string> actionHistory = new List<string>();
    public List<Vector3> path = new List<Vector3>();
    public List<Vector3> last_move = new List<Vector3>();

    public void Train(StatEntry stat)
    {
        _Attack += stat.atk;
        _Defense += stat.def;
    }

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
        if (last_move.Count == 1)
            last_move[0] = transform.position;
        else
            last_move.Add(transform.position);
        path.Add(transform.position);
        //������ ����� ����� ���� ����
        statEntry.def = 0;
        statEntry.camo = false;
    }

    public void AtTurnEnd()
    {
        //액티브 스킬
        var active = combo.Lmatch(actionHistory, actives);
        if (active != null)
        {
            var Corners = ShapeUtils.IterateThruShapeCorners(active.range_coordinates, this.transform.position);
            line.DrawPath(Corners);
            statEntry += active.stat_bonuses;
            var area = ShapeUtils.IterateThruShape(active.range_coordinates);
            foreach (var cord in area)
            {
                if(TryAttackGrid(cord, active.piercing_damage))
                {
                    var victim = MapManager.Instance.GetUnitAt(cord);
                    if (active.poison_stack > 0)
                    {
                        victim.StatusEffects.AddStatus("Poison", active.poison_stack);
                    }
                    if (active.burn_stack > 0)
                    {
                        victim.StatusEffects.AddStatus("Burn", active.burn_stack);
                    }
                    //knockback
                    if (active.knockback > 0)
                    {
                        MapManager.Instance.PushUnit(victim, this.CurrentGridPosition, active.knockback);
                    }
                    if (active.bleed_stack > 0)
                    {
                        victim.StatusEffects.AddStatus("Bleed", active.bleed_stack);
                    }
                    if (active.immobilize_stack > 0)
                    {
                        victim.StatusEffects.AddStatus("Paralysis", active.immobilize_stack);
                    }
                }
            }
        }
        //공격력 증가는 턴 종료시까지 유지
        statEntry.atk = 0;

        ui.Refresh();
    }

    public void AtDuelStart()
    {
        statEntry.atk = 0;
        statEntry.def = 0;
        statEntry.camo = false;
    }

    public void AtBattleEnd()
    {
        nextBattle.atk = 0;
        nextBattle.def = 0;
    }

    public void RegisterAction(string action)
    {
        if (!engaged)
            return;
        ActionPoints--;
        actionHistory.Add(action);
        statEntry += combo.Pmatch(actionHistory, passives);
        if (statEntry.heal > 0)
        {
            HP = Mathf.Min(HP + statEntry.heal, MaxHP);
            statEntry.heal = 0;
        }
    }

    public void DoMove(Vector2Int dir)
    {
        if (ActionPoints <= 0) return;

        if (TryMove(dir))
        {
            path.Add(transform.position);
            if (last_move.Count == 1)
                last_move[0] = transform.position;
            else
                last_move.Add(transform.position);
            RegisterAction("Move");
            ui.Refresh();
        }
    }

    public void DoFlash(Vector2Int dir)
    {
        if (ActionPoints <= 0) return;

        if (TryMove(dir * 2))
        {
            path.Add(transform.position);
            if (last_move.Count == 1)
                last_move[0] = transform.position;
            else
                last_move.Add(transform.position);
            StatusEffects.AddStatus("Weakness");
            RegisterAction("C");
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

    public void TerdinaryAction()
    {

    }

    public void SetPlayer(UnitSpawnSetting setting)
    {
        SetupStats(
            setting.displayName,
            Team.Ally,
            setting.hp,
            setting.attack,
            setting.defense,
            setting.on_hit_status_id
        );
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        Sprite cache = ModDatabase.Instance.GetPic(setting.sprite_id + "N");
        spr.sprite = cache;
        duelturn = setting.duelturn;
    }
}