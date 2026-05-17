using UnityEngine;

public abstract class FullScreenPanel : MonoBehaviour
{
    public WindowManager window;

    private void OnEnable()
    {
        window.Increase();
        Refresh();
    }

    private void OnDisable()
    {
        window.Decrease();
    }

    public virtual void Refresh()
    {

    }
}
