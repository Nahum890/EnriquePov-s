using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public RaceCharacterController player;
    public RaceCharacterController[] npcs;

    void Start()
    {
        tutorialPanel.SetActive(true);

        player.canMove = false;
        foreach (var npc in npcs)
            npc.canMove = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            tutorialPanel.SetActive(false);

            player.canMove = true;
            foreach (var npc in npcs)
                npc.canMove = true;

            Destroy(this);
        }
    }
}
