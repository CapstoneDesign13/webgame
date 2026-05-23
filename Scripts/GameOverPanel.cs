using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : FullScreenPanel
{
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}