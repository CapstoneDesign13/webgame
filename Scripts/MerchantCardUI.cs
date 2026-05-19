using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantCardUI : MonoBehaviour
{
    private MerchantPanel parent;
    public TMP_Text nameTxt;
    public TMP_Text typeTxt;
    public TMP_Text priceTxt;
    public TMP_Text descTxt;
    public Button btn;
    public Image pic;
    public void Setup((Combo, PoolType) pair)
    {
        if (pair == Soldout.pair)
        {
            nameTxt.text = "판매 완료";
            typeTxt.text = "";
            priceTxt.text = "";
            descTxt.text = "아무것도 없다.";
            pic.sprite = null;
            btn.onClick.RemoveAllListeners();
        }
        else
        {
            if (parent == null)
                parent = GetComponentInParent<MerchantPanel>();
            nameTxt.text = pair.Item1.name;
            typeTxt.text = $"<color=#B8F8FB>{pair.Item2}</color>";
            priceTxt.text = $"가격:---";
            descTxt.text = $"{pair.Item1.description}";
            Sprite cache = ModDatabase.Instance.GetPic(pair.Item1.id + "_pic");
            pic.sprite = cache;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => MapManager.Instance.Player.Learn(pair));
            btn.onClick.AddListener(() => parent.Purchase(pair));
        }
    }
}