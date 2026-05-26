using UnityEngine;

public class StageManager : MonoBehaviour
{
    public RunManager run;
    public WindowManager window;

    public RoomType type;
    public int stage = 1;

    public bool pharmacyhappened;

    int exp = 0;

    public void ProxyRand()
    {
        Option flag = (Option)UnityEngine.Random.Range(1, (int)Option.약방);
        Proxy((int)flag);
    }

    public void Proxy(int type)
    {
        if (type == (int)Option.약방)
            pharmacyhappened = true;
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
            pharmacyhappened = false;
            window.OpenElevate();
        }
        window.OpenOracle();
    }
}