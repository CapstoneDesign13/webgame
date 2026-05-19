using UnityEngine;

public class InitHandler : MonoBehaviour
{
    public ResourceLoader resourceLoader;
    public SpawnManager spawn;
    public RoomBootstrap room;
    public TurnManager turn;
    public PlayerUnit player;
    

    [SerializeField]
    private readonly Vector2Int initpos = new Vector2Int(4, 0);

    void Start()
    {
        // Create shared database
        ModDatabase sharedDatabase = ScriptableObject.CreateInstance<ModDatabase>();
        sharedDatabase.Initialize(); // set the singleton instance

        resourceLoader.database = sharedDatabase;

        resourceLoader.LoadMods();

        MapManager.Instance.PlaceUnit(player, initpos);
        if (player.team != Team.Ally)
            Debug.LogError("주인공 팀이 이상합니다.");
        room.BootstrapLoad();
        turn.StartPlayerTurn();
    }
}