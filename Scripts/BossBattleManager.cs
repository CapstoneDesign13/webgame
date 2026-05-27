using System.Collections;
using UnityEngine;

public class BossBattleManager : MonoBehaviour
{
    public static BossBattleManager Instance { get; private set; }

    private const string Phase1KingId = "demonic_king_phase1";
    private const string Phase1AuraId = "demonic_king_phase1_aura";
    private const string Phase2KingId = "demonic_king_phase2";

    private DemonicKingPhase1Unit phase1King;
    private int phase1AuraDeadCount;
    private bool phaseTransitioning;
    private bool victoryTriggered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OnRoomLoaded(RoomType type)
    {
        phaseTransitioning = false;
        victoryTriggered = false;

        if (type == RoomType.BossBattle1)
        {
            phase1AuraDeadCount = 0;
            phase1King = FindUnit<DemonicKingPhase1Unit>(Phase1KingId);

            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(0.4f, 0.18f);
            }
        }
        else if (type == RoomType.BossBattle2)
        {
            phase1King = null;
            phase1AuraDeadCount = 0;

            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(0.5f, 0.22f);
            }
        }
    }

    public void NotifyUnitDied(CharacterBase unit)
    {
        if (unit == null) return;

        if (unit.UnitId == Phase1AuraId)
        {
            HandlePhase1AuraDead();
            return;
        }

        if (unit.UnitId == Phase2KingId)
        {
            HandlePhase2KingDead();
            return;
        }
    }

    private void HandlePhase1AuraDead()
    {
        if (phaseTransitioning) return;

        phase1AuraDeadCount++;

        if (phase1King == null)
        {
            phase1King = FindUnit<DemonicKingPhase1Unit>(Phase1KingId);
        }

        if (phase1King != null)
        {
            phase1King.TakeAuraBreakDamage(24);
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.2f, 0.1f);
        }

        if (phase1AuraDeadCount >= 4)
        {
            StartCoroutine(EnterPhase2());
        }
    }

    private IEnumerator EnterPhase2()
    {
        phaseTransitioning = true;

        yield return new WaitForSeconds(0.6f);

        if (RunManager.Instance != null)
        {
            RunManager.Instance.GoNextRoom();
        }
    }

    private void HandlePhase2KingDead()
    {
        if (victoryTriggered) return;

        victoryTriggered = true;

        if (RunManager.Instance != null)
        {
            RunManager.Instance.CompleteRunFromBoss();
        }
    }

    private T FindUnit<T>(string unitId) where T : CharacterBase
    {
        if (MapManager.Instance == null) return null;

        foreach (CharacterBase unit in MapManager.Instance.AllUnits)
        {
            if (unit == null) continue;
            if (unit.UnitId != unitId) continue;

            return unit as T;
        }

        return null;
    }
}