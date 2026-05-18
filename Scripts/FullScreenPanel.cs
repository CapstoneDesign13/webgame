using UnityEngine;

public abstract class FullScreenPanel : MonoBehaviour
{
    public WindowManager window;

    private void OnEnable()
    {
        if (window != null)
            window.Increase();
        Refresh();
    }

    private void OnDisable()
    {
        if (window != null)
            window.Decrease();
    }

    public virtual void Refresh()
    {

    }
}
