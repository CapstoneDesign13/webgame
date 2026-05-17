using System;
using UnityEngine;

public class MerchantUnit : CharacterBase
{
    public Action dialog;
    public override bool Answer()
    {
        if (dialog == null)
        {
            Debug.LogWarning("말을 걸었지만 할 말이 없어 보인다!");
            return false;
        }
        dialog();
        return true;
    }
}