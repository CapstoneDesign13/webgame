using UnityEngine;

public class EnemyUnit : CharacterBase
{
    // 기본 쫄병 AI
    public virtual void TakeTurn(PlayerUnit player)
    {
        if (IsDead) return;

        Vector2Int dir = player.CurrentGridPosition - CurrentGridPosition;

        Vector2Int moveDir = new Vector2Int(
            Mathf.Clamp(dir.x, -1, 1),
            Mathf.Clamp(dir.y, -1, 1)
        );

        // 상하좌우 인접 공격
        if (Mathf.Abs(dir.x) + Mathf.Abs(dir.y) == 1)
        {
            TryAttackTarget(player);
        }
        else
        {
            TryMove(moveDir);
        }
    }
}