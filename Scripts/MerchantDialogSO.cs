using UnityEngine;

public class MerchantDialog : Dialog
{
    public override void Open(WindowManager window)
    {
        window.OpenMerchant();
    }
}