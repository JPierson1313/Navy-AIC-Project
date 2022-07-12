using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuSystems : MonoBehaviour
{
    [Header("Pause Menu Systems")]
    [SerializeField] string startScreenName;
    [SerializeField] GameObject pauseMenuCanvas;
    private int pauseNum = 0;

    private void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            pauseNum++;
        }

        switch (pauseNum)
        {
            case 1:
                Time.timeScale = 0.0f;
                pauseMenuCanvas.SetActive(true);
                break;
            case 2:
                Time.timeScale = 1;
                pauseMenuCanvas.SetActive(false);
                pauseNum = 0;
                break;
        }
    }

    public void ResumeButton()
    {
        Time.timeScale = 1;
        pauseNum = 0;
        pauseMenuCanvas.SetActive(false);
    }

    public void RestartButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitButton()
    {
        Time.timeScale = 1;
        pauseNum = 0;
        SceneManager.LoadScene(startScreenName);
    }
}
