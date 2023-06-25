using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls pause menu logic and input.
/// </summary>
public class PauseMenu : MonoBehaviour {

    public static bool gamePaused = false;

    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] Button resumeButton;

    PlayerControls controls;

    void OnEnable() {
        controls = InputManager.GetInputActions();
        controls.Gameplay.Pause.performed += PauseToggle;
    }

    void OnDisable() {
        controls.Gameplay.Pause.performed -= PauseToggle;
    }

    void PauseToggle(InputAction.CallbackContext context) {
        if (gamePaused) {
            Resume();
        }
        else {
            pauseMenuUI.SetActive(true);
            resumeButton.Select();
            Time.timeScale = 0;
            GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().DisableMovement();
            gamePaused = true;
        }
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().EnableMovement();
        gamePaused = false;
    }

    // Destroys "DontDestroyOnLoad" objects before loading main menu
    public void QuitGame() {
        var temp = GameObject.FindGameObjectWithTag("MainCamera");
        if (temp != null) {
            Destroy(temp);
        }
        temp = GameObject.FindGameObjectWithTag("VirtualCam");
        if (temp != null) {
            Destroy(temp);
        }
        temp = GameObject.FindGameObjectWithTag("GameController");
        if (temp != null) {
            Destroy(temp);
        }
        temp = GameObject.FindGameObjectWithTag("Respawn");
        if (temp != null) {
            Destroy(temp);
        }
        temp = GameObject.FindGameObjectWithTag("Player");
        if (temp != null) {
            // set temp to parent if temp is the player collider child
            temp = (temp.transform.parent != null && temp.transform.parent.tag == "Player") ? temp.transform.parent.gameObject : temp;
            Destroy(temp);
        }

        Time.timeScale = 1;
        gamePaused = false;

        SceneManager.LoadScene(0);
    }
}
