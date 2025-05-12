using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GuideMenu : MonoBehaviour
{
    public void ShowGuide()
    {
        Debug.Log("���� ������!");

    }
    public string levelToLoad, levelToLoad1;
    [SerializeField] private RawImage _img;
    [SerializeField] private float _x, _y;
    private void Start()
    {

    }
    [SerializeField] private float scrollSpeed = 0.01f;
    [SerializeField] private Vector2 direction = new Vector2(1f, 0f);

    private void Update()
    {
        Vector2 offset = direction.normalized * scrollSpeed * Time.deltaTime;
        _img.uvRect = new Rect(_img.uvRect.position + offset, _img.uvRect.size);
    }

    public void PlayPressed()
    {
        SceneManager.LoadScene(levelToLoad);
    }
    public void SettingLoad()
    {
        Debug.Log("Setting is entered");
        SceneManager.LoadScene("levelToLoad");
    }
    public void ExitGame()
    {
        Debug.Log("game is exited");
        Application.Quit();
    }
}

