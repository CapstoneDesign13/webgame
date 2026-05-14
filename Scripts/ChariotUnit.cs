using UnityEngine;

public class ChariotUnit : EnemyUnit
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

        // 1. 공격 가능한지 검사
        foreach (var dir in directions)
        {
            for (int i = 1; i <= moveRange; i++)
            {
                Vector2Int targetPos = CurrentGridPosition + dir * i;

                // 벽 또는 유닛 만나면 중단
                if (IsBlocked(targetPos, player))
                    break;

                // 플레이어 발견 -> 공격
                if (targetPos == playerPos)
                {
                    TryAttackTarget(player);
                    return;
                }
            }
        }

        // 2. 플레이어에게 가까워지는 위치 탐색
        Vector2Int bestMove = Vector2Int.zero;
        int bestDistance = int.MaxValue;

        foreach (var dir in directions)
        {
            for (int i = 1; i <= moveRange; i++)
            {
                Vector2Int targetPos = CurrentGridPosition + dir * i;

                if (IsBlocked(targetPos, player))
                    break;

                if (!MapManager.Instance.CanMoveTo(targetPos))
                    continue;

                int distance =
                    Mathf.Abs(playerPos.x - targetPos.x) +
                    Mathf.Abs(playerPos.y - targetPos.y);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestMove = dir * i;
                }
            }
        }

        if (bestMove != Vector2Int.zero)
        {
            TryMove(bestMove);
        }
    }

    private bool IsBlocked(Vector2Int pos, PlayerUnit player)
    {
        // 맵 밖
        if (!MapManager.Instance.IsInsideBoard(pos))
            return true;

        // 플레이어 위치는 공격 가능하므로 막지 않음
        if (player.CurrentGridPosition == pos)
            return false;

        return MapManager.Instance.IsTileOccupied(pos);
    }
}