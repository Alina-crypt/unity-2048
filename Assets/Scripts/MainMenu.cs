using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SceneTransition transition;
    public AudioManager audioManager;
    public void playGame()

    {
        
        Debug.Log("Play button clicked");
        transition.FadeToScene("2048game2");
    }

    public void settingsGame()
    {
        Debug.Log("Settings button clicked");
        transition.FadeToScene("Settings");
    }

    public void exitGame()
    {
       
        Debug.Log("Exit button clicked");
        Application.Quit();
    }
}
