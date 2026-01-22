using UnityEngine;
using TMPro;

public class CountdownManager : MonoBehaviour
{
    public float countdownTime = 3f;
    public TMP_Text countdownText;

    public RaceCharacterController player;
    public RaceCharacterController[] npcs;

    private bool countdownFinished = false;

    void Start()
    {
        // Bloquear movimiento
        player.canMove = false;

        foreach (var npc in npcs)
            npc.canMove = false;
    }

    void Update()
    {
        if (countdownFinished) return;

        countdownTime -= Time.deltaTime;

        if (countdownTime > 0)
        {
            countdownText.text = Mathf.Ceil(countdownTime).ToString();
        }
        else
        {
            countdownText.text = "YA!";
            countdownFinished = true;

            // Activar movimiento
            player.canMove = true;
            foreach (var npc in npcs)
                npc.canMove = true;

            Invoke(nameof(HideCountdown), 1f);
        }
    }

    void HideCountdown()
    {
        countdownText.gameObject.SetActive(false);
    }
}
