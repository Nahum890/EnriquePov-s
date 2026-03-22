using UnityEngine;
using UnityEngine.UI;
using System;

namespace QuizMinigame
{
    public class TimerController : MonoBehaviour
    {
        [Header("Configuración del Tiempo")]
        public float timePerQuestion = 15f;
        private float currentTime;

        [Header("UI Elementos")]
        public Image timerImage;
        public Text timerText; // Asumiendo UI Text clásico. Para TextMeshPro habría que usar TMP_Text

        [Header("Colores")]
        public Color normalColor = Color.green;
        public Color warningColor = Color.yellow;
        public Color dangerColor = Color.red;

        public event Action OnTimeOut;

        private bool isRunning = false;

        public void StartTimer()
        {
            currentTime = timePerQuestion;
            isRunning = true;
            UpdateUI();
        }

        public void StopTimer()
        {
            isRunning = false;
        }

        public float GetRemainingTime() => currentTime;
        public float GetTotalTime() => timePerQuestion;

        void Update()
        {
            if (!isRunning) return;

            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                isRunning = false;
                UpdateUI();
                OnTimeOut?.Invoke();
            }
            else
            {
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (timerImage != null)
            {
                timerImage.fillAmount = currentTime / timePerQuestion;

                if (currentTime > 10f) timerImage.color = normalColor;
                else if (currentTime > 4f) timerImage.color = warningColor;
                else timerImage.color = dangerColor;
            }

            if (timerText != null)
                timerText.text = Mathf.CeilToInt(currentTime).ToString();
        }
    }
}
