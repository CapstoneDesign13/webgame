using UnityEngine;

public class DemonicKingPhase1Unit : KingUnit
{
    public override void TakeTurn(PlayerUnit player)
    {
        // 1페이즈 천마는 아무 행동도 하지 않음
    }

    public override void TakeDamage(CharacterBase attacker, bool pierce = false)
    {
        Debug.Log($"{name} is protected by aura.");
    }

    public override void TakeFlatDamage(int damage, string source = "Status")
    {
        Debug.Log($"{name} ignores flat damage: {source}");
    }

    public void TakeAuraBreakDamage(int damage)
    {
        base.TakeFlatDamage(damage, "Aura Break");
    }
}