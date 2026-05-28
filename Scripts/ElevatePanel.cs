using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElevatePanel : MonoBehaviour
{
    public StageManager stage;
    public TMP_Text txt;
    public Image pic;
    public SpriteRenderer render;

    private void OnEnable()
    {
        txt.text = $"스테이지{stage.stage}입성";
        Sprite cache = ModDatabase.Instance.GetPic($"stage{stage.stage}_enter");
        pic.sprite = cache;
    }
}