using UnityEngine;
using TMPro;

public class FPSCount : MonoBehaviour
{
    [Header("Settings")]
    public float updateInterval = 0.5f; // Как часто обновлять текст
    public TextMeshProUGUI fpsText; // Ссылка на UI-текст
    private float timeSinceLastUpdate = 0f;
    private int frameCount = 0;

    void Start()
    {
        Application.targetFrameRate = 144;

        // Если текст не назначен в инспекторе, ищем его на объекте
        if (fpsText == null)
            fpsText = GetComponent<TextMeshProUGUI>();

        // Если всё равно null — создаём объект сами
        if (fpsText == null)
        {
            GameObject textObj = new GameObject("FPS_Text");
            textObj.transform.SetParent(transform);
            fpsText = textObj.AddComponent<TextMeshProUGUI>();
            fpsText.fontSize = 36;
            fpsText.color = Color.white;
        }
    }

    void Update()
    {
        frameCount++;
        timeSinceLastUpdate += Time.unscaledDeltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            float fps = frameCount / timeSinceLastUpdate;
            fpsText.text = $"FPS: {fps:F0}";

            if (fps < 50) Debug.Log(fps);

            // Сброс
            frameCount = 0;
            timeSinceLastUpdate = 0f;
        }
    }
}