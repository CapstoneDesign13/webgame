using UnityEngine;

public class DemonicKingPhase2Unit : KingUnit
{
    private static readonly Vector2Int[] QueenDirs =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
    };

    public override void TakeTurn(PlayerUnit player)
    {
        if (IsDead || player == null || !player.IsAlive) return;

        Vector2Int playerPos = player.CurrentGridPosition;
        Vector2Int delta = playerPos - CurrentGridPosition;

        if (Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y)) == 1)
        {
            TryAttackTarget(player);
            return;
        }

        Vector2Int bestTarget = CurrentGridPosition;
        int bestDistance = int.MaxValue;

        foreach (Vector2Int dir in QueenDirs)
        {
            for (int step = 1; step <= 9; step++)
            {
                Vector2Int candidate = CurrentGridPosition + dir * step;

                if (!MapManager.Instance.IsInsideBoard(candidate))
                {
                    break;
                }

                if (MapManager.Instance.IsTileOccupied(candidate))
                {
                    break;
                }

                int distance =
                    Mathf.Abs(playerPos.x - candidate.x)
                    + Mathf.Abs(playerPos.y - candidate.y);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTarget = candidate;
                }
            }
        }

        if (bestTarget != CurrentGridPosition)
        {
            Vector2Int moveDelta = bestTarget - CurrentGridPosition;

            if (TryMove(moveDelta))
            {
                CameraShake shaker = CameraShake.Instance;
                if (shaker != null)
                {
                    shaker.Shake();
                }
            }
        }
    }
}