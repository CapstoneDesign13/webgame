using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public InputHandler input;
    public UIManager ui;
    public InfoPanelUI infoPanel;
    public DuelPanel duelPanel;
    public MerchantPanel merchantPanel;
    public GameObject stageClearPanel;
    public ChoicePanel choicePanel;

    private int _active = 0;

    public void Increase()
    {
        _active++;

        if (_active > 0)
        {
            if (input != null)
            {
                input.gameObject.SetActive(false);
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
                input.gameObject.SetActive(true);
                ui.Refresh();
            }
        }
    }

    public void CloseAll()
    {
        if (duelPanel != null)
        {
            duelPanel.gameObject.SetActive(false);
        }

        if (merchantPanel != null)
        {
            merchantPanel.gameObject.SetActive(false);
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
            infoPanel.gameObject.SetActive(false);
        }
    }

    public void OpenInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.gameObject.SetActive(true);
        }
    }

    public void OpenDuel()
    {
        CloseAll();

        if (duelPanel != null)
        {
            duelPanel.gameObject.SetActive(true);
        }
    }

    public void OpenMerchant()
    {
        CloseAll();

        if (merchantPanel != null)
        {
            merchantPanel.gameObject.SetActive(true);
        }
    }

    public void OpenPub(string title_txt, List<SelcEntry> entries)
    {
        CloseAll();

        choicePanel.Setup(title_txt, entries);
        if (choicePanel != null)
        {
            choicePanel.gameObject.SetActive(true);
        }
    }

    public void OpenStageClear()
    {
        CloseAll();

        if (input != null)
        {
            input.gameObject.SetActive(false);
        }

        if (stageClearPanel != null)
        {
            stageClearPanel.SetActive(true);
        }
    }
}