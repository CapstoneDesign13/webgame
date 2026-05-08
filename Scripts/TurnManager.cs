using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public PlayerUnit player;
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
    public UIManager ui;

    public int turnCount = 0;
    private bool duelStarted = false;

    void Start()
    {
        Setup();
        StartPlayerTurn();
    }

    void Setup()
    {
        MapManager.Instance.PlaceUnit(player, new Vector2Int(4, 1));

        MapManager.Instance.PlaceUnit(enemies[0], new Vector2Int(4, 6));
        MapManager.Instance.PlaceUnit(enemies[1], new Vector2Int(3, 6));
        MapManager.Instance.PlaceUnit(enemies[2], new Vector2Int(5, 6));
    }

    void StartPlayerTurn()
    {
        player.ResetTurn();
        ui.Refresh();
    }

    public void EndPlayerTurn()
    {
        StartCoroutine(EnemyTurn());
    }

    System.Collections.IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy Turn");

        foreach (var enemy in enemies)
        {
            enemy.TakeTurn(player);
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
        duelStarted = true;
        Debug.Log("=== Duel Start ===");

        foreach (var enemy in enemies)
        {
            if (enemy.IsDead) continue;

            Debug.Log("Duel: Player vs " + enemy.name);

            while (!player.IsDead && !enemy.IsDead)
            {
                enemy.TakeDamage(player);

                if (enemy.IsDead) break;

                player.TakeDamage(enemy);
            }

            if (player.IsDead)
            {
                Debug.Log("Game Over");
                return;
            }
        }

        Debug.Log("Clear!");
    }
}