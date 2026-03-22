#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using QuizMinigame;

/// <summary>
/// Script de Editor que genera TODA la escena del Quiz automáticamente.
/// Menú: Tools -> Quiz -> Crear Escena Quiz
/// Menú: Tools -> Quiz -> Crear Banco de Preguntas de Ejemplo
/// </summary>
public class QuizSceneSetup : Editor
{
    // =====================================================
    // CREAR ESCENA COMPLETA
    // =====================================================
    [MenuItem("Tools/Quiz/Crear Escena Quiz")]
    public static void CreateQuizScene()
    {
        // ==========================================
        // 1) CREAR ESCENA NUEVA
        // ==========================================
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // ==========================================
        // 2) MATERIALES PARA EL AULA
        // ==========================================
        // Asegurarnos de que la carpeta existe
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Materials/Quiz"))
            AssetDatabase.CreateFolder("Assets/Materials", "Quiz");

        Material floorMat = CreateOrLoadMaterial("Assets/Materials/Quiz/Piso_Aula.mat",
            new Color(0.45f, 0.35f, 0.25f)); // Marrón madera
        Material wallMat = CreateOrLoadMaterial("Assets/Materials/Quiz/Pared_Aula.mat",
            new Color(0.85f, 0.82f, 0.72f)); // Beige claro
        Material boardMat = CreateOrLoadMaterial("Assets/Materials/Quiz/Pizarra.mat",
            new Color(0.12f, 0.22f, 0.15f)); // Verde pizarrón
        Material deskMat = CreateOrLoadMaterial("Assets/Materials/Quiz/Pupitre.mat",
            new Color(0.55f, 0.40f, 0.25f)); // Madera oscura
        Material chairMat = CreateOrLoadMaterial("Assets/Materials/Quiz/Silla.mat",
            new Color(0.25f, 0.35f, 0.55f)); // Azul escolar
        Material legMat = CreateOrLoadMaterial("Assets/Materials/Quiz/Metal_Pata.mat",
            new Color(0.5f, 0.5f, 0.5f)); // Gris metálico

        // ==========================================
        // 3) CONSTRUIR AULA 3D
        // ==========================================
        // Piso
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Piso_Aula";
        floor.transform.position = Vector3.zero;
        floor.transform.localScale = new Vector3(3f, 1f, 3f);
        floor.GetComponent<Renderer>().sharedMaterial = floorMat;

        // Pared trasera (donde va la pizarra)
        GameObject backWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backWall.name = "Pared_Trasera";
        backWall.transform.position = new Vector3(0f, 2.5f, 14f);
        backWall.transform.localScale = new Vector3(30f, 5f, 0.3f);
        backWall.GetComponent<Renderer>().sharedMaterial = wallMat;

        // Pizarrón (sobre la pared trasera)
        GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.name = "Pizarron";
        board.transform.position = new Vector3(0f, 2.8f, 13.8f);
        board.transform.localScale = new Vector3(8f, 3f, 0.1f);
        board.GetComponent<Renderer>().sharedMaterial = boardMat;

        // Paredes laterales
        GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftWall.name = "Pared_Izquierda";
        leftWall.transform.position = new Vector3(-15f, 2.5f, 7f);
        leftWall.transform.localScale = new Vector3(0.3f, 5f, 14f);
        leftWall.GetComponent<Renderer>().sharedMaterial = wallMat;

        GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightWall.name = "Pared_Derecha";
        rightWall.transform.position = new Vector3(15f, 2.5f, 7f);
        rightWall.transform.localScale = new Vector3(0.3f, 5f, 14f);
        rightWall.GetComponent<Renderer>().sharedMaterial = wallMat;

        // Pared de fondo (detrás de la cámara)
        GameObject frontWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frontWall.name = "Pared_Frontal";
        frontWall.transform.position = new Vector3(0f, 2.5f, -1f);
        frontWall.transform.localScale = new Vector3(30f, 5f, 0.3f);
        frontWall.GetComponent<Renderer>().sharedMaterial = wallMat;

        // Techo
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Techo";
        ceiling.transform.position = new Vector3(0f, 5f, 7f);
        ceiling.transform.localScale = new Vector3(30f, 0.2f, 16f);
        ceiling.GetComponent<Renderer>().sharedMaterial = wallMat;

        // Pupitres (3 filas x 3 columnas)
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                float x = -6f + col * 6f;
                float z = 2f + row * 4f;

                // Mesa
                GameObject desk = GameObject.CreatePrimitive(PrimitiveType.Cube);
                desk.name = $"Pupitre_{row}_{col}";
                desk.transform.position = new Vector3(x, 0.55f, z);
                desk.transform.localScale = new Vector3(1.8f, 0.1f, 1.0f);
                desk.GetComponent<Renderer>().sharedMaterial = deskMat;

                // Patas de mesa
                for (int p = 0; p < 4; p++)
                {
                    float px = (p % 2 == 0) ? -0.7f : 0.7f;
                    float pz = (p < 2) ? -0.35f : 0.35f;
                    GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    leg.name = $"Pata_{row}_{col}_{p}";
                    leg.transform.position = new Vector3(x + px, 0.25f, z + pz);
                    leg.transform.localScale = new Vector3(0.06f, 0.5f, 0.06f);
                    leg.GetComponent<Renderer>().sharedMaterial = legMat;
                    leg.transform.parent = desk.transform;
                }

                // Silla
                GameObject chairSeat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                chairSeat.name = $"Silla_Asiento_{row}_{col}";
                chairSeat.transform.position = new Vector3(x, 0.38f, z - 0.9f);
                chairSeat.transform.localScale = new Vector3(0.55f, 0.06f, 0.55f);
                chairSeat.GetComponent<Renderer>().sharedMaterial = chairMat;

                GameObject chairBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
                chairBack.name = $"Silla_Respaldo_{row}_{col}";
                chairBack.transform.position = new Vector3(x, 0.6f, z - 1.15f);
                chairBack.transform.localScale = new Vector3(0.55f, 0.5f, 0.06f);
                chairBack.GetComponent<Renderer>().sharedMaterial = chairMat;
                chairBack.transform.parent = chairSeat.transform;
            }
        }

        // ==========================================
        // 4) ILUMINACIÓN
        // ==========================================
        // Luz ambiental cálida de aula
        RenderSettings.ambientLight = new Color(0.45f, 0.4f, 0.35f);

        GameObject ceilingLight1 = new GameObject("Luz_Techo_1");
        ceilingLight1.transform.position = new Vector3(-5f, 4.5f, 7f);
        Light l1 = ceilingLight1.AddComponent<Light>();
        l1.type = LightType.Point;
        l1.range = 18f;
        l1.intensity = 1.2f;
        l1.color = new Color(1f, 0.95f, 0.85f);

        GameObject ceilingLight2 = new GameObject("Luz_Techo_2");
        ceilingLight2.transform.position = new Vector3(5f, 4.5f, 7f);
        Light l2 = ceilingLight2.AddComponent<Light>();
        l2.type = LightType.Point;
        l2.range = 18f;
        l2.intensity = 1.2f;
        l2.color = new Color(1f, 0.95f, 0.85f);

        // ==========================================
        // 5) CÁMARA
        // ==========================================
        Camera mainCam = Camera.main;
        if (mainCam)
        {
            mainCam.transform.position = new Vector3(0f, 4f, -6f);
            mainCam.transform.rotation = Quaternion.Euler(20f, 0f, 0f);
            mainCam.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        }

        // ==========================================
        // 6) CANVAS PRINCIPAL (UI QUIZ)
        // ==========================================
        GameObject canvasObj = new GameObject("Canvas_Quiz");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // ---- FONDO OSCURO COMPLETO (para que el UI sea bien legible) ----
        GameObject bgOverlay = CreatePanel(canvasObj.transform, "Fondo_Overlay",
            Vector2.zero, Vector2.one,
            new Color(0f, 0f, 0f, 0.55f));

        // ---- PREGUNTA (Arriba Centro) ----
        GameObject questionPanel = CreatePanel(canvasObj.transform, "Panel_Pregunta",
            new Vector2(0.08f, 0.72f), new Vector2(0.92f, 0.95f),
            new Color(0.08f, 0.08f, 0.18f, 0.95f));
        // Borde decorativo
        CreatePanel(questionPanel.transform, "Borde_Pregunta",
            new Vector2(0f, 0f), new Vector2(1f, 0.04f),
            new Color(0.3f, 0.6f, 1f, 1f));

        Text questionText = CreateTextUI(questionPanel.transform, "Texto_Pregunta",
            "¿Pregunta?", 36, TextAnchor.MiddleCenter, Color.white,
            new Vector2(0.03f, 0.1f), new Vector2(0.97f, 0.95f));

        // ---- OPCIONES (Centro, Grilla 2x2) ----
        Color colorA = new Color(0.15f, 0.35f, 0.7f, 1f);  // Azul
        Color colorB = new Color(0.7f, 0.25f, 0.15f, 1f);  // Rojo
        Color colorC = new Color(0.1f, 0.55f, 0.3f, 1f);   // Verde
        Color colorD = new Color(0.7f, 0.55f, 0.1f, 1f);   // Amarillo

        Button[] optionButtons = new Button[4];
        Text[] optionTexts = new Text[4];

        CreateOptionBtn(canvasObj.transform, "Boton_A", "A) Opción 1",
            new Vector2(0.08f, 0.42f), new Vector2(0.48f, 0.65f), colorA,
            out optionButtons[0], out optionTexts[0]);

        CreateOptionBtn(canvasObj.transform, "Boton_B", "B) Opción 2",
            new Vector2(0.52f, 0.42f), new Vector2(0.92f, 0.65f), colorB,
            out optionButtons[1], out optionTexts[1]);

        CreateOptionBtn(canvasObj.transform, "Boton_C", "C) Opción 3",
            new Vector2(0.08f, 0.16f), new Vector2(0.48f, 0.39f), colorC,
            out optionButtons[2], out optionTexts[2]);

        CreateOptionBtn(canvasObj.transform, "Boton_D", "D) Opción 4",
            new Vector2(0.52f, 0.16f), new Vector2(0.92f, 0.39f), colorD,
            out optionButtons[3], out optionTexts[3]);

        // ---- HUD SUPERIOR IZQUIERDO: PUNTAJES ----
        GameObject scorePanel = CreatePanel(canvasObj.transform, "Panel_Puntajes",
            new Vector2(0.01f, 0.88f), new Vector2(0.28f, 1f),
            new Color(0f, 0f, 0f, 0.6f));
        Text scoreQuizText = CreateTextUI(scorePanel.transform, "Texto_PuntosQuiz",
            "Puntos Quiz: 0", 26, TextAnchor.MiddleLeft, Color.white,
            new Vector2(0.05f, 0.5f), new Vector2(0.95f, 0.95f));
        Text scoreGlobalText = CreateTextUI(scorePanel.transform, "Texto_PuntosGlobal",
            "Global: 0", 22, TextAnchor.MiddleLeft, new Color(0.6f, 0.85f, 1f),
            new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.5f));

        // ---- HUD SUPERIOR DERECHO: TIMER + PROGRESO ----
        GameObject timerPanel = CreatePanel(canvasObj.transform, "Panel_Timer",
            new Vector2(0.78f, 0.88f), new Vector2(0.99f, 1f),
            new Color(0f, 0f, 0f, 0.6f));

        // Timer fill ring
        GameObject timerFillObj = new GameObject("Timer_Fill");
        timerFillObj.transform.SetParent(timerPanel.transform, false);
        RectTransform timerFillRect = timerFillObj.AddComponent<RectTransform>();
        timerFillRect.anchorMin = new Vector2(0.0f, 0.1f);
        timerFillRect.anchorMax = new Vector2(0.4f, 0.9f);
        timerFillRect.offsetMin = Vector2.zero;
        timerFillRect.offsetMax = Vector2.zero;
        Image timerFillImg = timerFillObj.AddComponent<Image>();
        timerFillImg.type = Image.Type.Filled;
        timerFillImg.fillMethod = Image.FillMethod.Radial360;
        timerFillImg.fillAmount = 1f;
        timerFillImg.color = Color.green;

        Text timerText = CreateTextUI(timerPanel.transform, "Texto_Timer",
            "15", 38, TextAnchor.MiddleCenter, Color.white,
            new Vector2(0.4f, 0.15f), new Vector2(0.95f, 0.85f));

        Text progressText = CreateTextUI(canvasObj.transform, "Texto_Progreso",
            "Pregunta: 0/5", 22, TextAnchor.MiddleRight, new Color(1f, 0.85f, 0.4f),
            new Vector2(0.65f, 0.83f), new Vector2(0.99f, 0.88f));

        // ---- COMODINES (Parte inferior) ----
        Color jokerPurple = new Color(0.45f, 0.15f, 0.65f, 1f);
        Color jokerBlue = new Color(0.15f, 0.4f, 0.65f, 1f);
        Color jokerOrange = new Color(0.65f, 0.4f, 0.1f, 1f);

        Button fiftyBtn = CreateJokerBtn(canvasObj.transform, "Boton_5050", "50/50",
            new Vector2(0.15f, 0.02f), new Vector2(0.35f, 0.12f), jokerPurple);
        Button hintBtn = CreateJokerBtn(canvasObj.transform, "Boton_Pista", "PISTA",
            new Vector2(0.40f, 0.02f), new Vector2(0.60f, 0.12f), jokerBlue);
        Button publicBtn = CreateJokerBtn(canvasObj.transform, "Boton_Publico", "PUBLICO",
            new Vector2(0.65f, 0.02f), new Vector2(0.85f, 0.12f), jokerOrange);

        // ---- PANEL DE PISTA (oculto por defecto) ----
        GameObject hintPanel = CreatePanel(canvasObj.transform, "Panel_Pista",
            new Vector2(0.2f, 0.3f), new Vector2(0.8f, 0.7f),
            new Color(0.05f, 0.05f, 0.2f, 0.95f));
        CreatePanel(hintPanel.transform, "Borde_Pista_Top",
            new Vector2(0f, 0.92f), new Vector2(1f, 1f),
            new Color(1f, 0.8f, 0.2f, 1f));
        CreateTextUI(hintPanel.transform, "Titulo_Pista",
            "PISTA", 28, TextAnchor.MiddleCenter, new Color(1f, 0.85f, 0.3f),
            new Vector2(0.1f, 0.75f), new Vector2(0.9f, 0.92f));
        Text hintDisplayText = CreateTextUI(hintPanel.transform, "Texto_Pista_Contenido",
            "Pista...", 26, TextAnchor.MiddleCenter, Color.white,
            new Vector2(0.08f, 0.15f), new Vector2(0.92f, 0.72f));
        hintPanel.SetActive(false);

        // ---- PANEL DE RESULTADO FINAL (oculto por defecto) ----
        GameObject resultPanel = CreatePanel(canvasObj.transform, "Panel_Resultado",
            new Vector2(0.15f, 0.15f), new Vector2(0.85f, 0.85f),
            new Color(0.03f, 0.03f, 0.1f, 0.95f));
        CreatePanel(resultPanel.transform, "Borde_Resultado",
            new Vector2(0f, 0.88f), new Vector2(1f, 1f),
            new Color(0.3f, 0.7f, 0.3f, 1f));
        Text resultDisplayText = CreateTextUI(resultPanel.transform, "Texto_Resultado",
            "¡Resultado!", 48, TextAnchor.MiddleCenter, Color.white,
            new Vector2(0.05f, 0.3f), new Vector2(0.95f, 0.85f));

        // Botón Volver al HUB
        Button returnBtn;
        Text dummyReturnText;
        CreateOptionBtn(resultPanel.transform, "Boton_Volver", "Volver al HUB",
            new Vector2(0.25f, 0.05f), new Vector2(0.75f, 0.22f),
            new Color(0.2f, 0.55f, 0.2f, 1f),
            out returnBtn, out dummyReturnText);

        resultPanel.SetActive(false);

        // ==========================================
        // 7) EVENT SYSTEM
        // ==========================================
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // ==========================================
        // 8) GAME MANAGER OBJECT (todos los scripts)
        // ==========================================
        GameObject gmObj = new GameObject("===== QUIZ_GAME_MANAGER =====");

        QuizGameManager gm = gmObj.AddComponent<QuizGameManager>();
        QuizUIController uiCtrl = gmObj.AddComponent<QuizUIController>();
        TimerController timerCtrl = gmObj.AddComponent<TimerController>();
        JokerManager jokerMgr = gmObj.AddComponent<JokerManager>();

        // Asignar referencias de UI
        uiCtrl.questionText = questionText;
        uiCtrl.optionButtons = optionButtons;
        uiCtrl.optionTexts = optionTexts;
        uiCtrl.currentScoreText = scoreQuizText;
        uiCtrl.globalScoreText = scoreGlobalText;
        uiCtrl.progressText = progressText;
        uiCtrl.resultPanel = resultPanel;
        uiCtrl.resultText = resultDisplayText;
        uiCtrl.hintPanel = hintPanel;
        uiCtrl.hintText = hintDisplayText;

        timerCtrl.timerImage = timerFillImg;
        timerCtrl.timerText = timerText;

        jokerMgr.fiftyFiftyButton = fiftyBtn;
        jokerMgr.hintButton = hintBtn;
        jokerMgr.publicOpinionButton = publicBtn;

        gm.uiController = uiCtrl;
        gm.timerController = timerCtrl;
        gm.jokerManager = jokerMgr;

        // Conectar botones de opciones
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            optionButtons[i].onClick.AddListener(() => gm.SubmitAnswer(index));
        }

        // ==========================================
        // 9) ASIGNAR QUESTIONBANK SI EXISTE
        // ==========================================
        string bankPath = "Assets/Data/Quiz/QuestionBank_Default.asset";
        QuestionBankSO existingBank = AssetDatabase.LoadAssetAtPath<QuestionBankSO>(bankPath);
        if (existingBank != null)
        {
            gm.currentBank = existingBank;
            Debug.Log("✅ Banco de preguntas asignado automáticamente.");
        }
        else
        {
            // Crear el banco automáticamente
            Debug.Log("No se encontró banco, creándolo automáticamente...");
            CreateSampleQuestionBank();
            existingBank = AssetDatabase.LoadAssetAtPath<QuestionBankSO>(bankPath);
            if (existingBank != null)
                gm.currentBank = existingBank;
        }

        // ==========================================
        // 10) GUARDAR ESCENA
        // ==========================================
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");

        string scenePath = "Assets/Scenes/Quiz.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);

        // Agregar a Build Settings automáticamente
        AddSceneToBuildSettings(scenePath);

        Debug.Log($"✅ Escena Quiz generada en: {scenePath}");
        EditorUtility.DisplayDialog("✅ Escena Quiz Lista",
            "La escena del Quiz ha sido creada con éxito.\n\n" +
            "• Aula 3D con pupitres, pizarra y sillas de colores\n" +
            "• UI completa con pregunta, opciones, timer y comodines\n" +
            "• Banco de preguntas ya asignado\n" +
            "• Escena ya agregada a Build Settings\n\n" +
            "Dale PLAY para probar.",
            "¡Perfecto!");
    }

    // =====================================================
    // CREAR BANCO DE PREGUNTAS DE EJEMPLO
    // =====================================================
    [MenuItem("Tools/Quiz/Crear Banco de Preguntas de Ejemplo")]
    public static void CreateSampleQuestionBank()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
            AssetDatabase.CreateFolder("Assets", "Data");
        if (!AssetDatabase.IsValidFolder("Assets/Data/Quiz"))
            AssetDatabase.CreateFolder("Assets/Data", "Quiz");

        QuestionBankSO bank = ScriptableObject.CreateInstance<QuestionBankSO>();
        bank.subjectName = "Historia General";

        bank.questions = new System.Collections.Generic.List<QuestionData>
        {
            new QuestionData
            {
                questionText = "¿En qué año llegó Cristóbal Colón a América?",
                options = new string[] { "1492", "1520", "1402", "1500" },
                correctOptionIndex = 0,
                hintText = "Fue a finales del siglo XV."
            },
            new QuestionData
            {
                questionText = "¿Quién escribió 'Don Quijote de la Mancha'?",
                options = new string[] { "Lope de Vega", "Miguel de Cervantes", "Shakespeare", "Garcilaso" },
                correctOptionIndex = 1,
                hintText = "Es considerado el padre de la novela moderna española."
            },
            new QuestionData
            {
                questionText = "¿Cuál es el resultado de 7 × 8?",
                options = new string[] { "54", "58", "56", "48" },
                correctOptionIndex = 2,
                hintText = "Es un número entre 50 y 60."
            },
            new QuestionData
            {
                questionText = "¿Cuál es el planeta más grande del sistema solar?",
                options = new string[] { "Saturno", "Marte", "Urano", "Júpiter" },
                correctOptionIndex = 3,
                hintText = "Es un gigante gaseoso con una gran mancha roja."
            },
            new QuestionData
            {
                questionText = "¿Cuál es la capital de Francia?",
                options = new string[] { "Madrid", "Londres", "París", "Berlín" },
                correctOptionIndex = 2,
                hintText = "Es conocida como la 'Ciudad de la Luz'."
            },
            new QuestionData
            {
                questionText = "¿Qué elemento químico tiene el símbolo 'O'?",
                options = new string[] { "Oro", "Osmio", "Oxígeno", "Oganesón" },
                correctOptionIndex = 2,
                hintText = "Es esencial para la respiración."
            },
            new QuestionData
            {
                questionText = "¿En qué continente se encuentra Egipto?",
                options = new string[] { "Asia", "Europa", "África", "Oceanía" },
                correctOptionIndex = 2,
                hintText = "Las pirámides están en este continente."
            }
        };

        string path = "Assets/Data/Quiz/QuestionBank_Default.asset";
        AssetDatabase.CreateAsset(bank, path);
        AssetDatabase.SaveAssets();
        Debug.Log($"✅ Banco de preguntas creado en: {path}");
    }

    // =====================================================
    // HELPERS
    // =====================================================
    private static Material CreateOrLoadMaterial(string path, Color color)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null) return mat;

        mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static void AddSceneToBuildSettings(string scenePath)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(
            EditorBuildSettings.scenes);
        
        // Verificar si ya está
        foreach (var s in scenes)
        {
            if (s.path == scenePath) return;
        }

        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log($"✅ Escena '{scenePath}' agregada a Build Settings.");
    }

    private static GameObject CreatePanel(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image img = panel.AddComponent<Image>();
        img.color = color;
        return panel;
    }

    private static Text CreateTextUI(Transform parent, string name, string content,
        int fontSize, TextAnchor alignment, Color color,
        Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 14;
        text.resizeTextMaxSize = fontSize;

        // Agregar Outline para mejorar legibilidad
        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.8f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        return text;
    }

    private static void CreateOptionBtn(Transform parent, string name, string label,
        Vector2 anchorMin, Vector2 anchorMax, Color bgColor,
        out Button button, out Text text)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = new Vector2(4f, 4f);
        rect.offsetMax = new Vector2(-4f, -4f);
        Image img = btnObj.AddComponent<Image>();
        img.color = bgColor;
        button = btnObj.AddComponent<Button>();

        // Navegación solo por teclado/mouse
        var nav = button.navigation;
        nav.mode = UnityEngine.UI.Navigation.Mode.None;
        button.navigation = nav;

        text = CreateTextUI(btnObj.transform, name + "_Texto", label,
            28, TextAnchor.MiddleCenter, Color.white,
            new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.95f));
    }

    private static Button CreateJokerBtn(Transform parent, string name, string label,
        Vector2 anchorMin, Vector2 anchorMax, Color bgColor)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = new Vector2(3f, 3f);
        rect.offsetMax = new Vector2(-3f, -3f);
        Image img = btnObj.AddComponent<Image>();
        img.color = bgColor;
        Button btn = btnObj.AddComponent<Button>();

        var nav = btn.navigation;
        nav.mode = UnityEngine.UI.Navigation.Mode.None;
        btn.navigation = nav;

        CreateTextUI(btnObj.transform, name + "_Texto", label,
            24, TextAnchor.MiddleCenter, Color.white,
            new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.95f));
        return btn;
    }
}
#endif
