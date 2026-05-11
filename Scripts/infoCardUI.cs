using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class infoCardUI : MonoBehaviour
{
    public TMP_Text teamTxt;
    public TMP_Text hpTxt;
    public TMP_Text atkTxt;
    public TMP_Text defTxt;
    public TMP_Text posTxt;
    public void setup(CharacterBase unit)
    {
        teamTxt.text = unit.team.ToString();
        hpTxt.text = string.Join("/", unit.HP, unit.MaxHP);
        atkTxt.text = unit.Attack.ToString();
        defTxt.text = unit.Defense.ToString();
        posTxt.text = string.Join(",", unit.CurrentGridPosition);
    }
}
