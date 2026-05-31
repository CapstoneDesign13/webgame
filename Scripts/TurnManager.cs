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

    public void ResetForNewRoom()
{
    if (enemyTurnCoroutine != null)
    {
        StopCoroutine(enemyTurnCoroutine);
        enemyTurnCoroutine = null;
    }

    duelStarted = false;
    isResolvingTurn = false;
    turnCount = 0;

    if (window != null)
    {
        window.OpenField();
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

        MapManager.Instance.Player.TickStatus(StatusTickTiming.TurnStart);
        if (!MapManager.Instance.Player.IsAlive)
        {
            Debug.Log("플레이어가 상태 이상으로 사망했습니다.");
            return;
        }

        MapManager.Instance.Player.ResetTurn();
        if (ui != null)
        {
            ui.Refresh();
        }

        if (window != null && window.input != null)
        {
            window.input.gameObject.SetActive(true);
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
            window.input.gameObject.SetActive(false);
        }

        MapManager.Instance.Player.AtTurnEnd();
        MapManager.Instance.Player.TickStatus(StatusTickTiming.TurnEnd);
        if (!MapManager.Instance.Player.IsAlive)
        {
            Debug.Log("플레이어가 상태 이상으로 사망했습니다.");
            isResolvingTurn = false;
            enemyTurnCoroutine = null;
            return;
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

            enemy.TickStatus(StatusTickTiming.TurnStart);
            if (enemy == null || !enemy.IsAlive)
            {
                if (RunManager.Instance != null)
                {
                    RunManager.Instance.TryClearBattle();
                }
                continue;
            }
            enemy.TakeTurn(player);
            if (enemy != null && enemy.IsAlive)
            {
                enemy.TickStatus(StatusTickTiming.TurnEnd);
            }
            if (RunManager.Instance != null && RunManager.Instance.TryClearBattle())
            {
                yield break;
            }
            yield return new WaitForSeconds(0.2f);
        }

        turnCount++;

        enemyTurnCoroutine = null;
        isResolvingTurn = false;

        if (player == null || !player.IsAlive)
        {
            window.OpenGameOver();
        }

        if (RunManager.Instance != null
            // && !RunManager.Instance.IsCurrentBossRoom()
            && turnCount >= player.duelturn
            && !duelStarted)
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

        MapManager.Instance.Player.AtDuelStart();
        if (window != null)
        {
            window.OpenDuel();
        }
    }
}
