using UnityEngine;

public class HorseUnit : EnemyUnit
{
    private readonly Vector2Int[] horseMoves =
    {
        new Vector2Int( 2,  1),
        new Vector2Int( 2, -1),
        new Vector2Int(-2,  1),
        new Vector2Int(-2, -1),
        new Vector2Int( 1,  2),
        new Vector2Int( 1, -2),
        new Vector2Int(-1,  2),
        new Vector2Int(-1, -2),
    };

    public override void TakeTurn(PlayerUnit player)
    {
        if (IsDead) return;

        Vector2Int playerPos = player.CurrentGridPosition;

        // 1. 공격 가능한지 먼저 검사
        foreach (var move in horseMoves)
        {
            // 말 다리 막힘 체크
            if (IsHorseLegBlocked(move))
                continue;

            Vector2Int attackPos = CurrentGridPosition + move;

            if (attackPos == playerPos)
            {
                TryAttackTarget(player);
                return;
            }
        }

        // 2. 플레이어에게 가장 가까워지는 위치로 이동
        Vector2Int bestMove = Vector2Int.zero;
        int bestDistance = int.MaxValue;

        foreach (var move in horseMoves)
        {
            if (IsHorseLegBlocked(move))
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

    private bool IsHorseLegBlocked(Vector2Int move)
    {
        Vector2Int legPos;

        // 가로 2칸 이동
        if (Mathf.Abs(move.x) == 2)
        {
            legPos = CurrentGridPosition + new Vector2Int(move.x / 2, 0);
        }
        // 세로 2칸 이동
        else
        {
            legPos = CurrentGridPosition + new Vector2Int(0, move.y / 2);
        }

        return MapManager.Instance.IsTileOccupied(legPos);
    }
}