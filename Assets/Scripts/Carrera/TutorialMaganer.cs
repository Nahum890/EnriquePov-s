using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public RaceCharacterController player;
    public NPCAI_Rigidbody[] npcs;

    void Start()
    {
        tutorialPanel.SetActive(true);
        player.canMove = false;

        foreach (var npc in npcs)
            npc.canMove = false;
    }

    public void StartRace()
    {
        tutorialPanel.SetActive(false);
        player.canMove = true;

        foreach (var npc in npcs)
            npc.canMove = true;
    }
}
