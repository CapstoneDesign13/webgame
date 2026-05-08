using UnityEngine;

public class EnemyUnit : CharacterBase
{
    public void TakeTurn(PlayerUnit player)
    {
        if (IsDead) return;

        Vector2Int dir = player.Position - Position;

        Vector2Int moveDir = new Vector2Int(
            Mathf.Clamp(dir.x, -1, 1),
            Mathf.Clamp(dir.y, -1, 1)
        );

        // 공격 가능하면 공격
        if (Mathf.Abs(dir.x) + Mathf.Abs(dir.y) == 1)
        {
            TryAttack();
        }
        else
        {
            TryMove(moveDir);
        }
    }
}