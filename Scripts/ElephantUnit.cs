using System.Collections.Generic;
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

        // 1. 공격 체크
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

        // 2. 실제 경로 거리 기반 이동 선택
        Vector2Int bestMove = Vector2Int.zero;
        int bestPathDistance = int.MaxValue;

        foreach (var move in elephantMoves)
        {
            if (IsElephantPathBlocked(move))
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

            foreach (var move in elephantMoves)
            {
                Vector2Int next = current + move;

                if (visited.Contains(next))
                    continue;

                if (IsElephantPathBlockedFrom(current, move))
                    continue;

                if (!MapManager.Instance.CanMoveTo(next) && next != target)
                    continue;

                visited.Add(next);
                queue.Enqueue((next, dist + 1));
            }
        }

        return int.MaxValue;
    }

    private bool IsElephantPathBlocked(Vector2Int move)
    {
        return IsElephantPathBlockedFrom(CurrentGridPosition, move);
    }

    private bool IsElephantPathBlockedFrom(Vector2Int origin, Vector2Int move)
    {
        Vector2Int firstStep;
        Vector2Int secondStep;

        // 가로 3칸 이동
        if (Mathf.Abs(move.x) == 3)
        {
            firstStep =
                origin + new Vector2Int(move.x / 3, 0);

            secondStep =
                origin + new Vector2Int(move.x / 3 * 2, move.y / 2);
        }
        // 세로 3칸 이동
        else
        {
            firstStep =
                origin + new Vector2Int(0, move.y / 3);

            secondStep =
                origin + new Vector2Int(move.x / 2, move.y / 3 * 2);
        }

        return MapManager.Instance.IsTileOccupied(firstStep)
            || MapManager.Instance.IsTileOccupied(secondStep);
    }
}