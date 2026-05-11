using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public UIManager ui;
    public WindowManager window;

    public int turnCount = 0;
    private bool duelStarted = false;

    void Start()
    {
        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        MapManager.Instance.Player.ResetTurn();
        ui.Refresh();
        window.input.SetActive(true);
    }

    public void EndPlayerTurn()
    {
        window.input.SetActive(false);
        StartCoroutine(EnemyTurn());
    }

    System.Collections.IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy Turn");

        foreach (var enemy in MapManager.Instance.Enemies)
        {
            enemy.TakeTurn(MapManager.Instance.Player);
            yield return new WaitForSeconds(0.2f);
        }

        turnCount++;

        if (turnCount >= 5 && !duelStarted)
        {
            StartDuel();
            yield break;
        }

        StartPlayerTurn();
    }

    void StartDuel()
    {
        window.OpenDuel();
    }
}