using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public string picid = "stage1";
    public SpriteRenderer sprite;

    public void SetPic(string newid)
    {
        if (picid != newid)
        {
            Sprite cache = ModDatabase.Instance.GetPic(newid);
            sprite.sprite = cache;
        }
    }
}
