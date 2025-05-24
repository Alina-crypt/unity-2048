using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeartSystem : MonoBehaviour
{
    public GameObject[] hearts; 
    private int life;
    GameManager gameManager;

    private void Start()
    {
        life = hearts.Length;  
    }

    public void LoseLife()
    {
        if (life <= 0)
            return;

        life--;

        if (hearts[life] != null)
        {
            hearts[life].SetActive(false);
        }

        if (life <= 0)
        {
            GameOver();
        }
    }

    public void TryAgain() 
    {
        life = hearts.Length;

        foreach (GameObject heart in hearts)
        {
            heart.SetActive(true);
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        
    }
}
