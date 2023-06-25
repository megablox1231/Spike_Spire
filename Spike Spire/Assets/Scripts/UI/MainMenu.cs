using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Handles input from main menu ui and plays opening cutscene.
/// </summary>
public class MainMenu : MonoBehaviour {

    public SaveSystem saveSystem;
    public BootStrap bootStrap;

    [SerializeField]
    Animator bgAnim;
    [SerializeField]
    AnimationClip opCutscene;
    [SerializeField]
    Image uiImage;
    [SerializeField]
    Button continueButton;
    [SerializeField]
    GameObject buttonGroup;
    [SerializeField]
    GameObject controlsOptionsMenu;

    PlayerControls controls;
    Action<InputAction.CallbackContext> skipAction;
    Coroutine cutsceneCoroutine;

    void Awake() {
        // lamda skipAction saved for removal later
        controls = new PlayerControls();
        skipAction = ctx => StartCoroutine(SkipOpening());
        controls.Gameplay.AnyInput.performed += skipAction;
        // TODO: comment when making webgl build
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        //string[] subs;
        //input.onEndEdit.AddListener((string text) => { subs = text.Split(' '); bootStrap.startScene = subs[0]; bootStrap.Load(subs[1]); });
    }

    void Start() {
        controlsOptionsMenu.SetActive(false);

        cutsceneCoroutine = StartCoroutine(WaitForOpeningScene());

        saveSystem = new SaveSystem();
        saveSystem.Load();
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void Continue() {
        SaveSystem.SaveData data = saveSystem.GetData();
        bootStrap.startScene = data.area;
        bootStrap.Load(data.level, data.hasForwardSlash);
    }

    public void NewGame() {
        bootStrap.startScene = "Area_1";
        bootStrap.Load("CamTrigger0", false);
    }

    IEnumerator WaitForOpeningScene() {
        buttonGroup.SetActive(false);
        yield return new WaitForSeconds(opCutscene.length);
        yield return uiImage.DOFade(1, 1).WaitForCompletion();
        buttonGroup.SetActive(true);
        continueButton.Select();
        controls.Gameplay.AnyInput.performed -= skipAction;
    }

    IEnumerator SkipOpening() {
        if (!buttonGroup.activeSelf && cutsceneCoroutine != null) {
            StopCoroutine(cutsceneCoroutine);
            yield return new WaitForSeconds(0.01f);
            bgAnim.SetBool("skipOpening", true);
            uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, 1);
            buttonGroup.SetActive(true);
            continueButton.Select();
            controls.Gameplay.AnyInput.performed -= skipAction;
        }
    }

    void OnEnable() {
        controls.Gameplay.Enable();
    }

    void OnDisable() {
        controls.Gameplay.Disable();
    }
}
