using UnityEngine;

// ø’(Ì‚)
public class KingUnit : EnemyUnit
{
    private readonly Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public override void TakeTurn(PlayerUnit player)
    {
        if (IsDead) return;

        Vector2Int playerPos = player.CurrentGridPosition;

        // ¿Œ¡¢ ∞¯∞›
        foreach (var dir in directions)
        {
            Vector2Int attackPos = CurrentGridPosition + dir;

            if (attackPos == playerPos)
            {
                TryAttackTarget(player);
                return;
            }
        }

        // ¿Ãµø
        Vector2Int bestMove = Vector2Int.zero;
        int bestDistance = int.MaxValue;

        foreach (var dir in directions)
        {
            Vector2Int targetPos = CurrentGridPosition + dir;

            if (!MapManager.Instance.IsInsidePalace(targetPos))
                continue;
            if (!MapManager.Instance.CanMoveTo(targetPos))
                continue;

            int distance =
                Mathf.Abs(playerPos.x - targetPos.x) +
                Mathf.Abs(playerPos.y - targetPos.y);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestMove = dir;
            }
        }

        if (bestMove != Vector2Int.zero)
        {
            TryMove(bestMove);
        }
    }
}