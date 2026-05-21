using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Pub")]
public class PubDialogSO : DialogSO
{
    public string title_txt;
    public List<SelcEntry> entries;
    public override void Open(WindowManager window)
    {
        window.OpenPub(title_txt, entries);
    }
}