using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using UnityEngine;

[JsonConverter(typeof(StringEnumConverter))]
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

public abstract class Dialog
{
    public DialogType type;

    public abstract void Open(WindowManager window);
}

public class DialogManager : MonoBehaviour
{
    public WindowManager window;
    public List<Dialog> dialogs = new List<Dialog>()
    {
        new MerchantDialog()
        {
            type = DialogType.상인
        }
    };

    private Dictionary<DialogType, Dialog> map = new Dictionary<DialogType, Dialog>();

    public void StartMap()
    {
        foreach (var d in dialogs)
        {
            map[d.type] = d;
        }
        foreach (var d in ModDatabase.Instance.dialogDB.Values)
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