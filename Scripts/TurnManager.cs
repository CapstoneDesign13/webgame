using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public UIManager ui;
    public WindowManager window;

    public int turnCount = 0;

    private bool duelStarted = false;
    private bool isResolvingTurn = false;
    private Coroutine enemyTurnCoroutine;

    public bool CanUsePlayerInput
    {
        get
        {
            return !duelStarted && !isResolvingTurn && enemyTurnCoroutine == null;
        }
    }

    public void StartPlayerTurn()
    {
        if (duelStarted)
        {
            return;
        }

        isResolvingTurn = false;
        enemyTurnCoroutine = null;

        if (MapManager.Instance == null || MapManager.Instance.Player == null)
        {
            Debug.LogWarning("StartPlayerTurn failed: Player is not ready.");
            return;
        }

        MapManager.Instance.Player.ResetTurn();

        if (ui != null)
        {
            ui.Refresh();
        }

        if (window != null && window.input != null)
        {
            window.input.SetActive(true);
        }
    }

    public void EndPlayerTurn()
    {
        if (!CanUsePlayerInput)
        {
            Debug.Log("EndPlayerTurn ignored: turn is already resolving or duel has started.");
            return;
        }

        isResolvingTurn = true;

        if (window != null && window.input != null)
        {
            window.input.SetActive(false);
        }

        enemyTurnCoroutine = StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy Turn");

        if (MapManager.Instance == null || MapManager.Instance.Player == null)
        {
            Debug.LogWarning("EnemyTurn stopped: MapManager or Player is missing.");
            enemyTurnCoroutine = null;
            isResolvingTurn = false;
            yield break;
        }

        PlayerUnit player = MapManager.Instance.Player;

        List<EnemyUnit> enemies = MapManager.Instance.GetLivingEnemies();

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyUnit enemy = enemies[i];

            if (enemy == null || !enemy.IsAlive)
            {
                continue;
            }

            if (player == null || !player.IsAlive)
            {
                break;
            }

            enemy.TakeTurn(player);

            yield return new WaitForSeconds(0.2f);
        }

        turnCount++;

        enemyTurnCoroutine = null;
        isResolvingTurn = false;

        if (turnCount >= 5 && !duelStarted)
        {
            StartDuel();
            yield break;
        }

        StartPlayerTurn();
    }

    private void StartDuel()
    {
        if (duelStarted)
        {
            return;
        }

        duelStarted = true;
        isResolvingTurn = true;
        enemyTurnCoroutine = null;

        if (window != null)
        {
            window.OpenDuel();
        }
    }
}
