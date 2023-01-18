using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public static bool gamePaused = false;

    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (gamePaused) {
                Resume();
            }
            else {
                Pause();
            }
        }
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        gamePaused = true;
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        gamePaused = false;
    }

    public void QuitGame() {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
