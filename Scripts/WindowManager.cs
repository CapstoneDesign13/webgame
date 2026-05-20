using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public GameObject input;
    public GameObject infoPanel;
    public GameObject duelPanel;
    public GameObject merchantPanel;
    public GameObject stageClearPanel;

    private int _active = 0;

    public void Increase()
    {
        _active++;

        if (_active > 0)
        {
            if (input != null)
            {
                input.SetActive(false);
            }
        }
    }

    public void Decrease()
    {
        _active--;

        if (_active <= 0)
        {
            _active = 0;

            if (input != null)
            {
                input.SetActive(true);
            }
        }
    }

    public void CloseAll()
    {
        if (duelPanel != null)
        {
            duelPanel.SetActive(false);
        }

        if (merchantPanel != null)
        {
            merchantPanel.SetActive(false);
        }

        if (stageClearPanel != null)
        {
            stageClearPanel.SetActive(false);
        }
    }

    public void OpenField()
    {
        CloseAll();
    }

    public void CloseInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    public void OpenInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
        }
    }

    public void OpenDuel()
    {
        CloseAll();

        if (duelPanel != null)
        {
            duelPanel.SetActive(true);
        }
    }

    public void OpenMerchant()
    {
        CloseAll();

        if (merchantPanel != null)
        {
            merchantPanel.SetActive(true);
        }
    }

    public void OpenStageClear()
    {
        CloseAll();

        if (input != null)
        {
            input.SetActive(false);
        }

        if (stageClearPanel != null)
        {
            stageClearPanel.SetActive(true);
        }
    }
}