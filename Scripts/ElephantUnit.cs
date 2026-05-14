using UnityEngine;

// 상(象)
public class ElephantUnit : EnemyUnit
{
    private readonly Vector2Int[] elephantMoves =
    {
        new Vector2Int( 3,  2),
        new Vector2Int( 3, -2),
        new Vector2Int(-3,  2),
        new Vector2Int(-3, -2),
        new Vector2Int( 2,  3),
        new Vector2Int( 2, -3),
        new Vector2Int(-2,  3),
        new Vector2Int(-2, -3),
    };

    public override void TakeTurn(PlayerUnit player)
    {
        if (IsDead) return;

        Vector2Int playerPos = player.CurrentGridPosition;

        // 공격 체크
        foreach (var move in elephantMoves)
        {
            if (IsElephantPathBlocked(move))
                continue;

            Vector2Int attackPos = CurrentGridPosition + move;

            if (attackPos == playerPos)
            {
                TryAttackTarget(player);
                return;
            }
        }

        // 이동
        Vector2Int bestMove = Vector2Int.zero;
        int bestDistance = int.MaxValue;

        foreach (var move in elephantMoves)
        {
            if (IsElephantPathBlocked(move))
                continue;

            Vector2Int targetPos = CurrentGridPosition + move;

            if (!MapManager.Instance.CanMoveTo(targetPos))
                continue;

            int distance =
                Mathf.Abs(playerPos.x - targetPos.x) +
                Mathf.Abs(playerPos.y - targetPos.y);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestMove = move;
            }
        }

        if (bestMove != Vector2Int.zero)
        {
            TryMove(bestMove);
        }
    }

    private bool IsElephantPathBlocked(Vector2Int move)
    {
        Vector2Int firstStep;
        Vector2Int secondStep;

        // 가로 3 이동
        if (Mathf.Abs(move.x) == 3)
        {
            firstStep = CurrentGridPosition + new Vector2Int(move.x / 3, 0);
            secondStep = CurrentGridPosition + new Vector2Int(move.x / 3 * 2, move.y / 2);
        }
        // 세로 3 이동
        else
        {
            firstStep = CurrentGridPosition + new Vector2Int(0, move.y / 3);
            secondStep = CurrentGridPosition + new Vector2Int(move.x / 2, move.y / 3 * 2);
        }

        return MapManager.Instance.IsTileOccupied(firstStep)
            || MapManager.Instance.IsTileOccupied(secondStep);
    }
}