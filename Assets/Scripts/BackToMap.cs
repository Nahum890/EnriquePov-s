using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMap : MonoBehaviour
{
    public string mapSceneName = "Mapa";

    public void GoBack()
    {
        SceneManager.LoadScene(mapSceneName);
    }
}
