using UnityEngine;

public class StageManager : MonoBehaviour
{
    public RoomBootstrap room;
    public WindowManager window;
    public TurnManager turn;

    public RoomType type;
    public int stage = 1;

    public void Proxy(int type)
    {
        if (window != null)
        {
            window.CloseAll();
        }

        MapManager map = MapManager.Instance;

        if (map != null)
        {
            map.ClearRoomUnitsExceptPlayer();
            map.RepositionPlayer(new Vector2Int(4, 0));
        }

        if (turn != null)
        {
            turn.ResetForNewRoom();
        }

        room.LoadRoom(new RoomDefinition()
        {
            type = (RoomType)type,
            stageKey = $"stage{stage}"
        });

        if (turn != null)
        {
            turn.StartPlayerTurn();
        }
    }

    public void OpenOracle()
    {
        window.OpenOracle();
    }
}