using System.Collections.Generic;

public class PubDialog : Dialog, IHasID
{
    public string id;
    string IHasID.id => id;
    public string title_txt;
    public string content_txt;
    public List<SelcEntry> entries;
    public override void Open(WindowManager window)
    {
        window.OpenPub(title_txt, content_txt, entries);
    }
}