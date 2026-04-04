using UnityEngine;
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
        // Camera
        Camera.main.backgroundColor = Color.black;
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 6f;

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
        var ballCol = ball.AddComponent<BoxCollider2D>();
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
        rpCtrl.isPlayer = false; // AI by default
        rpCtrl.ball = ball.transform;

        // --- Walls (top and bottom) ---
        CreateWall("TopWall", new Vector3(0f, 6f, 0f), new Vector3(20f, 1f, 1f));
        CreateWall("BottomWall", new Vector3(0f, -6f, 0f), new Vector3(20f, 1f, 1f));

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

        // --- UI / Scoreboard ---
        GameObject canvas = new GameObject("Canvas");
        var c = canvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<UnityEngine.UI.CanvasScaler>();

        TextMeshProUGUI leftScoreText = CreateScoreText(canvas.transform, "LeftScore", new Vector2(-200f, 250f));
        TextMeshProUGUI rightScoreText = CreateScoreText(canvas.transform, "RightScore", new Vector2(200f, 250f));

        // --- Game Manager ---
        GameObject gm = new GameObject("GameManager");
        var manager = gm.AddComponent<GameManager>();
        manager.leftScoreText = leftScoreText;
        manager.rightScoreText = rightScoreText;
        manager.ball = ballCtrl;
    }

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

    TextMeshProUGUI CreateScoreText(Transform parent, string name, Vector2 position)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        var rt = textObj.AddComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(200f, 100f);
        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "0";
        tmp.fontSize = 72;
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
}
