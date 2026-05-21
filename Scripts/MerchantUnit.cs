using System;
using UnityEngine;

public class MerchantUnit : CharacterBase
{
    public DialogType? type;
    public Action<DialogType> dialog;
    public override bool Answer()
    {
        if (dialog == null || type == null)
        {
            Debug.LogWarning("말을 걸었지만 할 말이 없어 보인다!");
            return false;
        }
        dialog((DialogType)type);
        return true;
    }

    public void SetDialogHandler(DialogManager dm)
    {
        dialog = dm != null ? dm.Open : null;
    }
}