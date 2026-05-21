using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class SelcEntry
{
    public string content;
    public string desc;
}

public class ChoicePanel : FullScreenPanel
{
    public TMP_Text title;
    public List<SelcEntry> choices;

    public void Setup(string title_txt, List<SelcEntry> entries)
    {
        title.text = title_txt;
        choices = entries;
    }
}