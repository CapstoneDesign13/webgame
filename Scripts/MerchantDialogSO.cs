using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Merchant")]
public class MerchantDialogSO : DialogSO
{
    public override void Open(WindowManager window)
    {
        window.OpenMerchant();
    }
}