using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public GameObject fieldPanel;
    public GameObject infoPanel;

    public void CloseAll()
    {
        fieldPanel.SetActive(false);
        infoPanel.SetActive(false);
    }

    public void OpenField()
    {
        CloseAll();
        fieldPanel.SetActive(true);
    }

    public void OpenInfo()
    {
        CloseAll();
        infoPanel.SetActive(true);
    }
}
