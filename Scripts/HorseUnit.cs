using System.Collections.Generic;
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

        // 1. 공격 가능 여부 검사
        foreach (var move in horseMoves)
        {
            if (IsHorseLegBlocked(move))
                continue;

            Vector2Int attackPos = CurrentGridPosition + move;

            if (attackPos == playerPos)
            {
                TryAttackTarget(player);
                return;
            }
        }

        // 2. 실제 경로 거리 기준 최적 이동 선택
        Vector2Int bestMove = Vector2Int.zero;
        int bestPathDistance = int.MaxValue;

        foreach (var move in horseMoves)
        {
            if (IsHorseLegBlocked(move))
                continue;

            Vector2Int nextPos = CurrentGridPosition + move;

            if (!MapManager.Instance.CanMoveTo(nextPos))
                continue;

            int pathDistance = GetPathDistance(nextPos, playerPos);

            if (pathDistance < bestPathDistance)
            {
                bestPathDistance = pathDistance;
                bestMove = move;
            }
        }

        if (bestMove != Vector2Int.zero)
        {
            TryMove(bestMove);
        }
    }

    private int GetPathDistance(Vector2Int start, Vector2Int target)
    {
        Queue<(Vector2Int pos, int dist)> queue = new();
        HashSet<Vector2Int> visited = new();

        queue.Enqueue((start, 0));
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();

            if (current == target)
                return dist;

            foreach (var move in horseMoves)
            {
                Vector2Int next = current + move;

                if (visited.Contains(next))
                    continue;

                // 임시로 현재 위치 기준 체크용 함수 사용
                if (IsHorseLegBlockedFrom(current, move))
                    continue;

                if (!MapManager.Instance.CanMoveTo(next) && next != target)
                    continue;

                visited.Add(next);
                queue.Enqueue((next, dist + 1));
            }
        }

        return int.MaxValue;
    }

    private bool IsHorseLegBlocked(Vector2Int move)
    {
        return IsHorseLegBlockedFrom(CurrentGridPosition, move);
    }

    private bool IsHorseLegBlockedFrom(Vector2Int origin, Vector2Int move)
    {
        Vector2Int legPos;

        if (Mathf.Abs(move.x) == 2)
        {
            legPos = origin + new Vector2Int(move.x / 2, 0);
        }
        else
        {
            legPos = origin + new Vector2Int(0, move.y / 2);
        }

        return MapManager.Instance.IsTileOccupied(legPos);
    }
}