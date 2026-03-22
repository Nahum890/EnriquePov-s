using UnityEngine;
using System.Collections.Generic;

namespace QuizMinigame
{
    [CreateAssetMenu(fileName = "NewQuestionBank", menuName = "Quiz/Question Bank")]
    public class QuestionBankSO : ScriptableObject
    {
        [Header("Detalles del Nivel / Materia")]
        public string subjectName;

        [Header("Lista de Preguntas")]
        public List<QuestionData> questions = new List<QuestionData>();
    }

    [System.Serializable]
    public class QuestionData
    {
        [TextArea(2, 4)]
        public string questionText;

        [Tooltip("Debe tener exactamente 4 opciones.")]
        public string[] options = new string[4]; // Opciones A, B, C, D

        [Tooltip("0 = A, 1 = B, 2 = C, 3 = D")]
        [Range(0, 3)]
        public int correctOptionIndex;

        [TextArea(1, 3)]
        public string hintText; // Para el comodín de "Pista"
    }
}
