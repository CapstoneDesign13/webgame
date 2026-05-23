using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public int Money { get; private set; }

    public void Earn(int amount)
    {
        Money += amount;
    }

    public bool TrySpend(int amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            return true;
        }
        return false;
    }
}
