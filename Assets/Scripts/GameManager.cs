using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup gameOver;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;
    AudioManager audioManager;
    public HeartSystemWithTimer heartSystem;
     bool isGameOver = false;
    private int score;
    

    private void Start()
    {
        LoadGame();
    }
    public bool CanPlay()
    {
        return heartSystem != null && heartSystem.HasLives();
    }

    public void NewGame()
    {
        if (heartSystem != null && !heartSystem.HasLives())
        {
            Debug.Log("Нет жизней! Ждите восстановления.");
            return;
        }
        AudioManager.Instance.PlayClick();
        SetScore(0);
        hiscoreText.text = LoadHiscore().ToString();
        gameOver.alpha = 0f;
        gameOver.interactable = false;
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
        PlayerPrefs.SetInt("isGameOver", 0);


    }


    public void OnButtonClick()
    {
        if (heartSystem == null)
            return;

        if (heartSystem.HasLives())
        {
            SaveGame(); // сохраняем перед перезапуском
            NewGame();
            heartSystem.LoseLife();
        }
        else
        {
            Debug.Log("You have run out of lives! Wait for recovery.");
        }
    }



    public void GameOver()
    {
        AudioManager.Instance.PlayGameOver();
        board.enabled = false;
        gameOver.interactable = true;
        StartCoroutine(Fade(gameOver, 1f, 1f));
        PlayerPrefs.SetInt("isGameOver", 1);

    }



    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds(delay);
        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
        SaveHiscore();
    }

    private void SaveHiscore()
    {
        int hisCore = LoadHiscore();
        if (score > hisCore)
        {
            PlayerPrefs.SetInt("hiscore", score);
        }
    }

    private int LoadHiscore()
    {
        return PlayerPrefs.GetInt("hiscore", 0);
    }
    public void SaveGame()
    {
        PlayerPrefs.SetInt("score", score);

        if (heartSystem != null)
        {
            PlayerPrefs.SetInt("lives", heartSystem.currentLives);
            PlayerPrefs.SetString("nextLifeTime", heartSystem.nextLifeTime.ToString());
        }
        string boardData = board.SerializeBoard();
        PlayerPrefs.SetString("board", boardData);

        PlayerPrefs.Save();
        PlayerPrefs.SetInt("isGameRunning", 1);

    }

    public void LoadGame()
    {
        // Загружаем счёт
        if (PlayerPrefs.HasKey("score"))
        {
            SetScore(PlayerPrefs.GetInt("score"));
        }

        // Загружаем поле
        if (PlayerPrefs.HasKey("board"))
        {
            board.DeserializeBoard(PlayerPrefs.GetString("board"));
        }

        // Жизни
        string savedTime = PlayerPrefs.GetString("nextLifeTime", "");
        if (!string.IsNullOrEmpty(savedTime))
        {
            if (DateTime.TryParse(savedTime, out DateTime parsedTime))
            {
                heartSystem.nextLifeTime = parsedTime;
                heartSystem.isRestoring = heartSystem.currentLives < heartSystem.maxLives;
            }
            else
            {
                Debug.LogWarning("Не удалось распарсить дату восстановления жизни.");
                heartSystem.nextLifeTime = DateTime.Now.AddSeconds(heartSystem.restoreInterval);
                heartSystem.isRestoring = true;
            }
        }

        // Спрятать GameOver панель, если она была видна
        gameOver.alpha = 0f;
        gameOver.interactable = false;

        // Включить игровую логику
        board.enabled = true;
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

}
