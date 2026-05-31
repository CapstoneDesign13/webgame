using UnityEngine;

// 선비(士)
public class GuardUnit : EnemyUnit
{
    private readonly Vector2Int[] diagonalMoves =
    {
        new Vector2Int( 1,  0),
        new Vector2Int( -1, 0),
        new Vector2Int(0,  1),
        new Vector2Int(0, -1),
    };

    public override void TakeTurn(PlayerUnit player)
    {
        if (IsDead) return;

        Vector2Int playerPos = player.CurrentGridPosition;

        // 공격
        foreach (var move in diagonalMoves)
        {
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

        foreach (var move in diagonalMoves)
        {
            Vector2Int targetPos = CurrentGridPosition + move;

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
                bestMove = move;
            }
        }

        if (bestMove != Vector2Int.zero)
        {
            TryMove(bestMove);
        }
    }
}