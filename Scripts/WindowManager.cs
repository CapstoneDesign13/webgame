using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject duelPanel;

    public void CloseAll()
    {
        duelPanel.SetActive(false);
    }

    public void OpenField()
    {
        CloseAll();
    }

    public void CloseInfo()
    {
        infoPanel.SetActive(false);
    }

    public void OpenInfo()
    {
        infoPanel.SetActive(true);
    }

    public void OpenDuel()
    {
        CloseAll();
        duelPanel.SetActive(true);
    }
}
