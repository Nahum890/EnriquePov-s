using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace QuizMinigame
{
    public class QuizGameManager : MonoBehaviour
    {
        [Header("Referencias")]
        public QuestionBankSO currentBank;
        public QuizUIController uiController;
        public TimerController timerController;
        public JokerManager jokerManager;

        [Header("Configuración del Minijuego")]
        public int questionsToWin = 5;
        public int allowedLives = 3;
        public int pointsPerCorrect = 1000;
        public int maxTimeBonus = 500;
        public int penaltyPerWrong = 500;

        private int currentQuestionIndex = 0;
        private int currentLives;
        private int quizLocalScore = 0;
        private bool isAnswering = false;

        private List<QuestionData> activeQuestions = new List<QuestionData>();

        void Start()
        {
            if (currentBank == null || currentBank.questions.Count == 0)
            {
                Debug.LogError("No hay banco de preguntas asignado en QuizGameManager.");
                return;
            }

            // Mezclar preguntas para la partida
            activeQuestions = new List<QuestionData>(currentBank.questions);
            ShuffleList(activeQuestions);

            currentLives = allowedLives;
            
            // Suscribirse a eventos
            if (timerController) timerController.OnTimeOut += HandleTimeOut;
            if (jokerManager)
            {
                jokerManager.OnFiftyFiftyUsed += uiController.SetupFiftyFifty;
                jokerManager.OnHintUsed += uiController.ShowHintModal;

                // Conectar botones de comodines
                if (jokerManager.fiftyFiftyButton)
                    jokerManager.fiftyFiftyButton.onClick.AddListener(() =>
                    {
                        if (currentQuestionIndex < activeQuestions.Count)
                            jokerManager.UseFiftyFifty(activeQuestions[currentQuestionIndex]);
                    });

                if (jokerManager.hintButton)
                    jokerManager.hintButton.onClick.AddListener(() =>
                    {
                        if (currentQuestionIndex < activeQuestions.Count)
                            jokerManager.UseHint(activeQuestions[currentQuestionIndex]);
                    });

                if (jokerManager.publicOpinionButton)
                    jokerManager.publicOpinionButton.onClick.AddListener(() =>
                    {
                        jokerManager.UsePublicOpinion();
                    });
            }

            StartCoroutine(InitQuizCoroutine());
        }

        private IEnumerator InitQuizCoroutine()
        {
            yield return new WaitForSeconds(1f); // Pequeña espera visual inicial
            UpdateUIScores();
            LoadNextQuestion();
        }

        private void OnDestroy()
        {
            if (timerController) timerController.OnTimeOut -= HandleTimeOut;
            if (jokerManager)
            {
                jokerManager.OnFiftyFiftyUsed -= uiController.SetupFiftyFifty;
                jokerManager.OnHintUsed -= uiController.ShowHintModal;
            }
        }

        public void ForceActionCloseHint() // Llamar desde botón cerrar pista
        {
            if (uiController.hintPanel) uiController.hintPanel.SetActive(false);
        }

        void LoadNextQuestion()
        {
            if (currentQuestionIndex >= questionsToWin || currentQuestionIndex >= activeQuestions.Count)
            {
                StartCoroutine(EndGameRoutine(true));
                return;
            }

            if (currentLives <= 0)
            {
                StartCoroutine(EndGameRoutine(false));
                return;
            }

            if (jokerManager) jokerManager.ResetRoundState();

            QuestionData qData = activeQuestions[currentQuestionIndex];
            uiController.SetupQuestion(qData);
            UpdateUIScores();

            isAnswering = true;
            if (timerController) timerController.StartTimer();
        }

        public void SubmitAnswer(int selectedIndex)
        {
            if (!isAnswering) return;
            isAnswering = false;

            if (timerController) timerController.StopTimer();
            uiController.HighlightSelected(selectedIndex);
            uiController.SetButtonsInteractable(false);

            StartCoroutine(ResolveAnswerRoutine(selectedIndex));
        }

        private void HandleTimeOut()
        {
            if (!isAnswering) return;
            isAnswering = false;
            uiController.SetButtonsInteractable(false);
            
            StartCoroutine(ResolveAnswerRoutine(-1)); // -1 para indicar que no eligió nada
        }

        private IEnumerator ResolveAnswerRoutine(int selectedIndex)
        {
            yield return new WaitForSeconds(1.5f); // Tiempo de suspenso tipo "100 argentinos dicen"

            QuestionData qData = activeQuestions[currentQuestionIndex];
            bool isCorrect = (selectedIndex == qData.correctOptionIndex);

            uiController.ShowCorrectAnswer(qData.correctOptionIndex, selectedIndex);

            if (isCorrect)
            {
                int pointsEarned = pointsPerCorrect;

                // Bonus por tiempo
                if (timerController)
                {
                    float ratio = timerController.GetRemainingTime() / timerController.GetTotalTime();
                    pointsEarned += Mathf.FloorToInt(maxTimeBonus * ratio);
                }

                // Penalización si usó comodines
                if (jokerManager != null && jokerManager.IsFiftyFiftyActiveThisRound)
                    pointsEarned /= 2;

                if (jokerManager != null && jokerManager.IsHintActiveThisRound)
                    pointsEarned -= 200;

                quizLocalScore += Mathf.Max(0, pointsEarned);
            }
            else
            {
                quizLocalScore -= penaltyPerWrong;
                currentLives--;
            }

            UpdateUIScores();
            currentQuestionIndex++;

            yield return new WaitForSeconds(2.5f); // Tiempo par ver el resultado (verde/rojo)

            LoadNextQuestion();
        }

        private void UpdateUIScores()
        {
            int globalScore = ScoreManager.Instance != null ? ScoreManager.Instance.totalPoints : 0;
            uiController.UpdateScores(quizLocalScore, globalScore, currentQuestionIndex, questionsToWin);
        }

        private IEnumerator EndGameRoutine(bool won)
        {
            uiController.ShowFinalResult(won, quizLocalScore);
            
            yield return new WaitForSeconds(3f);

            if (won)
            {
                if (ScoreManager.Instance != null)
                {
                    // Sumamos puntaje al global asegurando no restar
                    ScoreManager.Instance.AddPoints(Mathf.Max(0, quizLocalScore)); 
                    ScoreManager.Instance.quizzesCompleted++;
                }
            }

            // Aquí renegocia la escena de retorno
            SceneManager.LoadScene("HUB"); 
        }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = UnityEngine.Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}
