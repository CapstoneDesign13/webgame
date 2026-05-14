using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class infoCardUI : MonoBehaviour
{
    public TMP_Text teamTxt;
    public TMP_Text hpTxt;
    public TMP_Text atkTxt;
    public TMP_Text defTxt;
    public TMP_Text posTxt;
    public Image pic;
    public void setup(CharacterBase unit)
    {
        teamTxt.text = unit.team.ToString();
        hpTxt.text = $"체력:{unit.HP}/{unit.MaxHP}";
        atkTxt.text = $"공격력:{ unit.Attack}";
        defTxt.text = $"방어력:{unit.Defense}";
        posTxt.text = $"위치:{unit.CurrentGridPosition}";
        Sprite cache = ModDatabase.Instance.GetPic(unit.name + "S");
        pic.sprite = cache;
    }
}
