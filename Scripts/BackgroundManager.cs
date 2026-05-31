using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public string picid = "stage1";
    public SpriteRenderer sprite;
    public AudioSource audioSource;
    public AudioClip stage5Clip;
    public AudioClip BossBattle2Clip;

    public void SetPic(string newid)
    {
        if (picid != newid)
        {
            picid = newid;
            Sprite cache = ModDatabase.Instance.GetPic(newid);
            sprite.sprite = cache;

            if (newid == "BossBattle2")
            {
                audioSource.Stop();
                audioSource.clip = BossBattle2Clip;
                audioSource.Play();
            }
            else if (newid == "stage5")
            {
                audioSource.Stop();
                audioSource.clip = stage5Clip;
                audioSource.Play();
            }
        }
    }
}
