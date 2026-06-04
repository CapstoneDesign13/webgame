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
        int flag = 1 << UnityEngine.Random.Range(0, 4);
        Proxy(flag);
    }

    public void Proxy(int type)
    {
        if (type == (int)Option.¾à¹æ)
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
        Debug.Log($"exp:{exp}");
        if (stage < 5 && exp >= 10)
        {
            stage++;
            exp = 0;
            pharmacyhappened = false;
            window.OpenElevate();
        }
        window.OpenOracle();
    }
}