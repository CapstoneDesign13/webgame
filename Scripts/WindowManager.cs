using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public GameObject input;
    public GameObject infoPanel;
    public GameObject duelPanel;
    public GameObject merchantPanel;

    private int _active = 0;
    public void Increase()
    {
        _active++;
        if (_active > 0)
        {
            input.SetActive(false);
        }
    }

    public void Decrease()
    {
        _active--;
        if (_active == 0)
        {
            input.SetActive(true);
        }
    }

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

    public void OpenMerchant()
    {
        CloseAll();
        merchantPanel.SetActive(true);
    }
}
