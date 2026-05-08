using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelUI : MonoBehaviour
{
    public TurnManager turn;
    public GameObject infoCard;
    private void OnEnable()
    {
        foreach (Transform child in this.transform)
            Destroy(child.gameObject);
        GameObject obj = Instantiate(infoCard, this.transform);
        infoCardUI card = obj.GetComponent<infoCardUI>();
        card.setup(turn.player);
        foreach (EnemyUnit enemy in turn.enemies)
        {
            if (enemy.IsDead)
                continue;
            obj = Instantiate(infoCard, this.transform);
            card = obj.GetComponent<infoCardUI>();
            card.setup(enemy);
        }
    }
}
