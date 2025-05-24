using UnityEngine;
using UnityEngine.UI;

public class ThemeDarkeningOverTime : MonoBehaviour
{
    public RawImage overlay;
    public float maxAlpha = 0.6f;         // Максимальная тёмность
    public float timeToMaxDark = 180f;    // Через сколько секунд будет максимальная тёмность

    private float timer = 0f;
    private bool isActive = true;

    void Start()
    {
        if (overlay != null)
            overlay.color = new Color(0f, 0f, 0f, 0f); // Начинаем с прозрачности
    }

    void Update()
    {
        if (!isActive || overlay == null) return;

        timer += Time.deltaTime;

        float progress = Mathf.Clamp01(timer / timeToMaxDark);
        float currentAlpha = Mathf.Lerp(0f, maxAlpha, progress);
        overlay.color = new Color(0f, 0f, 0f, currentAlpha);
    }

    public void ResetTimer()
    {
        timer = 0f;
        if (overlay != null)
            overlay.color = new Color(0f, 0f, 0f, 0f);
    }

    public void SetActive(bool state)
    {
        isActive = state;
    }
}
