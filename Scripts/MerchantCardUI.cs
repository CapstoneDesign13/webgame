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
    public void Setup(DropInstance drop)
    {
        if (drop.id == DropInstance.soldout.id)
        {
            nameTxt.text = "판매 완료";
            typeTxt.text = "";
            priceTxt.text = "";
            descTxt.text = "아무것도 없다.";
            pic.sprite = null;
            btn.onClick.RemoveAllListeners();
        }
        else if (drop.type == PoolType.Active)
        {
            if (parent == null)
                parent = GetComponentInParent<MerchantPanel>();
            ModDatabase.Instance.activePool.TryGetValue(drop.id, out Active active);
            nameTxt.text = active.name;
            typeTxt.text = $"<color=#B8F8FB>{drop.type}</color>";
            priceTxt.text = $"가격:{drop.price}";
            descTxt.text = $"{active.description}";
            Sprite cache = ModDatabase.Instance.GetPic(active.id + "_pic");
            pic.sprite = cache;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => parent.Purchase(drop));
        }
        else if (drop.type == PoolType.Passive)
        {
            if (parent == null)
                parent = GetComponentInParent<MerchantPanel>();
            ModDatabase.Instance.passivePool.TryGetValue(drop.id, out Passive passive);
            nameTxt.text = passive.name;
            typeTxt.text = $"<color=#B8F8FB>{drop.type}</color>";
            priceTxt.text = $"가격:{drop.price}";
            descTxt.text = $"{passive.description}";
            Sprite cache = ModDatabase.Instance.GetPic(passive.id + "_pic");
            pic.sprite = cache;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => parent.Purchase(drop));
        }
    }
}