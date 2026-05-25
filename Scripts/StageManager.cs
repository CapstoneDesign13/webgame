using UnityEngine;

public class StageManager : MonoBehaviour
{
    public RunManager run;
    public WindowManager window;

    public RoomType type;
    public int stage = 1;

    int exp = 0;

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
        exp++;
        if (exp >= 10)
        {
            stage++;
            exp = 0;
            window.OpenElevate();
        }
        window.OpenOracle();
    }
}