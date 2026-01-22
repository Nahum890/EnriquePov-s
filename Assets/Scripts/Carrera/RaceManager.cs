using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public Transform goal;
    public Transform player;
    public Transform[] racers;

    public TMP_Text positionText;
    public GameObject resultPanel;
    public TMP_Text resultText;

    private bool raceFinished = false;

    void Update()
    {
        if (raceFinished) return;

        int position = 1;

        foreach (Transform racer in racers)
        {
            if (Vector3.Distance(racer.position, goal.position) <
                Vector3.Distance(player.position, goal.position))
            {
                position++;
            }
        }

        positionText.text = "Posición: " + position + "°";
    }

    public void FinishRace(bool playerWon)
    {
        raceFinished = true;
        resultPanel.SetActive(true);

        if (playerWon)
            resultText.text = "¡GANASTE!";
        else
            resultText.text = "PERDISTE";

        Invoke(nameof(ReturnToHub), 3f);
    }

    void ReturnToHub()
    {
        SceneManager.LoadScene("Hub");
    }
}
