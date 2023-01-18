using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public SaveSystem saveSystem;
    public BootStrap bootStrap;

    [SerializeField]
    private Animator bgAnim;
    [SerializeField]
    private AnimationClip opCutscene;
    [SerializeField]
    private Image uiImage;
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private GameObject buttonGroup;

    PlayerControls controls;
    bool skipOpening;
    private Action<InputAction.CallbackContext> skipAction;
    Coroutine cutsceneCoR;

    void Awake() {
        Application.targetFrameRate = 60;

        // lamda skipAction saved for removal later
        controls = new PlayerControls();
        skipAction = ctx => StartCoroutine(SkipOpening());
        controls.Gameplay.AnyInput.performed += skipAction;
    }

    void Start() {
        cutsceneCoR = StartCoroutine(WaitForOpeningScene());

        saveSystem = new SaveSystem();
        saveSystem.Load();
    }

    public void QuitGame() {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void Continue() {
        SaveSystem.SaveData data = saveSystem.GetData();
        bootStrap.startScene = data.area;
        bootStrap.Load(data.level);
    }

    public void NewGame() {
        bootStrap.startScene = "Area_1";
        bootStrap.Load("CamTrigger0");
    }

    private IEnumerator WaitForOpeningScene() {
        uiImage.enabled = false;
        buttonGroup.SetActive(false);
        yield return new WaitForSeconds(opCutscene.length);
        uiImage.enabled = true;
        buttonGroup.SetActive(true);
        continueButton.Select();
    }

    private IEnumerator SkipOpening() {
        Debug.Log("skipped");
        if (!uiImage.enabled && cutsceneCoR != null) {
            StopCoroutine(cutsceneCoR);
            //bgAnim.CrossFade("OP Cutscene End", 0.5f);
            yield return new WaitForSeconds(0.01f);
            bgAnim.SetBool("skipOpening", true);
            Debug.Log("Huh");
            uiImage.enabled = true;
            buttonGroup.SetActive(true);
            continueButton.Select();
            controls.Gameplay.AnyInput.performed -= skipAction;
        }
    }

    private void OnEnable() {
        controls.Gameplay.Enable();
    }

    private void OnDisable() {
        controls.Gameplay.Disable();
    }
}
