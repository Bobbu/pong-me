using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attach this to an empty GameObject in a blank scene.
/// It builds the entire Pong game programmatically — no manual scene setup needed.
/// </summary>
public class GameSetup : MonoBehaviour
{
    void Awake()
    {
        BuildGame();
    }

    void BuildGame()
    {
#if !UNITY_IOS
        // Force windowed mode on desktop builds
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(1920, 1080, false);
#endif

        // Camera — force solid black background (no skybox)
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.black;
        Camera.main.orthographic = true;

        // Adapt camera to aspect ratio — ensure full game area (18 units wide, 12 tall) is visible
        float targetAspect = 18f / 12f; // 1.5 (game area width / height)
        float screenAspect = (float)Screen.width / Screen.height;
        if (screenAspect < targetAspect)
        {
            // Screen is taller than game area — increase ortho size to fit width
            Camera.main.orthographicSize = 9f / screenAspect;
        }
        else
        {
            // Screen is wider than game area — 6 units half-height is fine
            Camera.main.orthographicSize = 6f;
        }

        // Remove the default directional light if present
        var light = FindAnyObjectByType<Light>();
        if (light != null)
            Destroy(light.gameObject);

        // Materials
        Material greenMat = new Material(Shader.Find("Sprites/Default"));
        greenMat.color = new Color(0f, 1f, 0.2f); // bold green

        // --- Ball ---
        GameObject ball = CreateQuad("Ball", Vector3.zero, new Vector3(0.4f, 0.4f, 1f), greenMat);
        ball.tag = "Ball";
        var ballRb = ball.AddComponent<Rigidbody2D>();
        ballRb.gravityScale = 0f;
        ballRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        ballRb.freezeRotation = true;
        ball.AddComponent<BoxCollider2D>();
        var ballCtrl = ball.AddComponent<BallController>();

        // Bouncy physics material
        PhysicsMaterial2D bounceMat = new PhysicsMaterial2D("Bounce");
        bounceMat.bounciness = 1f;
        bounceMat.friction = 0f;
        ballRb.sharedMaterial = bounceMat;

        // --- Left Paddle (Player — W/S) ---
        GameObject leftPaddle = CreateQuad("LeftPaddle", new Vector3(-8f, 0f, 0f), new Vector3(0.4f, 2.5f, 1f), greenMat);
        leftPaddle.tag = "Paddle";
        var lpRb = leftPaddle.AddComponent<Rigidbody2D>();
        lpRb.bodyType = RigidbodyType2D.Kinematic;
        leftPaddle.AddComponent<BoxCollider2D>();
        var lpCtrl = leftPaddle.AddComponent<PaddleController>();
        lpCtrl.isPlayer = true;
        lpCtrl.upKey = KeyCode.W;
        lpCtrl.downKey = KeyCode.S;

        // --- Right Paddle (AI or Player 2 — Up/Down) ---
        GameObject rightPaddle = CreateQuad("RightPaddle", new Vector3(8f, 0f, 0f), new Vector3(0.4f, 2.5f, 1f), greenMat);
        rightPaddle.tag = "Paddle";
        var rpRb = rightPaddle.AddComponent<Rigidbody2D>();
        rpRb.bodyType = RigidbodyType2D.Kinematic;
        rightPaddle.AddComponent<BoxCollider2D>();
        var rpCtrl = rightPaddle.AddComponent<PaddleController>();
        rpCtrl.isPlayer = false;
        rpCtrl.ball = ball.transform;

        // --- Walls (top and bottom) ---
        CreateWall("TopWall", new Vector3(0f, 6f, 0f), new Vector3(20f, 1f, 1f));
        CreateWall("BottomWall", new Vector3(0f, -6f, 0f), new Vector3(20f, 1f, 1f));

        // --- Border lines (visible boundary indicators) ---
        CreateQuad("TopBorder", new Vector3(0f, 5.5f, 0f), new Vector3(18f, 0.06f, 1f), greenMat);
        CreateQuad("BottomBorder", new Vector3(0f, -5.5f, 0f), new Vector3(18f, 0.06f, 1f), greenMat);

        // --- Center Line (visual only) ---
        for (float y = -5.5f; y <= 5.5f; y += 1f)
        {
            GameObject dash = CreateQuad("Dash", new Vector3(0f, y, 0f), new Vector3(0.1f, 0.4f, 1f), greenMat);
            var dashColor = dash.GetComponent<SpriteRenderer>();
            if (dashColor != null)
                dashColor.color = new Color(0f, 0.5f, 0.1f, 0.5f);
        }

        // --- Goal Zones (triggers behind paddles) ---
        GameObject leftGoal = new GameObject("LeftGoal");
        leftGoal.transform.position = new Vector3(-10f, 0f, 0f);
        var lgCol = leftGoal.AddComponent<BoxCollider2D>();
        lgCol.size = new Vector2(1f, 14f);
        lgCol.isTrigger = true;
        var lgZone = leftGoal.AddComponent<GoalZone>();
        lgZone.isLeftGoal = true;

        GameObject rightGoal = new GameObject("RightGoal");
        rightGoal.transform.position = new Vector3(10f, 0f, 0f);
        var rgCol = rightGoal.AddComponent<BoxCollider2D>();
        rgCol.size = new Vector2(1f, 14f);
        rgCol.isTrigger = true;
        var rgZone = rightGoal.AddComponent<GoalZone>();
        rgZone.isLeftGoal = false;

        // =============================================
        // UI
        // =============================================

        // EventSystem is required for UI buttons to receive mouse clicks
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        GameObject canvas = new GameObject("Canvas");
        var c = canvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 10;
        var scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;
        canvas.AddComponent<GraphicRaycaster>();

        // Safe area container — all UI goes inside this so it avoids
        // the iPhone notch/Dynamic Island and rounded corners
        GameObject safeArea = new GameObject("SafeArea");
        safeArea.transform.SetParent(canvas.transform, false);
        var safeRt = safeArea.AddComponent<RectTransform>();
        safeRt.anchorMin = Vector2.zero;
        safeRt.anchorMax = Vector2.one;
        safeRt.offsetMin = Vector2.zero;
        safeRt.offsetMax = Vector2.zero;
        safeArea.AddComponent<SafeAreaHandler>();

        // --- Scores (above the top border line) ---
        TextMeshProUGUI leftScoreText = CreateScoreText(safeArea.transform, "LeftScore", -200f);
        TextMeshProUGUI rightScoreText = CreateScoreText(safeArea.transform, "RightScore", 200f);

        // --- Win overlay (center, hidden) ---
        GameObject winObj = new GameObject("WinOverlay");
        winObj.transform.SetParent(safeArea.transform, false);
        var winRt = winObj.AddComponent<RectTransform>();
        winRt.anchorMin = Vector2.zero;
        winRt.anchorMax = Vector2.one;
        winRt.offsetMin = Vector2.zero;
        winRt.offsetMax = Vector2.zero;

        // Win message text
        GameObject winTextObj = new GameObject("WinText");
        winTextObj.transform.SetParent(winObj.transform, false);
        var winTextRt = winTextObj.AddComponent<RectTransform>();
        winTextRt.anchoredPosition = new Vector2(0f, 40f);
        winTextRt.sizeDelta = new Vector2(800f, 150f);
        var winTmp = winTextObj.AddComponent<TextMeshProUGUI>();
        winTmp.text = "";
        winTmp.fontSize = 60;
        winTmp.color = new Color(0f, 1f, 0.2f);
        winTmp.alignment = TextAlignmentOptions.Center;
        winTmp.fontStyle = FontStyles.Bold;

        // Play Again button
        GameObject playAgainObj = new GameObject("PlayAgainBtn");
        playAgainObj.transform.SetParent(winObj.transform, false);
        var paBtnRt = playAgainObj.AddComponent<RectTransform>();
        paBtnRt.anchoredPosition = new Vector2(0f, -120f);
        paBtnRt.sizeDelta = new Vector2(250f, 60f);
        var paBtnImg = playAgainObj.AddComponent<Image>();
        paBtnImg.sprite = CreateOpaqueSprite();
        paBtnImg.color = new Color(0f, 0.25f, 0.05f, 1f);
        var paBtn = playAgainObj.AddComponent<Button>();
        var paColors = paBtn.colors;
        paColors.highlightedColor = new Color(0f, 0.5f, 0.12f);
        paColors.pressedColor = new Color(0f, 1f, 0.2f);
        paBtn.colors = paColors;

        GameObject paTxtObj = new GameObject("Label");
        paTxtObj.transform.SetParent(playAgainObj.transform, false);
        var paTxtRt = paTxtObj.AddComponent<RectTransform>();
        paTxtRt.anchorMin = Vector2.zero;
        paTxtRt.anchorMax = Vector2.one;
        paTxtRt.offsetMin = Vector2.zero;
        paTxtRt.offsetMax = Vector2.zero;
        var paTxt = paTxtObj.AddComponent<TextMeshProUGUI>();
        paTxt.text = "PLAY AGAIN";
        paTxt.fontSize = 28;
        paTxt.color = new Color(0f, 1f, 0.2f);
        paTxt.alignment = TextAlignmentOptions.Center;
        paTxt.fontStyle = FontStyles.Bold;
        paTxt.raycastTarget = false;

        winObj.SetActive(false);

        // --- Settings icon (top-left) ---
        GameObject settingsBtn = CreateIconButton(safeArea.transform, "SettingsBtn",
            new Vector2(35f, -35f), TextAnchor.UpperLeft, "*", true);
        GameObject settingsPanel = CreateSettingsPanel(safeArea.transform, ballCtrl, rpCtrl);
        settingsPanel.SetActive(false);
        settingsBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            bool opening = !settingsPanel.activeSelf;
            settingsPanel.SetActive(opening);
            // Close help if open
            var hp = safeArea.transform.Find("HelpPanel");
            if (hp != null && hp.gameObject.activeSelf)
                hp.gameObject.SetActive(false);
            // Pause/unpause
            bool anyOpen = opening; // settings just opened
            if (GameManager.Instance != null)
                GameManager.Instance.SetPaused(anyOpen);
        });

        // --- Help icon (top-right) ---
        GameObject helpBtn = CreateIconButton(safeArea.transform, "HelpBtn",
            new Vector2(-35f, -35f), TextAnchor.UpperRight, "?", false);
        GameObject helpPanel = CreateHelpPanel(safeArea.transform);
        helpPanel.SetActive(false);
        helpBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            bool opening = !helpPanel.activeSelf;
            helpPanel.SetActive(opening);
            // Close settings if open
            if (settingsPanel.activeSelf)
                settingsPanel.SetActive(false);
            // Pause/unpause
            bool anyOpen = opening;
            if (GameManager.Instance != null)
                GameManager.Instance.SetPaused(anyOpen);
        });

        // --- Sound Manager ---
        GameObject sm = new GameObject("SoundManager");
        var sound = sm.AddComponent<SoundManager>();
        sound.wallBounce = SoundManager.GenerateWallBounce();
        sound.paddleHit = SoundManager.GeneratePaddleHit();
        sound.score = SoundManager.GenerateScore();
        sound.win = SoundManager.GenerateWin();

        // --- Game Manager ---
        GameObject gm = new GameObject("GameManager");
        var manager = gm.AddComponent<GameManager>();
        manager.leftScoreText = leftScoreText;
        manager.rightScoreText = rightScoreText;
        manager.winText = winTmp;
        manager.winOverlay = winObj;
        manager.ball = ballCtrl;

        // Wire Play Again button to reset
        paBtn.onClick.AddListener(() => manager.ResetGame());
    }

    // =============================================
    // Settings Panel
    // =============================================
    GameObject CreateSettingsPanel(Transform parent, BallController ball, PaddleController aiPaddle)
    {
        GameObject panel = CreatePanel(parent, "SettingsPanel", new Vector2(220f, 490f),
            new Vector2(140f, -90f), TextAnchor.UpperLeft);
        panel.AddComponent<RectMask2D>();

        float y = -10f;

        // Title
        CreatePanelText(panel.transform, "SettingsTitle", "SETTINGS",
            new Vector2(0f, y), 22, FontStyles.Bold, TextAlignmentOptions.Center, 30f);
        y -= 40f;

        // Speed label
        var speedLabel = CreatePanelText(panel.transform, "SpeedLabel", "Speed: MEDIUM",
            new Vector2(0f, y), 18, FontStyles.Normal, TextAlignmentOptions.Center, 25f);
        y -= 50f;

        // Speed buttons
        CreatePanelButton(panel.transform, "SlowBtn", "SLOW", new Vector2(-70f, y), () =>
        {
            if (GameManager.Instance != null) GameManager.Instance.SetSpeed(5f, 0.3f, 12f);
            speedLabel.text = "Speed: SLOW";
        });
        CreatePanelButton(panel.transform, "MedBtn", "MED", new Vector2(0f, y), () =>
        {
            if (GameManager.Instance != null) GameManager.Instance.SetSpeed(8f, 0.5f, 20f);
            speedLabel.text = "Speed: MEDIUM";
        });
        CreatePanelButton(panel.transform, "FastBtn", "FAST", new Vector2(70f, y), () =>
        {
            if (GameManager.Instance != null) GameManager.Instance.SetSpeed(12f, 0.8f, 28f);
            speedLabel.text = "Speed: FAST";
        });
        y -= 40f;

        // AI difficulty label
        var aiLabel = CreatePanelText(panel.transform, "AILabel", "AI: MEDIUM",
            new Vector2(0f, y), 18, FontStyles.Normal, TextAlignmentOptions.Center, 25f);
        y -= 50f;

        // AI difficulty buttons
        //                                   reactionSpeed, errorRange, reactionDelay
        CreatePanelButton(panel.transform, "AIEasyBtn", "EASY", new Vector2(-70f, y), () =>
        {
            aiPaddle.SetDifficulty(3.5f, 1.5f, 0.3f);
            aiLabel.text = "AI: EASY";
        });
        CreatePanelButton(panel.transform, "AIMedBtn", "MED", new Vector2(0f, y), () =>
        {
            aiPaddle.SetDifficulty(6f, 0.5f, 0.1f);
            aiLabel.text = "AI: MEDIUM";
        });
        CreatePanelButton(panel.transform, "AIHardBtn", "HARD", new Vector2(70f, y), () =>
        {
            aiPaddle.SetDifficulty(10f, 0.1f, 0.02f);
            aiLabel.text = "AI: HARD";
        });
        y -= 40f;

        // Sound label
        var soundLabel = CreatePanelText(panel.transform, "SoundLabel", "Sound: ON",
            new Vector2(0f, y), 18, FontStyles.Normal, TextAlignmentOptions.Center, 25f);
        y -= 50f;

        // Sound buttons
        CreatePanelButton(panel.transform, "SoundOnBtn", "ON", new Vector2(-35f, y), () =>
        {
            if (SoundManager.Instance != null) SoundManager.Instance.SetMuted(false);
            soundLabel.text = "Sound: ON";
        });
        CreatePanelButton(panel.transform, "SoundOffBtn", "OFF", new Vector2(35f, y), () =>
        {
            if (SoundManager.Instance != null) SoundManager.Instance.SetMuted(true);
            soundLabel.text = "Sound: OFF";
        });
        y -= 40f;

        // Win score label
        var winScoreLabel = CreatePanelText(panel.transform, "WinScoreLabel", "Win Score: 3",
            new Vector2(0f, y), 18, FontStyles.Normal, TextAlignmentOptions.Center, 25f);
        y -= 50f;

        // Win score buttons
        CreatePanelButton(panel.transform, "Win3Btn", "3", new Vector2(-70f, y), () =>
        {
            if (GameManager.Instance != null) GameManager.Instance.winningScore = 3;
            winScoreLabel.text = "Win Score: 3";
        });
        CreatePanelButton(panel.transform, "Win5Btn", "5", new Vector2(0f, y), () =>
        {
            if (GameManager.Instance != null) GameManager.Instance.winningScore = 5;
            winScoreLabel.text = "Win Score: 5";
        });
        CreatePanelButton(panel.transform, "Win7Btn", "7", new Vector2(70f, y), () =>
        {
            if (GameManager.Instance != null) GameManager.Instance.winningScore = 7;
            winScoreLabel.text = "Win Score: 7";
        });
        y -= 50f;

        // Close hint
        CreatePanelText(panel.transform, "CloseHint", "tap gear to close",
            new Vector2(0f, y), 14, FontStyles.Italic, TextAlignmentOptions.Center, 25f);

        return panel;
    }

    void CreatePanelButton(Transform parent, string name, string label, Vector2 pos, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        var rt = btnObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(60f, 30f);

        var img = btnObj.AddComponent<Image>();
        img.color = new Color(0f, 0.25f, 0.05f, 0.9f);

        var btn = btnObj.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(0f, 0.6f, 0.15f);
        colors.pressedColor = new Color(0f, 1f, 0.2f);
        btn.colors = colors;
        btn.onClick.AddListener(onClick);

        GameObject txtObj = new GameObject("Label");
        txtObj.transform.SetParent(btnObj.transform, false);
        var txtRt = txtObj.AddComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = Vector2.zero;
        txtRt.offsetMax = Vector2.zero;
        var tmp = txtObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 14;
        tmp.color = new Color(0f, 1f, 0.2f);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
    }

    // =============================================
    // Help Panel
    // =============================================
    GameObject CreateHelpPanel(Transform parent)
    {
        GameObject panel = CreatePanel(parent, "HelpPanel", new Vector2(300f, 520f),
            new Vector2(-180f, -90f), TextAnchor.UpperRight);

        // Enable clipping so nothing renders outside the panel
        var mask = panel.AddComponent<RectMask2D>();

        CreatePanelText(panel.transform, "HelpTitle", "? INSTRUCTIONS",
            new Vector2(0f, -10f), 22, FontStyles.Bold, TextAlignmentOptions.Center, 35f);

        string instructions =
            "<b>CONTROLS</b>\n" +
            "  W / S  -  Move paddle up/down\n" +
            "  Touch  -  Drag left side of screen\n" +
            "  R  -  Reset / New game\n" +
            "  M  -  Toggle sound\n" +
            "  Cmd+Q  -  Quit (Mac)\n\n" +
            "<b>OBJECTIVE</b>\n" +
            "  First to target score wins.\n" +
            "  Don't let the ball pass\n" +
            "  your paddle!\n\n" +
            "<b>SPEED</b>\n" +
            "  Tap gear or press 1/2/3\n" +
            "  to change ball speed.\n\n" +
            "<i>tap ? to close</i>";

        CreatePanelText(panel.transform, "HelpBody", instructions,
            new Vector2(0f, -50f), 16, FontStyles.Normal, TextAlignmentOptions.Left, 450f);

        return panel;
    }

    // =============================================
    // Shared UI helpers
    // =============================================
    GameObject CreatePanel(Transform parent, string name, Vector2 size, Vector2 pos, TextAnchor corner)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        var rt = panel.AddComponent<RectTransform>();
        rt.sizeDelta = size;

        // Anchor to corner
        if (corner == TextAnchor.UpperLeft)
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
        }
        else // UpperRight
        {
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
        }
        rt.anchoredPosition = pos;

        var img = panel.AddComponent<Image>();
        img.sprite = CreateOpaqueSprite();
        img.type = Image.Type.Simple;
        img.color = new Color(0.02f, 0.06f, 0.02f, 1f);

        return panel;
    }

    TextMeshProUGUI CreatePanelText(Transform parent, string name, string text, Vector2 pos, int fontSize, FontStyles style, TextAlignmentOptions align = TextAlignmentOptions.Center, float height = 35f)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(270f, height);
        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.color = new Color(0f, 1f, 0.2f);
        tmp.alignment = align;
        tmp.fontStyle = style;
        tmp.richText = true;
        return tmp;
    }

    GameObject CreateIconButton(Transform parent, string name, Vector2 pos, TextAnchor corner, string icon, bool useGearSprite)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        var rt = btnObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50f, 50f);

        if (corner == TextAnchor.UpperLeft)
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
        }
        else
        {
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
        }
        rt.anchoredPosition = pos;

        var img = btnObj.AddComponent<Image>();
        img.color = new Color(0f, 0.15f, 0.04f, 0.8f);

        var btn = btnObj.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(0f, 0.4f, 0.1f);
        colors.pressedColor = new Color(0f, 0.7f, 0.18f);
        btn.colors = colors;

        if (useGearSprite)
        {
            // Draw a gear icon as a sprite (TMP default font lacks gear unicode)
            GameObject gearObj = new GameObject("GearIcon");
            gearObj.transform.SetParent(btnObj.transform, false);
            var gearRt = gearObj.AddComponent<RectTransform>();
            gearRt.anchorMin = Vector2.zero;
            gearRt.anchorMax = Vector2.one;
            gearRt.offsetMin = new Vector2(8f, 8f);
            gearRt.offsetMax = new Vector2(-8f, -8f);
            var gearImg = gearObj.AddComponent<Image>();
            gearImg.sprite = CreateGearSprite();
            gearImg.color = new Color(0f, 1f, 0.2f);
            gearImg.raycastTarget = false;
        }
        else
        {
            // Text icon (for characters the font supports)
            GameObject txtObj = new GameObject("Icon");
            txtObj.transform.SetParent(btnObj.transform, false);
            var txtRt = txtObj.AddComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero;
            txtRt.anchorMax = Vector2.one;
            txtRt.offsetMin = Vector2.zero;
            txtRt.offsetMax = Vector2.zero;
            var tmp = txtObj.AddComponent<TextMeshProUGUI>();
            tmp.text = icon;
            tmp.fontSize = 30;
            tmp.color = new Color(0f, 1f, 0.2f);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.raycastTarget = false;
        }

        return btnObj;
    }

    Sprite CreateGearSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = Color.clear;
        Color white = Color.white;
        float center = size / 2f;
        float outerR = size * 0.45f;
        float innerR = size * 0.28f;
        float holeR = size * 0.14f;
        int teeth = 8;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float angle = Mathf.Atan2(dy, dx);

                // Tooth pattern: oscillate between inner and outer radius
                float toothWave = Mathf.Cos(angle * teeth);
                float gearR = toothWave > 0.2f ? outerR : innerR;

                if (dist < gearR && dist > holeR)
                    tex.SetPixel(x, y, white);
                else
                    tex.SetPixel(x, y, clear);
            }
        }

        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    // =============================================
    // Game object helpers
    // =============================================
    GameObject CreateQuad(string name, Vector3 position, Vector3 scale, Material mat)
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateWhiteSquareSprite();
        sr.material = mat;
        sr.color = new Color(0f, 1f, 0.2f);
        return obj;
    }

    void CreateWall(string name, Vector3 position, Vector3 size)
    {
        GameObject wall = new GameObject(name);
        wall.transform.position = position;
        var col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
        var rb = wall.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
    }

    TextMeshProUGUI CreateScoreText(Transform parent, string name, float xOffset)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        var rt = textObj.AddComponent<RectTransform>();
        // Anchor to top-center of safe area
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(xOffset, -5f);
        rt.sizeDelta = new Vector2(200f, 80f);
        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "0";
        tmp.fontSize = 60;
        tmp.color = new Color(0f, 1f, 0.2f);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        return tmp;
    }

    Sprite CreateWhiteSquareSprite()
    {
        Texture2D tex = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
    }

    Sprite CreateOpaqueSprite()
    {
        Texture2D tex = new Texture2D(4, 4, TextureFormat.RGB24, false);
        Color[] pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
    }
}
