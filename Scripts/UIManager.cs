using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TurnManager turn;
    public TMP_Text QiText;
    public TMP_Text TurnText;
    private float unit_per_index = 100;

    Vector3 ToVector3(Vector2Int v)
    {
        return new Vector3(v.x * unit_per_index, v.y * unit_per_index, 0f);
    }

    public void Refresh()
    {
        turn.player.transform.position = ToVector3(turn.player.Position);
        foreach (EnemyUnit enemy in turn.enemies)
        {
            enemy.transform.position = ToVector3(enemy.Position);
        }
        QiText.text = "<color=#B8F8FB>Qi" + turn.player.ActionPoints + "</color>";
        TurnText.text = "<color=#FFD000>Turn:" + turn.turnCount + "</color>";
    }
}