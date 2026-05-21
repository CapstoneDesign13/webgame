using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceCardUI : MonoBehaviour
{
    public TMP_Text content;
    public TMP_Text desc;
    public Button btn;

    public ChoicePanel parent;

    public void Setup(string s1, string s2, List<(Action<int> r, int t)> actions)
    {
        content.text = s1;
        desc.text = s2;
        btn.onClick.RemoveAllListeners();
        foreach (var (r, t) in actions)
            btn.onClick.AddListener(() => r(t));
        if (parent == null)
            parent = GetComponentInParent<ChoicePanel>();
        if (parent == null)
            Debug.LogError("No ChoicePanel");
        if (parent != null)
            btn.onClick.AddListener(() => parent.NextStage());
    }
}
