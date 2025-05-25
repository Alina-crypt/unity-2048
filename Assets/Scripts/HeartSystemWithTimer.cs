using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class HeartSystemWithTimer : MonoBehaviour
{
    [Header("Ссылки")]
    public Image[] hearts; 
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public TextMeshProUGUI timerText;

    [Header("Настройки")]
    public float restoreInterval = 300f; 

    public int maxLives;
    public int currentLives;
    public DateTime nextLifeTime;
    public bool isRestoring;

    private const string LivesKey = "Lives";
    private const string NextLifeKey = "NextLife";

    public bool HasLives()
    {
        return currentLives > 0;
    }

    void Start()
    {
        maxLives = hearts.Length;
        LoadState();
        UpdateHearts();
    }

    void Update()
    {
        if (currentLives < maxLives)
        {
            TimeSpan timeLeft = nextLifeTime - DateTime.Now;

            if (timeLeft <= TimeSpan.Zero)
            {
                currentLives++;
                ActivateHeart(currentLives - 1);

                if (currentLives < maxLives)
                {
                    nextLifeTime = DateTime.Now.AddSeconds(restoreInterval);
                    PlayerPrefs.SetString(NextLifeKey, nextLifeTime.ToString());
                }
                else
                {
                    PlayerPrefs.DeleteKey(NextLifeKey);
                    isRestoring = false;
                }

                PlayerPrefs.SetInt(LivesKey, currentLives);
                PlayerPrefs.Save();
            }
            else
            {
                timerText.text = $"Next life in: {timeLeft.Minutes:00}:{timeLeft.Seconds:00}";
            }
        }
        else
        {
            timerText.text = "Life is full";
        }
    }

    public void LoseLife()
    {
        if (currentLives <= 0)
        {
            return; 
        }

        currentLives--;
        DeactivateHeart(currentLives);

        if (!isRestoring && currentLives < maxLives)
        {
            nextLifeTime = DateTime.Now.AddSeconds(restoreInterval);
            PlayerPrefs.SetString(NextLifeKey, nextLifeTime.ToString());
            isRestoring = true;
        }

        PlayerPrefs.SetInt(LivesKey, currentLives);
        PlayerPrefs.Save();

        if (currentLives <= 0)
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
    }

    public void TryAgain()
    {
        currentLives = maxLives;

        for (int i = 0; i < maxLives; i++)
        {
            hearts[i].sprite = fullHeart;
            hearts[i].gameObject.SetActive(true);
        }

        PlayerPrefs.DeleteKey(NextLifeKey);
        PlayerPrefs.SetInt(LivesKey, currentLives);
        PlayerPrefs.Save();

        isRestoring = false;
        timerText.text = "Life is full";

        
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.NewGame();
        }
    }


    private void GameOver()
    {
        Debug.Log("Game Over");
    }

    private void LoadState()
    {
        currentLives = PlayerPrefs.GetInt(LivesKey, maxLives);

        if (currentLives < maxLives)
        {
            string savedTime = PlayerPrefs.GetString(NextLifeKey, "");
            if (!string.IsNullOrEmpty(savedTime))
            {
                nextLifeTime = DateTime.Parse(savedTime);
                isRestoring = true;
            }
        }
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < currentLives ? fullHeart : emptyHeart;
        }
    }

    private void DeactivateHeart(int index)
    {
        if (index >= 0 && index < hearts.Length)
            hearts[index].sprite = emptyHeart;
    }


    private void ActivateHeart(int index)
    {
        if (index >= 0 && index < hearts.Length)
            hearts[index].sprite = fullHeart;
    }

}
