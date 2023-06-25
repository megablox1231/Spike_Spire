using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices.ComTypes;
using System;
using Doublsb.Dialog;

/// <summary>
/// Initialized instance of PlayerControls, controls, and listens for feedback from input device to perform all movements and actions.
/// Also plays some sounds associated with actions.
/// When jumping or slashing, their respective methods are called and their colliders are set to active.
/// </summary>

[RequireComponent (typeof (PlayerMovement))]
public class PlayerInput : MonoBehaviour {

    [SerializeField] float jumpCooldown;
    [SerializeField] float fSlashCooldown;

    [HideInInspector] public bool hasForwardSlash;
    [HideInInspector] public bool isSwordJumping;
    [HideInInspector] public bool holdingDown;

    PlayerControls controls;
	PlayerMovement player;
    Controller2D controller;
    CameraControl camControl;
    Animator animator;
    ForwardSlashAbility fSlashAbility;

    Vector2 cameraInput;
    float directionalInput;
    float prevJumpTime;
    float prevFSlashTime;

    Action DialogAction;
    DialogManager dialogManager;

    Action<InputAction.CallbackContext> ViewLevelAction;

    void Start () {
		player = GetComponent<PlayerMovement> ();
        controller = GetComponent<Controller2D>();
        camControl = GetComponent<CameraControl>();
        animator = GetComponent<Animator>();
        fSlashAbility = GetComponentInChildren<ForwardSlashAbility>();

        hasForwardSlash = GameMaster.gm.PlayerHasForwardSlash;
        prevJumpTime = Time.time;
        prevFSlashTime = Time.time;
	}

	void Update () {
        player.SetDirectionalInput(new Vector2(directionalInput, 0));
        camControl.input = cameraInput;
    }

    void JumpInputDown(InputAction.CallbackContext context) {
        //Time delay ensures jump can't be spammed
        if (Time.time - prevJumpTime >= jumpCooldown) {
            player.OnJumpInputDown();
            if (!controller.collisions.below && !PauseMenu.gamePaused && !GameMaster.gm.Restarting) {
                isSwordJumping = true;
                animator.Play("JumpSword");
                StartCoroutine(DelayedSwordJumpEnd());
            }
            prevJumpTime = Time.time;
        }
    }

    void JumpInputUp(InputAction.CallbackContext context) {
        player.OnJumpInputUp();
    }

    void ForwardSlashInputDown(InputAction.CallbackContext context) {
        if (hasForwardSlash && Time.time - prevFSlashTime >= fSlashCooldown) {
            if (directionalInput != 0) {
                animator.Play("FSlash Run");
            }
            else {
                animator.Play("FSlash Idle");
            }
            StartCoroutine(fSlashAbility.DoForwardSlash());
            prevFSlashTime = Time.time;
        }
    }

    IEnumerator DelayedSwordJumpEnd() {
        yield return new WaitForSeconds(.2f);
        isSwordJumping = false;
    }

    void OnEnable() {
        controls = InputManager.GetInputActions();
        controls.Gameplay.Jump.performed += JumpInputDown;
        controls.Gameplay.Jump.canceled += JumpInputUp;

        controls.Gameplay.Move.performed += ctx => directionalInput = ctx.ReadValue<float>();
        controls.Gameplay.Move.canceled += ctx => directionalInput = 0;

        controls.Gameplay.ForwardSlash.performed += ForwardSlashInputDown;

        controls.Gameplay.Down.performed += ctx => holdingDown = true;
        controls.Gameplay.Down.canceled += ctx => holdingDown = false;

        controls.Gameplay.MoveCamera.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.MoveCamera.canceled += ctx => cameraInput = Vector2.zero;

        controls.Gameplay.Enable();
    }

    void OnDisable() {
        controls.Gameplay.Jump.performed -= JumpInputDown;
        controls.Gameplay.Jump.canceled -= JumpInputUp;

        controls.Gameplay.Move.performed -= ctx => directionalInput = ctx.ReadValue<float>();
        controls.Gameplay.Move.canceled -= ctx => directionalInput = 0;

        controls.Gameplay.ForwardSlash.performed -= ForwardSlashInputDown;

        controls.Gameplay.Down.performed -= ctx => holdingDown = true;
        controls.Gameplay.Down.canceled -= ctx => holdingDown = false;

        controls.Gameplay.MoveCamera.performed -= ctx => cameraInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.MoveCamera.canceled -= ctx => cameraInput = Vector2.zero;

        DisconnectLevelViwer();
        DisconnectDialog();
        controls.Gameplay.Disable();

        if (player != null) {
            player.SetDirectionalInput(new Vector2(0, 0)); // disabled so no more directional input
        }
        if (camControl != null) {
            camControl.input = Vector2.zero;
        }
    }

    public void DisableMovement() {
        controls.Gameplay.Jump.Disable();
        controls.Gameplay.ForwardSlash.Disable();
        controls.Gameplay.Move.Disable();
    }

    public void EnableMovement() {
        controls.Gameplay.Jump.Enable();
        controls.Gameplay.ForwardSlash.Enable();
        controls.Gameplay.Move.Enable();
    }

    public void ConnectDialog(Action Dialog, DialogManager manager) {
        DialogAction = Dialog;
        dialogManager = manager;
        controls.Gameplay.Interact.performed += StartDialog;
    }

    // Connects dialog action that skips the start dialog portion
    public void ConnectContinueDialog(Action Dialog, DialogManager manager) {
        DialogAction = Dialog;
        dialogManager = manager;
        controls.Gameplay.Interact.performed += ContinueDialog;
    }

    public void DisconnectDialog() {
        if (DialogAction != null) {
            controls.Gameplay.Interact.performed -= StartDialog;
            controls.Gameplay.Interact.performed -= ContinueDialog;
            DialogAction = null;
        }
    }

    void StartDialog(InputAction.CallbackContext ctx) {
        if (PauseMenu.gamePaused) { return; }

        DialogAction();
        controls.Gameplay.Interact.performed -= StartDialog;
        controls.Gameplay.Interact.performed += ContinueDialog;
        DisableMovement();
    }

    void ContinueDialog(InputAction.CallbackContext ctx) {
        if (!PauseMenu.gamePaused) {
            dialogManager.Click_Window();
        }
    }

    public void ConnectLevelViewer(Action<InputAction.CallbackContext> ViewLevel) {
        ViewLevelAction = ViewLevel;
        controls.Gameplay.Interact.performed += ViewLevelAction;
    }

    public void DisconnectLevelViwer() {
        if (ViewLevelAction != null) {
            controls.Gameplay.Interact.performed -= ViewLevelAction;
            ViewLevelAction = null;
        }
    }
}
