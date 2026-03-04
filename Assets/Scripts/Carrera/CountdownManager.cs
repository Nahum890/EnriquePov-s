using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText;

    public RaceCharacterController player;
    public NPCAI_Rigidbody[] npcs;

    public float countdownTime = 3f;

    void Start()
    {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        player.canMove = false;
        foreach (var npc in npcs)
            npc.canMove = false;

        float t = countdownTime;

        while (t > 0)
        {
            countdownText.text = Mathf.Ceil(t).ToString();
            yield return new WaitForSeconds(1f);
            t--;
        }

        countdownText.text = "GO!";
        player.canMove = true;

        foreach (var npc in npcs)
            npc.canMove = true;

        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
    }
}
