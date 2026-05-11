using UnityEngine;
using System.Collections.Generic;

public class PlayerUnit : CharacterBase
{
    public UIManager ui;
    public int ActionPoints = 3;

    public List<string> actionHistory = new List<string>();
    public List<Vector3> path = new List<Vector3>();


    public void ResetTurn()
    {
        ActionPoints = 3;
        actionHistory.Clear();
        path.Clear();
        path.Add(transform.position);
    }

    public void RegisterAction(string action)
    {
        actionHistory.Add(action);
        CheckComboSkill();
    }

    void CheckComboSkill()
    {
        if (actionHistory.Count < 2) return;

        var arr = actionHistory;

        if (arr[0] == "Move" && arr[1] == "Move")
        {
            Debug.Log("Combo: Diagonal Move Enabled (궁 내부)");
        }
        else if (arr[0] == "Move" && arr[1] == "Attack")
        {
            Debug.Log("Combo: Dash Attack!");
        }
        else if (arr[0] == "Attack" && arr[1] == "Attack")
        {
            Debug.Log("Combo: Double Strike!");
        }
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
}