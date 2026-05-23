using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TurnManager turn;
    public EconomyManager economy;

    public TMP_Text QiText;
    public TMP_Text TurnText;
    public LineAnimator line;
    public TMP_Text ATK;
    public TMP_Text DEF;
    public TMP_Text Gold;
    public List<GameObject> jewels;
    public TMP_Text Skills;
    List<Image> colors;

    private void Start()
    {
        colors = new List<Image>();
        foreach (var obj in jewels)
        {
            colors.Add(obj.GetComponent<Image>());
        }
    }

    public void Refresh()
    {
        PlayerUnit player = MapManager.Instance.Player;
        ATK.text = $"공:{player.Attack}";
        DEF.text = $"방:{player.Defense}";
        Gold.text = $"소지 금화:{economy.Money}";
        for (int i = 0; i < 3; i++)
        {
            string s = i >= player.actionHistory.Count ? null :  player.actionHistory[i];
            switch (s)
            {
                case "Move":
                    //#D4B000
                    colors[i].color = new Color32(212, 176, 0, 255);
                    break;
                case "Z":
                    //#CD2E3A
                    colors[i].color = new Color32(205, 46, 58, 255);
                    break;
                case "X":
                    //#0047A0
                    colors[i].color = new Color32(0, 71, 160, 255);
                    break;
                case "C":
                    colors[i].color = Color.black;
                    break;
                default:
                    colors[i].color = Color.white;
                    break;
            }
        }
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("배운 기술");
        foreach (var passive in player.passives)
        {
            foreach (string s in passive.command_chains)
            {
                sb.Append(map[s.ToLower()]);
            }
            sb.AppendLine(passive.name);
        }
        foreach (var active in player.actives)
        {
            foreach (string s in active.command_chains)
            {
                sb.Append(map[s.ToLower()]);
            }
            sb.AppendLine(active.name);
        }
        Skills.text = sb.ToString();
        QiText.text = "<color=#B8F8FB>Qi:" + player.ActionPoints + "</color>";
        TurnText.text = "<color=#FFD000>Turn:" + turn.turnCount + "</color>";
        var movement = player.path;
        if (movement.Count == 1)
            line.DrawPath(movement);
        else
            line.AddPath(player.last_move);
    }

    Dictionary<string, string> map = new Dictionary<string, string>()
    {
        {"move", "<color=#D4B000>Y</color>"},
        {"z", "<color=#CD2E3A>R</color>"},
        {"x", "<color=#0047A0>B</color>"},
        {"c", "<color=#FFD000>K</color>"},
    };
}