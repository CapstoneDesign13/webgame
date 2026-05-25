using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveStatusEffect
{
    public string id;
    public int turnsRemaining;
    public int stacks;
}

public class StatusEffectController : MonoBehaviour
{
    private CharacterBase owner;
    private readonly List<ActiveStatusEffect> activeEffects = new List<ActiveStatusEffect>();
    public GameObject[] icons;

    public int AttackModifier
    {
        get
        {
            int value = 0;

            foreach (ActiveStatusEffect effect in activeEffects)
            {
                StatusEffectData data = GetData(effect.id);

                if (data != null)
                {
                    value += data.attackModifier * effect.stacks;
                }
            }

            return value;
        }
    }

    public int DefenseModifier
    {
        get
        {
            int value = 0;

            foreach (ActiveStatusEffect effect in activeEffects)
            {
                StatusEffectData data = GetData(effect.id);

                if (data != null)
                {
                    value += data.defenseModifier * effect.stacks;
                }
            }

            return value;
        }
    }

    public bool DisableMove
    {
        get
        {
            foreach (ActiveStatusEffect effect in activeEffects)
            {
                StatusEffectData data = GetData(effect.id);

                if (data != null && data.disableMove)
                {
                    return true;
                }
            }

            return false;
        }
    }

    Dictionary<string, int> map = new Dictionary<string, int>()
    {
        {"Poison", 1},
        {"Bleed", 2},
        {"Paralysis", 3},
        {"Burn", 4},
        {"Weakness", 5}
    };
    private void Awake()
    {
        owner = GetComponent<CharacterBase>();
    }

    public void AddStatus(string statusId, int stacksToAdd = 1, int durationOverride = -1)
    {
        if (string.IsNullOrEmpty(statusId))
        {
            return;
        }

        StatusEffectData data = GetData(statusId);

        if (data == null)
        {
            Debug.LogWarning("상태 이상 데이터가 없습니다: " + statusId);
            return;
        }

        ActiveStatusEffect existing = activeEffects.Find(x => x.id == statusId);

        int duration = durationOverride > 0 ? durationOverride : data.defaultDuration;
        int maxStacks = Mathf.Max(1, data.maxStacks);

        if (existing == null)
        {
            activeEffects.Add(new ActiveStatusEffect
            {
                id = statusId,
                turnsRemaining = Mathf.Max(1, duration),
                stacks = Mathf.Clamp(stacksToAdd, 1, maxStacks)
            });
        }
        else
        {
            existing.turnsRemaining = Mathf.Max(existing.turnsRemaining, duration);
            existing.stacks = Mathf.Clamp(existing.stacks + stacksToAdd, 1, maxStacks);
        }

        Debug.Log(owner.name + " 상태 이상 적용: " + data.displayName);
        map.TryGetValue(statusId, out int iconid);
        if (iconid > 0)
            icons[iconid].SetActive(true);
    }

    public void Tick(StatusTickTiming timing)
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            ActiveStatusEffect effect = activeEffects[i];
            StatusEffectData data = GetData(effect.id);

            if (data == null)
            {
                activeEffects.RemoveAt(i);
                continue;
            }

            if (data.tickTiming != timing)
            {
                continue;
            }

            if (data.tickDamage > 0 && owner != null && owner.IsAlive)
            {
                int damage = data.tickDamage * Mathf.Max(1, effect.stacks);
                owner.TakeFlatDamage(damage, data.displayName);
            }

            effect.turnsRemaining--;

            if (effect.turnsRemaining <= 0)
            {
                Debug.Log(owner.name + " 상태 이상 종료: " + data.displayName);
                activeEffects.RemoveAt(i);
                map.TryGetValue(data.id, out int iconid);
                if (iconid > 0)
                    icons[iconid].SetActive(false);
            }
        }
    }

    private StatusEffectData GetData(string statusId)
    {
        if (ModDatabase.Instance == null)
        {
            return null;
        }

        if (ModDatabase.Instance.statusEffectDatabase == null)
        {
            return null;
        }

        ModDatabase.Instance.statusEffectDatabase.TryGetValue(statusId, out StatusEffectData data);
        return data;
    }
}