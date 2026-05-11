using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이 스크립트 역할:
/// - 9x10 장기판형 좌표 맵을 생성하고 관리한다.
/// - 각 좌표를 TileData로 관리한다.
/// - 궁 영역 3x3 판정을 제공한다.
/// - 플레이어와 적 3명을 초기 위치에 배치한다.
/// - 유닛 이동 시 타일 점유 상태를 갱신한다.
/// </summary>

[System.Serializable]
public struct GridPosition
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(x, y);
    }

    public static GridPosition FromVector2Int(Vector2Int value)
    {
        return new GridPosition(value.x, value.y);
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}

[System.Serializable]
public class TileData
{
    public GridPosition Position;

    // 이 좌표가 궁 3x3 영역 안인지 여부
    public bool IsPalace;

    // 현재 이 타일 위에 있는 유닛
    public CharacterBase Occupant;

    public TileData(GridPosition position, bool isPalace)
    {
        Position = position;
        IsPalace = isPalace;
        Occupant = null;
    }
}

[System.Serializable]
public class UnitSpawnSetting
{
    public string displayName;
    public Vector2Int startPosition;
    public int hp;
    public int attack;
    public int defense;

    public UnitSpawnSetting(string displayName, Vector2Int startPosition, int hp, int attack, int defense)
    {
        this.displayName = displayName;
        this.startPosition = startPosition;
        this.hp = hp;
        this.attack = attack;
        this.defense = defense;
    }
}

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    public UIManager ui;

    [Header("Board Size")]
    [SerializeField] private int width = 9;
    [SerializeField] private int height = 10;

    [Header("World Placement")]
    [SerializeField] private float cellSize = 1f;

    [Header("Board Intersection Alignment")]
    [SerializeField] private bool useBoardIntersectionAlignment = true;
    
    [Tooltip("(0, 0) 좌표가 될 장기판 왼쪽 아래 교차점")]
    [SerializeField] private Transform boardPoint_0_0;
    
    [Tooltip("(8, 9) 좌표가 될 장기판 오른쪽 위 교차점")]
    [SerializeField] private Transform boardPoint_8_9;
    
    [Tooltip("2D에서 캐릭터가 배치될 Z 위치")]
    [SerializeField] private float unitZPosition = 0f;
    
    [Tooltip("Scene 뷰에서 교차점 위치를 작은 점으로 보여줄지 여부")]
    [SerializeField] private bool drawIntersectionGizmos = true;

    [Header("Optional Tile Visual")]
    [SerializeField] private GameObject tileVisualPrefab;
    [SerializeField] private Transform tileRoot;

    [Header("Unit Prefabs")]
    [SerializeField] private PlayerUnit playerPrefab;
    [SerializeField] private EnemyUnit enemyPrefab;
    [SerializeField] private Transform unitRoot;

    [Header("Spawn Settings")]
    [SerializeField] private bool spawnUnitsOnAwake = true;

    [SerializeField]
    private UnitSpawnSetting playerSpawn =
        new UnitSpawnSetting("Player", new Vector2Int(4, 0), 100, 20, 5);

    [SerializeField]
    private List<UnitSpawnSetting> enemySpawns = new List<UnitSpawnSetting>()
    {
        new UnitSpawnSetting("Enemy 1", new Vector2Int(0, 9), 50, 12, 3),
        new UnitSpawnSetting("Enemy 2", new Vector2Int(4, 9), 60, 10, 4),
        new UnitSpawnSetting("Enemy 3", new Vector2Int(8, 9), 70, 15, 2)
    };

    [Header("Death Handling")]
    [SerializeField] private bool deactivateUnitOnDeath = true;

    private TileData[,] tiles;

    private readonly List<CharacterBase> allUnits = new List<CharacterBase>();
    private readonly List<EnemyUnit> enemies = new List<EnemyUnit>();

    public PlayerUnit Player { get; private set; }

    public IReadOnlyList<CharacterBase> AllUnits
    {
        get { return allUnits; }
    }

    public IReadOnlyList<EnemyUnit> Enemies
    {
        get { return enemies; }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        GenerateMap();

        if (spawnUnitsOnAwake)
        {
            SpawnInitialUnits();
        }
    }

    /// <summary>
    /// 9x10 타일 데이터를 생성한다.
    /// tileVisualPrefab이 있으면 월드에 간단한 시각 오브젝트도 생성한다.
    /// </summary>
    private void GenerateMap()
    {
        tiles = new TileData[width, height];

        Transform parent = tileRoot != null ? tileRoot : transform;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                bool isPalace = IsInsidePalace(pos);

                tiles[x, y] = new TileData(new GridPosition(x, y), isPalace);

                if (tileVisualPrefab != null)
                {
                    GameObject tileObject = Instantiate(
                        tileVisualPrefab,
                        GridToWorld(pos),
                        Quaternion.identity,
                        parent
                    );

                    tileObject.name = isPalace
                        ? "Tile_" + x + "_" + y + "_Palace"
                        : "Tile_" + x + "_" + y;
                }
            }
        }

        Debug.Log("Map generated: " + width + "x" + height);
    }

    /// <summary>
    /// 플레이어 1명과 적 3명을 기본 위치에 생성한다.
    /// Prefab을 Inspector에 연결해야 실제 GameObject가 생성된다.
    /// </summary>
    public void SpawnInitialUnits()
    {
        if (allUnits.Count > 0)
        {
            Debug.LogWarning("이미 유닛이 배치되어 있습니다.");
            return;
        }

        if (enemySpawns.Count != 3)
        {
            Debug.LogWarning("현재 적 시작 설정 개수: " + enemySpawns.Count + ". 요구사항 기준은 적 3명입니다.");
        }

        Transform parent = unitRoot != null ? unitRoot : transform;

        if (playerPrefab != null)
        {
            PlayerUnit player = Instantiate(playerPrefab, parent);
            player.SetupStats(
                playerSpawn.displayName,
                Team.Ally,
                playerSpawn.hp,
                playerSpawn.attack,
                playerSpawn.defense
            );
            player.ui = ui;
            PlaceUnit(player, playerSpawn.startPosition);
        }
        else
        {
            Debug.LogWarning("Player Prefab이 MapManager에 연결되지 않았습니다.");
        }

        if (enemyPrefab != null)
        {
            for (int i = 0; i < enemySpawns.Count; i++)
            {
                UnitSpawnSetting setting = enemySpawns[i];

                EnemyUnit enemy = Instantiate(enemyPrefab, parent);
                enemy.SetupStats(
                    setting.displayName,
                    Team.Enemy,
                    setting.hp,
                    setting.attack,
                    setting.defense
                );

                PlaceUnit(enemy, setting.startPosition);
            }
        }
        else
        {
            Debug.LogWarning("Enemy Prefab이 MapManager에 연결되지 않았습니다.");
        }
    }
    
    /// <summary>
    /// 보드 좌표를 Unity 월드 좌표로 변환한다.
    /// 장기판에서는 기물이 칸 중앙이 아니라 선과 선이 만나는 교차점 중앙에 놓여야 한다.
    /// 따라서: - (0, 0)은 왼쪽 아래 교차점, - (8, 9)는 오른쪽 위 교차점으로 보고, 그 사이를 균등하게 나눈다.
    /// </summary>
    public Vector3 GridToWorld(Vector2Int boardPosition)
    {
        // 보정용 기준점이 연결되어 있으면 장기판 이미지의 실제 교차점 기준으로 계산한다.
        if (useBoardIntersectionAlignment &&
            boardPoint_0_0 != null &&
            boardPoint_8_9 != null)
        {
            Vector3 bottomLeft = boardPoint_0_0.position;
            Vector3 topRight = boardPoint_8_9.position;

            // width = 9이면 교차점은 9개이고, 간격은 8칸이다.
            // height = 10이면 교차점은 10개이고, 간격은 9칸이다.
            float stepX = (topRight.x - bottomLeft.x) / (width - 1);
            float stepY = (topRight.y - bottomLeft.y) / (height - 1);
            float worldX = bottomLeft.x + boardPosition.x * stepX;
            float worldY = bottomLeft.y + boardPosition.y * stepY;
            return new Vector3(worldX, worldY, unitZPosition);
        }

        // 기준점이 없을 때 사용하는 기본 2D 좌표 방식
        return new Vector3(
            boardPosition.x * cellSize,
            boardPosition.y * cellSize,
            unitZPosition
        );
    }

    /// <summary>
    /// 좌표가 보드 안에 있는지 확인한다.
    /// </summary>
    public bool IsInsideBoard(Vector2Int boardPosition)
    {
        return boardPosition.x >= 0 &&
               boardPosition.x < width &&
               boardPosition.y >= 0 &&
               boardPosition.y < height;
    }

    public bool IsInsideBoard(GridPosition position)
    {
        return IsInsideBoard(position.ToVector2Int());
    }

    /// <summary>
    /// 궁 영역인지 확인한다.
    /// 아래쪽 궁: x 3~5, y 0~2
    /// 위쪽 궁: x 3~5, y 7~9
    /// </summary>
    public bool IsInsidePalace(Vector2Int boardPosition)
    {
        bool insidePalaceX = boardPosition.x >= 3 && boardPosition.x <= 5;

        bool insideBottomPalaceY = boardPosition.y >= 0 && boardPosition.y <= 2;
        bool insideTopPalaceY = boardPosition.y >= height - 3 && boardPosition.y <= height - 1;

        return insidePalaceX && (insideBottomPalaceY || insideTopPalaceY);
    }

    public bool IsInsidePalace(GridPosition position)
    {
        return IsInsidePalace(position.ToVector2Int());
    }

    /// <summary>
    /// 특정 좌표의 TileData를 가져온다.
    /// 보드 밖이면 null을 반환한다.
    /// </summary>
    public TileData GetTile(Vector2Int boardPosition)
    {
        if (!IsInsideBoard(boardPosition))
        {
            return null;
        }

        return tiles[boardPosition.x, boardPosition.y];
    }

    /// <summary>
    /// 특정 좌표에 유닛이 있는지 확인한다.
    /// </summary>
    public bool IsTileOccupied(Vector2Int boardPosition)
    {
        TileData tile = GetTile(boardPosition);

        if (tile == null)
        {
            return false;
        }

        return tile.Occupant != null;
    }

    /// <summary>
    /// 특정 좌표의 유닛을 가져온다.
    /// </summary>
    public CharacterBase GetUnitAt(Vector2Int boardPosition)
    {
        TileData tile = GetTile(boardPosition);

        if (tile == null)
        {
            return null;
        }

        return tile.Occupant;
    }

    /// <summary>
    /// 이동 가능한 좌표인지 확인한다.
    /// 조건:
    /// - 보드 안이어야 함
    /// - 다른 유닛이 없어야 함
    /// </summary>
    public bool CanMoveTo(Vector2Int boardPosition)
    {
        if (!IsInsideBoard(boardPosition))
        {
            return false;
        }

        if (IsTileOccupied(boardPosition))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 유닛을 보드의 특정 위치에 최초 배치한다.
    /// </summary>
    public bool PlaceUnit(CharacterBase unit, Vector2Int boardPosition)
    {
        if (unit == null)
        {
            Debug.LogWarning("배치할 유닛이 null입니다.");
            return false;
        }

        if (!CanMoveTo(boardPosition))
        {
            Debug.LogWarning(unit.name + " 배치 실패. 위치: " + boardPosition);
            return false;
        }

        TileData tile = GetTile(boardPosition);
        tile.Occupant = unit;

        if (!allUnits.Contains(unit))
        {
            allUnits.Add(unit);
        }

        PlayerUnit playerUnit = unit as PlayerUnit;
        if (playerUnit != null)
        {
            Player = playerUnit;
        }

        EnemyUnit enemyUnit = unit as EnemyUnit;
        if (enemyUnit != null && !enemies.Contains(enemyUnit))
        {
            enemies.Add(enemyUnit);
        }

        unit.SetGridPosition(boardPosition);

        Debug.Log(unit.name + " placed at " + boardPosition);
        return true;
    }

    /// <summary>
    /// 유닛을 다른 좌표로 이동시킨다.
    /// 기존 타일 점유를 비우고, 새 타일 점유를 설정한다.
    /// </summary>
    public bool MoveUnit(CharacterBase unit, Vector2Int targetPosition)
    {
        if (unit == null || !unit.IsAlive)
        {
            return false;
        }

        if (!CanMoveTo(targetPosition))
        {
            Debug.Log(unit.name + " 이동 실패. 목표 위치: " + targetPosition);
            return false;
        }

        Vector2Int currentPosition = unit.CurrentGridPosition;

        if (IsInsideBoard(currentPosition))
        {
            TileData currentTile = GetTile(currentPosition);

            if (currentTile != null && currentTile.Occupant == unit)
            {
                currentTile.Occupant = null;
            }
        }

        TileData targetTile = GetTile(targetPosition);
        targetTile.Occupant = unit;

        unit.SetGridPosition(targetPosition);

        Debug.Log(unit.name + " moved to " + targetPosition);
        return true;
    }

    /// <summary>
    /// 사망한 유닛을 보드 점유 목록에서 제거한다.
    /// 프로토타입에서는 GameObject를 비활성화한다.
    /// </summary>
    public void RemoveUnit(CharacterBase unit)
    {
        if (unit == null)
        {
            return;
        }

        Vector2Int pos = unit.CurrentGridPosition;

        if (IsInsideBoard(pos))
        {
            TileData tile = GetTile(pos);

            if (tile != null && tile.Occupant == unit)
            {
                tile.Occupant = null;
            }
        }

        allUnits.Remove(unit);

        EnemyUnit enemyUnit = unit as EnemyUnit;
        if (enemyUnit != null)
        {
            enemies.Remove(enemyUnit);
        }

        if (deactivateUnitOnDeath)
        {
            unit.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 현재 살아 있는 적 목록을 반환한다.
    /// </summary>
    public List<EnemyUnit> GetLivingEnemies()
    {
        List<EnemyUnit> result = new List<EnemyUnit>();

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyUnit enemy = enemies[i];

            if (enemy != null && enemy.IsAlive)
            {
                result.Add(enemy);
            }
        }

        return result;
    }

    /// <summary>
    /// 두 좌표가 상하좌우 인접인지 확인한다.
    /// 기본 공격 판정에 사용한다.
    /// </summary>
    public bool IsAdjacent4(Vector2Int a, Vector2Int b)
    {
        int distance = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        return distance == 1;
    }

    /// <summary>
    /// 입력 방향이 상하좌우 1칸 방향인지 확인한다.
    /// </summary>
    public bool IsCardinalDirection(Vector2Int direction)
    {
        return Mathf.Abs(direction.x) + Mathf.Abs(direction.y) == 1;
    }

    /// <summary>
    /// 입력 방향이 대각선 1칸 방향인지 확인한다.
    /// 궁 대각 이동 스킬에 사용한다.
    /// </summary>
    public bool IsDiagonalDirection(Vector2Int direction)
    {
        return Mathf.Abs(direction.x) == 1 && Mathf.Abs(direction.y) == 1;
    }
    /// <summary>
/// 특정 유닛이 현재 차지하고 있는 타일 점유 정보만 비운다.
/// 
/// 일기토 시작 시 여러 유닛을 한꺼번에 재배치하기 위해 사용한다.
/// 유닛 GameObject 자체를 삭제하지는 않는다.
/// </summary>
public void ClearUnitOccupationOnly(CharacterBase unit)
{
    if (unit == null)
    {
        return;
    }

    Vector2Int currentPos = unit.CurrentGridPosition;

    if (!IsInsideBoard(currentPos))
    {
        return;
    }

    TileData currentTile = GetTile(currentPos);

    if (currentTile != null && currentTile.Occupant == unit)
    {
        currentTile.Occupant = null;
    }
}

/// <summary>
/// 유닛을 특정 보드 좌표로 강제로 이동시킨다.
/// 
/// 일반 이동 규칙을 검사하지 않는다.
/// 일기토 시작 연출처럼 "기존 위치와 상관없이 특정 위치로 모으기"에 사용한다.
/// </summary>
public bool ForceSetUnitPosition(CharacterBase unit, Vector2Int targetPosition)
{
    if (unit == null)
    {
        return false;
    }

    if (!IsInsideBoard(targetPosition))
    {
        Debug.LogWarning("ForceSetUnitPosition 실패. 보드 밖 위치: " + targetPosition);
        return false;
    }

    TileData targetTile = GetTile(targetPosition);

    if (targetTile == null)
    {
        return false;
    }

    // 혹시 targetPosition에 다른 유닛 정보가 남아 있어도,
    // 일기토 재배치에서는 강제로 덮어쓴다.
    targetTile.Occupant = unit;

    unit.SetGridPosition(targetPosition);

    Debug.Log(unit.name + " moved to duel position " + targetPosition);

    return true;
    }
    /// <summary>
    /// Scene 뷰에서 9x10 교차점 위치를 작은 점으로 보여준다.
    /// 실제 게임 화면에는 표시되지 않는다.
    /// 이 기능은 캐릭터가 장기판 교차점 위에 정확히 놓이는지 확인하기 위한 디버그용이다.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!drawIntersectionGizmos)
        {
            return;
        }
        if (!useBoardIntersectionAlignment)
        {
            return;
        }
        if (boardPoint_0_0 == null || boardPoint_8_9 == null)
        {
            return;
        }

        if (width <= 1 || height <= 1)
        {
            return;
        }
        
        Vector3 bottomLeft = boardPoint_0_0.position;
        Vector3 topRight = boardPoint_8_9.position;
        float stepX = (topRight.x - bottomLeft.x) / (width - 1);
        float stepY = (topRight.y - bottomLeft.y) / (height - 1);
        
        Gizmos.color = Color.cyan;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 point = new Vector3(
                    bottomLeft.x + x * stepX,
                    bottomLeft.y + y * stepY,
                    unitZPosition
                );
                Gizmos.DrawSphere(point, 0.05f);
            }
        }
    }
}