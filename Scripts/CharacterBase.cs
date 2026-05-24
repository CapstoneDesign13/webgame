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
    public string on_hit_status_id;
    private StatusEffectController statusEffects;
    public StatusEffectController StatusEffects
    {
        get
        {
            if (statusEffects == null)
            {
                statusEffects = GetComponent<StatusEffectController>();
            }

            if (statusEffects == null)
            {
                statusEffects = gameObject.AddComponent<StatusEffectController>();
            }

            return statusEffects;
        }
    }
    
    protected int StatusAttackModifier
    {
        get
        {
            return StatusEffects != null ? StatusEffects.AttackModifier : 0;
        }
    }
    protected int StatusDefenseModifier
    {
        get
        {
            return StatusEffects != null ? StatusEffects.DefenseModifier : 0;
        }
    }
    
    public virtual int Attack
    {
        get
        {
            return Mathf.Max(0, _Attack + StatusAttackModifier);
        }
    }
    public virtual int Defense
    {
        get
        {
            return Mathf.Max(0, _Defense + StatusDefenseModifier);
        }
    }

    public virtual bool Camo
    {
        get
        {
            return false;
        }
    }

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

        statusEffects = GetComponent<StatusEffectController>();

        if (statusEffects == null)
        {
            statusEffects = gameObject.AddComponent<StatusEffectController>();
        }

        InitializeHealthBar();
    }

    /// <summary>
    /// 프로토타입용 스탯 세팅 함수.
    /// MapManager가 유닛 생성 직후 호출한다.
    /// </summary>
    public virtual void SetupStats(string newName, Team team, int hp, int atk, int def, string onHitStatusId = "")
    {
        name = newName;
        this.team = team;
        MaxHP = Mathf.Max(1, hp);
        _Attack = Mathf.Max(0, atk);
        _Defense = Mathf.Max(0, def);

        on_hit_status_id = onHitStatusId;

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

    public void TakeDamage(CharacterBase attacker, bool pierce = false)
    {
        //은신 상태에서 무시
        if (Camo)
            return;

        int damage = pierce ? Mathf.Max(1, attacker.Attack) : Mathf.Max(1, attacker.Attack - Defense);
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

    public void TakeFlatDamage(int damage, string source = "Status")
    {
        if (IsDead)
        {
            return;
        }
        
        damage = Mathf.Max(0, damage);
        
        if (damage <= 0)
        {
            return;
        }
        
        HP -= damage;
        
        if (HP < 0)
        {
            HP = 0;
        }
        
        Debug.Log(source + " -> " + name + " fixed damage: " + damage + " (HP: " + HP + ")");
        
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
        if (StatusEffects != null && StatusEffects.DisableMove)
        {
            Debug.Log(name + " 이동 불가 상태입니다.");
            return false;
        }
        Vector2Int target = CurrentGridPosition + dir;

        if (!MapManager.Instance.IsInsideBoard(target)) return false;
        if (MapManager.Instance.IsTileOccupied(target)) return false;

        MapManager.Instance.MoveUnit(this, target);
        return true;
    }
    
    private void ApplyOnHitStatus(CharacterBase target)
    {
        if (target == null)
        {
            return;
        }
        
        /*if (string.IsNullOrEmpty(passive_id))
        {
            return;
        }*/

        target.StatusEffects.AddStatus(on_hit_status_id);
    }
    
    public void TickStatus(StatusTickTiming timing)
    {
        StatusEffects.Tick(timing);
    }

    public bool TryAttackGrid(Vector2Int grid, bool pierce = false)
    {
        var targetPos = CurrentGridPosition + grid;
        var target = MapManager.Instance.GetUnitAt(targetPos);
        if (target != null && target.team != this.team)
        {
            target.TakeDamage(this, pierce);
            ApplyOnHitStatus(target);
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
        if (character == null)
        {
            return false;
        }
        character.TakeDamage(this);
        ApplyOnHitStatus(character);
        
        return true;
    }
}