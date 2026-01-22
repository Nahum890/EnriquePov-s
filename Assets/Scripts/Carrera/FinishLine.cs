using UnityEngine;
using TMPro;

public class FinishLine : MonoBehaviour
{
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    private int runnersFinished = 0;
    private bool raceFinished = false;

    private void Start()
    {
        resultPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (raceFinished)
            return;

        runnersFinished++;

        if (other.CompareTag("Player"))
        {
            raceFinished = true;
            ShowResult(runnersFinished);
        }
    }

    void ShowResult(int position)
    {
        resultPanel.SetActive(true);

        if (position == 1)
        {
            resultText.text = "¡GANASTE!";
        }
        else
        {
            resultText.text = "PERDISTE\nPosición: " + position;
        }

        Time.timeScale = 0f;
    }
}
