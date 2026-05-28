using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : FullScreenPanel
{
    public string picid = "ÆÐ¹è";
    public Image pic;
    public override void Refresh()
    {
        Sprite cache = ModDatabase.Instance.GetPic(picid);
        pic.sprite = cache;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}