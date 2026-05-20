using System;
using UnityEngine;

// 포
public class CannonUnit : EnemyUnit
{
    private readonly Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public int moveRange = 10;

    public override void TakeTurn(PlayerUnit player)
    {
        if (IsDead) return;

        Vector2Int playerPos = player.CurrentGridPosition;

        // 공격 검사
        bool attacked = SearchJumpPositions((pos, occupied) =>
        {
            if (pos == playerPos)
            {
                TryAttackTarget(player);
                return true;
            }

            // 플레이어 말고 다른 말 만나면 종료
            return occupied;
        });

        if (attacked)
            return;

        // 이동 검사
        Vector2Int bestMove = Vector2Int.zero;
        int bestDistance = int.MaxValue;

        SearchJumpPositions((pos, occupied) =>
        {
            // 점프 후 다른 말 만나면 종료
            if (occupied)
                return true;

            if (!MapManager.Instance.CanMoveTo(pos))
                return false;

            int distance =
                Mathf.Abs(playerPos.x - pos.x) +
                Mathf.Abs(playerPos.y - pos.y);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestMove = pos - CurrentGridPosition;
            }

            return false;
        });

        if (bestMove != Vector2Int.zero)
        {
            TryMove(bestMove);
        }
    }

    /// <summary>
    /// 포의 "하나를 뛰어넘은 이후" 위치 탐색
    /// return true 시 해당 방향 탐색 종료
    /// </summary>
    private bool SearchJumpPositions(Func<Vector2Int, bool, bool> onPosition)
    {
        foreach (var dir in directions)
        {
            bool jumped = false;

            for (int i = 1; i <= moveRange; i++)
            {
                Vector2Int pos = CurrentGridPosition + dir * i;

                if (!MapManager.Instance.IsInsideBoard(pos))
                    break;

                bool occupied = MapManager.Instance.IsTileOccupied(pos);

                // 아직 점프 전
                if (!jumped)
                {
                    if (occupied)
                        jumped = true;

                    continue;
                }

                // 점프 후 처리
                bool stop = onPosition(pos, occupied);

                if (stop)
                    break;
            }
        }

        return false;
    }
}