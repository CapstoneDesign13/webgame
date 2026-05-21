using UnityEngine;

public enum Team
{
    Enemy,
    Ally,
    Neutral
}

public class CharacterBase : MonoBehaviour
{
    public Team team = Team.Enemy;
    public int MaxHP = 20;
    public int HP;
    protected int _Attack = 4;
    protected int _Defense = 1;
    public virtual int Attack { get { return _Attack; } }
    public virtual int Defense { get { return _Defense; } }

    [SerializeField] protected GridPosition currentPosition;

    public bool IsAlive => HP > 0;
    public bool IsDead => HP <= 0;

    [Header("Health Bar")]
    [SerializeField] private bool showHealthBar = true;
    [SerializeField] private HealthBar2D healthBar;

    protected virtual void Awake()
    {
        if (HP <= 0)
        {
            HP = MaxHP;
        }

        InitializeHealthBar();
    }

    /// <summary>
    /// 프로토타입용 스탯 세팅 함수.
    /// MapManager가 유닛 생성 직후 호출한다.
    /// </summary>
    public virtual void SetupStats(string newName, Team team, int hp, int atk, int def)
    {
        name = newName;
        this.team = team;
        MaxHP = Mathf.Max(1, hp);
        _Attack = Mathf.Max(0, atk);
        _Defense = Mathf.Max(0, def);

        HP = MaxHP;

        InitializeHealthBar();
        UpdateHealthBar();
    }

    /// <summary>
    /// 유닛의 보드 좌표를 갱신하고, Transform 위치도 월드 좌표에 맞춘다.
    /// </summary>
    public void SetGridPosition(Vector2Int newPosition)
    {
        currentPosition = GridPosition.FromVector2Int(newPosition);

        if (MapManager.Instance != null)
        {
            transform.position = MapManager.Instance.GridToWorld(newPosition);
        }
    }

    public Vector2Int CurrentGridPosition
    {
        get { return currentPosition.ToVector2Int(); }
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

    public virtual bool Answer()
    {
        Debug.Log("대화가 통할 상대가 아니다!");
        return false;
    }

    public void Die()
    {
        HP = 0;

        UpdateHealthBar();

        Debug.Log(name + " died");

        if (MapManager.Instance != null)
        {
            MapManager.Instance.RemoveUnit(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 체력 바를 준비한다.
    /// PlayerUnit과 EnemyUnit 모두 CharacterBase를 상속하므로,
    /// 모든 유닛에게 자동으로 체력 바가 붙는다.
    /// </summary>
    private void InitializeHealthBar()
    {
        if (!showHealthBar)
        {
            return;
        }

        if (healthBar == null)
        {
            healthBar = GetComponent<HealthBar2D>();
        }

        if (healthBar == null)
        {
            healthBar = gameObject.AddComponent<HealthBar2D>();
        }

        UpdateHealthBar();
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
        Vector2Int target = CurrentGridPosition + dir;

        if (!MapManager.Instance.IsInsideBoard(target)) return false;
        if (MapManager.Instance.IsTileOccupied(target)) return false;

        MapManager.Instance.MoveUnit(this, target);
        return true;
    }

    public bool TryAttackGrid(Vector2Int grid)
    {
        var targetPos = CurrentGridPosition + grid;
        var target = MapManager.Instance.GetUnitAt(targetPos);
        if (target != null && target.team != this.team)
        {
            target.TakeDamage(this);
            return true;
        }
        return false;
    }

    public bool TryAttack()
    {
        bool hit = false;
        for (int x = -1; x <= 1; x += 1)
        {
            for (int y = -1; y <= 1; y += 1)
            {
                hit = TryAttackGrid(new Vector2Int(x, y));
            }
        }
        return hit;
    }

    public bool TryAttackTarget(CharacterBase character)
    {
        bool hit = false;
        character.TakeDamage(this);
        return hit;
    }
}