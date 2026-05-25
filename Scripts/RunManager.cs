using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [SerializeField] private RoomBootstrap roomBootstrap;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private WindowManager window;
    [SerializeField] private RoomDefinition[] rooms;
    [SerializeField] private PlayerUnit player;
    [SerializeField] private EconomyManager economy;

    [SerializeField] private Vector2Int playerStartPosition = new Vector2Int(4, 0);

    private int roomIndex = -1;
    private bool transitioning = false;
    private bool runStarted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void StartRun()
    {
        if (runStarted)
        {
            return;
        }

        runStarted = true;

        if (rooms == null || rooms.Length == 0)
        {
            Debug.LogError("RunManager: rooms가 비어 있습니다.");
            return;
        }

        LoadRoomIndex(0);
    }

    public void GoNextRoom()
    {
        LoadRoomIndex(roomIndex + 1);
    }

    public void GoNextBattleRoom()
    {
        int nextBattleIndex = FindNextRoomIndex(RoomType.Battle);

        if (nextBattleIndex < 0)
        {
            CompleteRun();
            return;
        }

        LoadRoomIndex(nextBattleIndex);
    }

    public void SelectVisitMerchant()
    {
        CloseStageClearChoice();

        int nextIndex = roomIndex + 1;

        if (IsValidRoomIndex(nextIndex) && rooms[nextIndex].type == RoomType.Merchant)
        {
            LoadRoomIndex(nextIndex);
            return;
        }
        // 다음 칸에 상점이 없으면 바로 다음 전투로 보낸다.
        GoNextBattleRoom();
    }

    public void SelectSkipMerchant()
    {
        CloseStageClearChoice();
        GoNextBattleRoom();
    }

    public bool TryClearBattle()
    {
        Debug.Log($"transitioning:{transitioning},방타입:{roomBootstrap.CurrentType},적수:{MapManager.Instance.GetLivingEnemies().Count}");
        if (transitioning)
        {
            return false;
        }

        if (roomBootstrap == null)
        {
            Debug.LogWarning("RunManager: roomBootstrap이 연결되어 있지 않습니다.");
            return false;
        }

        if (roomBootstrap.CurrentType != RoomType.Battle)
        {
            return false;
        }

        if (MapManager.Instance == null)
        {
            return false;
        }

        if (MapManager.Instance.GetLivingEnemies().Count > 0)
        {
            return false;
        }

        EndBattle("NORMAL");

        return true;
    }

    public void CompleteBattleFromDuel()
    {
        if (transitioning)
        {
            return;
        }
        if (roomBootstrap == null)
        {
            Debug.LogWarning("RunManager: roomBootstrap이 연결되어 있지 않습니다.");
            return;
        }
        if (roomBootstrap.CurrentType != RoomType.Battle)
        {
            Debug.LogWarning("RunManager: 현재 방이 Battle이 아니므로 일기토 클리어를 처리하지 않습니다.");
            return;
        }
        
        EndBattle("DUEL");
    }

    public void EndBattle(string source)
    {
        player.AtBattleEnd();
        economy.Earn(10);

        transitioning = true;
        Debug.Log($"STAGE CLEAR BY {source}");
        OpenStageClearChoice();
    }

    private void OpenStageClearChoice()
    {
        if (window != null && window.oraclePanel != null)
        {
            window.OpenOracle();
            return;
        }
        //레거시
        if (window != null && window.stageClearPanel != null)
        {
            window.OpenStageClear();
            return;
        }
        // 선택 UI가 아직 없을 때의 임시 동작
        GoNextRoom();
    }

    private void CloseStageClearChoice()
    {
        if (window != null)
        {
            window.CloseAll();
        }
    }

    private int FindNextRoomIndex(RoomType type)
    {
        if (rooms == null)
        {
            return -1;
        }

        for (int i = roomIndex + 1; i < rooms.Length; i++)
        {
            if (rooms[i].type == type)
            {
                return i;
            }
        }

        return -1;
    }

    private bool IsValidRoomIndex(int index)
    {
        return rooms != null && index >= 0 && index < rooms.Length;
    }

    private void LoadRoomIndex(int index)
    {
        if (rooms == null || rooms.Length == 0)
        {
            Debug.LogError("RunManager: rooms가 비어 있습니다.");
            return;
        }

        if (index >= rooms.Length)
        {
            CompleteRun();
            return;
        }

        if (index < 0)
        {
            Debug.LogError("RunManager: 잘못된 room index입니다. index = " + index);
            return;
        }

        if (roomBootstrap == null)
        {
            Debug.LogError("RunManager: roomBootstrap이 연결되어 있지 않습니다.");
            return;
        }

        roomIndex = index;
        CleanUp(rooms[roomIndex]);

        Debug.Log($"ROOM LOADED: index={roomIndex}, type={rooms[roomIndex].type}, stageKey={rooms[roomIndex].stageKey}");
    }

    public void CleanUp(RoomDefinition RD)
    {
        transitioning = false;

        if (window != null)
        {
            window.CloseAll();
        }

        MapManager map = MapManager.Instance;

        if (map != null)
        {
            map.ClearRoomUnitsExceptPlayer();
            map.RepositionPlayer(playerStartPosition);
        }

        if (turnManager != null)
        {
            turnManager.ResetForNewRoom();
        }

        roomBootstrap.LoadRoom(RD);

        if (turnManager != null)
        {
            turnManager.StartPlayerTurn();
        }
    }

    private void CompleteRun()
    {
        transitioning = true;
        Debug.Log("RUN CLEAR");

        if (window != null)
        {
            window.CloseAll();
        }
        // TODO: 최종 클리어 UI 연결
    }
}