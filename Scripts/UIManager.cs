using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TurnManager turn;
    public TMP_Text QiText;
    public TMP_Text TurnText;
    public LineAnimator line;
    public List<GameObject> jewels;
    List<SpriteRenderer> colors;

    private void Start()
    {
        colors = new List<SpriteRenderer>();
        foreach (var obj in jewels)
        {
            colors.Add(obj.GetComponent<SpriteRenderer>());
        }
    }

    public void Refresh()
    {
        PlayerUnit player = MapManager.Instance.Player;
        for (int i = 0; i < 3; i++)
        {
            string s = i >= player.actionHistory.Count ? null :  player.actionHistory[i];
            switch (s)
            {
                case "Move":
                    //#D4B000
                    colors[i].color = new Color32(212, 176, 0, 255);
                    break;
                case "Attack":
                    //#CD2E3A
                    colors[i].color = new Color32(205, 46, 58, 255);
                    break;
                case "Defense":
                    //#0047A0
                    colors[i].color = new Color32(0, 71, 160, 255);
                    break;
                default:
                    colors[i].color = Color.white;
                    break;
            }
        }
        QiText.text = "<color=#B8F8FB>Qi" + player.ActionPoints + "</color>";
        TurnText.text = "<color=#FFD000>Turn:" + turn.turnCount + "</color>";
        line.DrawPath(player.path);
    }
}