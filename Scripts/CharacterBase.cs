using UnityEngine;

public enum Team
{
    Enemy,
    Ally
}

public class CharacterBase : MonoBehaviour
{
    public Team team = Team.Enemy;
    public int MaxHP = 20;
    public int HP;
    public int Attack = 5;
    public int Defense = 2;

    public Vector2Int Position;

    public bool IsDead => HP <= 0;

    private HealthBar2D healthBar;

    protected virtual void Awake()
    {
        healthBar = GetComponent<HealthBar2D>();

        if (healthBar == null)
        {
            healthBar = gameObject.AddComponent<HealthBar2D>();
        }
    }

    protected virtual void Start()
    {
        HP = MaxHP;
        UpdateHealthBar();
    }

    public void TakeDamage(CharacterBase attacker)
    {
        int damage = Mathf.Max(1, attacker.Attack - Defense);
        HP -= damage;

        if (HP < 0)
        {
            HP = 0;
        }

        Debug.Log(attacker.name + " -> " + name + " damage: " + damage + " (HP: " + HP + ")");

        UpdateHealthBar();

        if (HP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log(name + " died");

        if (MapManager.Instance != null)
        {
            MapManager.Instance.RemoveUnit(this);
        }
        else
        {
            Position = new Vector2Int(-1, -1);
        }

        gameObject.SetActive(false);
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetValue(HP, MaxHP);
        }
    }

    public bool TryMove(Vector2Int dir)
    {
        Vector2Int target = Position + dir;

        if (!MapManager.Instance.IsInside(target)) return false;
        if (MapManager.Instance.IsOccupied(target)) return false;

        MapManager.Instance.MoveUnit(this, target);
        return true;
    }

    public bool TryAttack()
    {
        Vector2Int targetPos;
        bool hit = false;
        for (int x = -1; x <= 1; x += 1)
        {
            for (int y = -1; y <= 1; y += 1)
            {
                targetPos = Position + new Vector2Int(x, y);
                Debug.Log(targetPos);
                var target = MapManager.Instance.GetUnit(targetPos);

                if (target != null && target.team != this.team)
                {
                    hit = true;
                    target.TakeDamage(this);
                }
            }
        }
        return hit;
    }
}
