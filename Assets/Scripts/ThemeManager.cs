using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    public Image nightOverlayPanel;
    

    private Color transparent = new Color(0, 0, 0, 0);
    private Color dark = new Color(0, 0, 0, 0.4f);
    private Color targetColor;
    private float transitionSpeed = 3f;

    private void Start()
    {
        bool isDark = PlayerPrefs.GetInt("DarkTheme", 0) == 1;

       
        targetColor = isDark ? dark : transparent;
        if (nightOverlayPanel != null)
            nightOverlayPanel.color = targetColor;

        
    }

    private void Update()
    {
        if (nightOverlayPanel != null && nightOverlayPanel.color != targetColor)
        {
            nightOverlayPanel.color = Color.Lerp(nightOverlayPanel.color, targetColor, Time.deltaTime * transitionSpeed);
        }
    }

   
}
