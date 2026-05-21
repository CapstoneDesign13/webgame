using UnityEngine;

public class InitHandler : MonoBehaviour
{
    public ResourceLoader resourceLoader;
    public SpawnManager spawn;
    public RoomBootstrap room;
    public TurnManager turn;
    public PlayerUnit player;
    public RunManager run;

    public PlayerSetting playerSetting;
    [SerializeField] private readonly Vector2Int initpos = new Vector2Int(4, 0);

    void Start()
    {
        // Create shared database
        ModDatabase sharedDatabase = ScriptableObject.CreateInstance<ModDatabase>();
        sharedDatabase.Initialize();

        resourceLoader.database = sharedDatabase;
        resourceLoader.LoadMods();

        player.SetPlayer(playerSetting);
        turn.duelturn = player.duelturn;
        MapManager.Instance.PlaceUnit(player, initpos);

        if (player.team != Team.Ally)
        {
            Debug.LogError("주인공 팀이 이상합니다.");
        }

        if (run == null)
        {
            Debug.LogError("InitHandler: RunManager가 연결되어 있지 않습니다.");
            return;
        }

        run.StartRun();
    }
}