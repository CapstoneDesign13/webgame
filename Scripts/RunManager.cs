using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [SerializeField] private RoomBootstrap roomBootstrap;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private RoomDefinition[] rooms;

    [SerializeField] private Vector2Int playerStartPosition = new Vector2Int(4, 0);

    private int roomIndex = -1;
    private bool transitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        GoNextRoom();
    }

    public void GoNextRoom()
    {
        if (rooms == null || rooms.Length == 0)
        {
            Debug.LogError("RunManager: rooms가 비어 있습니다.");
            return;
        }
        roomIndex++;
        if (roomIndex >= rooms.Length)
        {
            Debug.Log("RUN CLEAR");
            return;
        }
        transitioning = false;
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
        roomBootstrap.LoadRoom(rooms[roomIndex]);
        if (turnManager != null)
        {
            turnManager.StartPlayerTurn();
        }
    }

    public bool TryClearBattle()
    {
        if (transitioning)
        {
            return false;
        }
        if (roomBootstrap.CurrentType != RoomType.Battle)
        {
            return false;
        }
        if (MapManager.Instance.GetLivingEnemies().Count > 0)
        {
            return false;
        }
        transitioning = true;
        Debug.Log("STAGE CLEAR");
        GoNextRoom();
        return true;
    }
}