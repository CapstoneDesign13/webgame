using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    private int _money = 50;
    public int Money { get { return _money; } private set { _money = value; } }

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
