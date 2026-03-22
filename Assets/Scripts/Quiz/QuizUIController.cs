using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace QuizMinigame
{
    public class QuizUIController : MonoBehaviour
    {
        [Header("Elementos de Pregunta")]
        public Text questionText;
        public Button[] optionButtons;
        public Text[] optionTexts;

        [Header("Puntuajes")]
        public Text currentScoreText;
        public Text globalScoreText;
        public Text progressText;

        [Header("Paneles / Modales")]
        public GameObject resultPanel;
        public Text resultText;
        public GameObject hintPanel;
        public Text hintText;

        [Header("Colores de Feedback")]
        public Color normalColor = Color.white;
        public Color selectedColor = new Color(1f, 0.5f, 0f); // Naranja
        public Color correctColor = Color.green;
        public Color wrongColor = Color.red;

        public void SetupQuestion(QuestionData qData)
        {
            if (questionText) questionText.text = qData.questionText;

            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (i < qData.options.Length)
                {
                    optionButtons[i].gameObject.SetActive(true);
                    optionButtons[i].interactable = true;
                    if (optionTexts[i]) optionTexts[i].text = qData.options[i];
                    optionButtons[i].image.color = normalColor;
                }
                else
                {
                    optionButtons[i].gameObject.SetActive(false);
                }
            }
        }

        public void HighlightSelected(int index)
        {
            if (index >= 0 && index < optionButtons.Length)
                optionButtons[index].image.color = selectedColor;
        }

        public void ShowCorrectAnswer(int correctIndex, int selectedIndex)
        {
            if (correctIndex >= 0 && correctIndex < optionButtons.Length)
                optionButtons[correctIndex].image.color = correctColor;

            if (selectedIndex != correctIndex && selectedIndex >= 0 && selectedIndex < optionButtons.Length)
                optionButtons[selectedIndex].image.color = wrongColor;
        }

        public void UpdateScores(int currentQuizScore, int globalScore, int currentQ, int totalQ)
        {
            if (currentScoreText) currentScoreText.text = $"Puntos Quiz: {currentQuizScore}";
            if (globalScoreText) globalScoreText.text = $"Global: {globalScore}";
            if (progressText) progressText.text = $"Preguntas: {currentQ}/{totalQ}";
        }

        public void SetupFiftyFifty(List<int> indicesToHide)
        {
            foreach (int index in indicesToHide)
            {
                if (index >= 0 && index < optionButtons.Length)
                {
                    optionButtons[index].interactable = false;
                    if (optionTexts[index]) optionTexts[index].text = ""; // Borrar el texto
                }
            }
        }

        public void ShowHintModal(string hint)
        {
            if (hintPanel && hintText)
            {
                hintText.text = hint;
                hintPanel.SetActive(true);
            }
        }

        public void ShowFinalResult(bool won, int finalScore)
        {
            if (resultPanel && resultText)
            {
                resultPanel.SetActive(true);
                resultText.text = won ? $"¡Aprobaste!\nPuntos Finales: {finalScore}" 
                                      : $"¡Fallaste!\nPuntos Finales: {finalScore}";
            }
        }

        public void SetButtonsInteractable(bool state)
        {
            foreach (var btn in optionButtons)
            {
                // Solo activamos si estaba visible y no había sido escondido por 50/50
                if (btn.gameObject.activeSelf && btn.GetComponentInChildren<Text>().text != "")
                    btn.interactable = state;
            }
        }
    }
}
