using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TimeController timeController;

    public static bool GameIsComplete = false;
    public static bool GameIsOver = false;

    public GameObject gameCompleteUI;
    public GameObject gameOverUI;

    public void GameComplete()
    {
        gameCompleteUI.SetActive(true);
        timeController.EndTimer();
        GameIsComplete = true;
        StartCoroutine(ExecuteAfterTime());
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        timeController.EndTimer();
        GameIsOver = true;
        StartCoroutine(ExecuteAfterTime());
    }

    private IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Env"); //Load by Scene name
    }
}
