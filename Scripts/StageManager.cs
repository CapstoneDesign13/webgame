using UnityEngine;

public class StageManager : MonoBehaviour
{
    public RunManager run;
    public WindowManager window;

    public RoomType type;
    public int stage = 1;

    public void Proxy(int type)
    {
        run.CleanUp(
        new RoomDefinition()
        {
            type = (RoomType)type,
            stageKey = $"stage{stage}"
        });
    }

    public void OpenOracle()
    {
        window.OpenOracle();
    }
}