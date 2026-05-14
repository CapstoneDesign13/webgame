using UnityEngine;

// 포(?)
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
        foreach (var dir in directions)
        {
            bool jumped = false;

            for (int i = 1; i <= moveRange; i++)
            {
                Vector2Int pos = CurrentGridPosition + dir * i;

                if (!MapManager.Instance.IsInsideBoard(pos))
                    break;

                bool occupied = MapManager.Instance.IsTileOccupied(pos);

                // 점프할 말 찾기
                if (!jumped)
                {
                    if (occupied)
                        jumped = true;

                    continue;
                }

                // 점프 후 플레이어 발견
                if (pos == playerPos)
                {
                    TryAttackTarget(player);
                    return;
                }

                // 다른 말 만나면 종료
                if (occupied)
                    break;
            }
        }

        // 이동
        Vector2Int bestMove = Vector2Int.zero;
        int bestDistance = int.MaxValue;

        foreach (var dir in directions)
        {
            for (int i = 1; i <= moveRange; i++)
            {
                Vector2Int pos = CurrentGridPosition + dir * i;

                if (!MapManager.Instance.CanMoveTo(pos))
                    break;

                int distance =
                    Mathf.Abs(playerPos.x - pos.x) +
                    Mathf.Abs(playerPos.y - pos.y);

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
}