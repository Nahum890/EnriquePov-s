using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Colocá este script en un GameObject con un Collider (trigger) en el HUB.
/// Cuando el jugador (tag "Player") se acerca, aparece un texto "Presiona E".
/// Al presionar E se carga la escena del Quiz.
/// </summary>
public class QuizEntrance : MonoBehaviour
{
    [Header("Escena a Cargar")]
    public string quizSceneName = "Quiz";

    [Header("UI de Interacción (opcional)")]
    [Tooltip("Un Text/UI que diga 'Presiona E para entrar al Quiz'. Se activa cuando el jugador está cerca.")]
    public GameObject interactionPrompt;

    private bool playerInRange = false;

    void Start()
    {
        if (interactionPrompt)
            interactionPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(quizSceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionPrompt)
                interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionPrompt)
                interactionPrompt.SetActive(false);
        }
    }
}
