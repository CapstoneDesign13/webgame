using System.Collections.Generic;
using UnityEngine;

public enum DialogType
{
    None,
    상인,
    객잔,
    영천,
    약방,
    사천당가_만천우침,
    소림사_지진나한,
    무당파_검성,
    하오문_혼선교란자,
}

public abstract class DialogSO : ScriptableObject
{
    public DialogType type;

    public abstract void Open(WindowManager window);
}

public class DialogManager : MonoBehaviour
{
    public WindowManager window;
    public List<DialogSO> dialogs;

    private Dictionary<DialogType, DialogSO> map;

    private void Awake()
    {
        map = new Dictionary<DialogType, DialogSO>();

        foreach (var d in dialogs)
        {
            map[d.type] = d;
        }
    }

    public void Open(DialogType type)
    {
        if (map.TryGetValue(type, out var dialog))
        {
            dialog.Open(window);
        }
        else
        {
            Debug.LogWarning($"Dialog not found: {type}");
        }
    }
}