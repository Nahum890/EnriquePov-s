using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToHubButton : MonoBehaviour
{
    public void BackToHub()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HUB");
    }
}
