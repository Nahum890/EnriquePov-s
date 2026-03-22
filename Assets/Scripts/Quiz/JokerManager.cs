using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace QuizMinigame
{
    public class JokerManager : MonoBehaviour
    {
        [Header("Botones UI")]
        public Button fiftyFiftyButton;
        public Button hintButton;
        public Button publicOpinionButton;

        // Eventos
        public event Action<List<int>> OnFiftyFiftyUsed; // Envía índices de respuestas a ocultar
        public event Action<string> OnHintUsed; // Envía el texto de la pista
        public event Action OnPublicOpinionUsed;

        private bool fiftyFiftyAvailable = true;
        private bool hintAvailable = true;
        private bool publicOpinionAvailable = true;

        public bool IsFiftyFiftyActiveThisRound { get; private set; } = false;
        public bool IsHintActiveThisRound { get; private set; } = false;

        public void ResetRoundState()
        {
            IsFiftyFiftyActiveThisRound = false;
            IsHintActiveThisRound = false;
        }

        public void UseFiftyFifty(QuestionData currentQuestion)
        {
            if (!fiftyFiftyAvailable) return;
            fiftyFiftyAvailable = false;
            IsFiftyFiftyActiveThisRound = true;

            if (fiftyFiftyButton) fiftyFiftyButton.interactable = false;

            // Elegir 2 opciones incorrectas al azar
            List<int> incorrectIndices = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                if (i != currentQuestion.correctOptionIndex)
                {
                    incorrectIndices.Add(i);
                }
            }

            // Shuffle basic
            for (int i = 0; i < incorrectIndices.Count; i++)
            {
                int temp = incorrectIndices[i];
                int randomIndex = UnityEngine.Random.Range(i, incorrectIndices.Count);
                incorrectIndices[i] = incorrectIndices[randomIndex];
                incorrectIndices[randomIndex] = temp;
            }

            List<int> indicesToHide = new List<int> { incorrectIndices[0], incorrectIndices[1] };
            OnFiftyFiftyUsed?.Invoke(indicesToHide);
        }

        public void UseHint(QuestionData currentQuestion)
        {
            if (!hintAvailable) return;
            hintAvailable = false;
            IsHintActiveThisRound = true;

            if (hintButton) hintButton.interactable = false;

            OnHintUsed?.Invoke(currentQuestion.hintText);
        }

        public void UsePublicOpinion()
        {
            if (!publicOpinionAvailable) return;
            publicOpinionAvailable = false;

            if (publicOpinionButton) publicOpinionButton.interactable = false;

            OnPublicOpinionUsed?.Invoke();
        }
    }
}
