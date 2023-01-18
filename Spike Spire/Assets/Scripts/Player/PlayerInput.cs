using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices.ComTypes;
using System;
using Doublsb.Dialog;

/// <summary>
/// Initialized instance of PlayerControls, controls, and listens for feedback from input device to perform all movements and actions.
/// Also plays sounds associated with actions.
/// When jumping or slashing, their respective methods are called and their colliders are set to active.
/// </summary>

[RequireComponent (typeof (PlayerMovement))]
public class PlayerInput : MonoBehaviour {

    public float jumpDelay;

    [HideInInspector]
    public bool hasForwardSlash;
    [HideInInspector]
    public bool isSwordJumping;
    [HideInInspector]
    public bool holdingDown;

    PlayerControls controls;
	PlayerMovement player;
    Controller2D controller;
    Animator animator;
    ForwardSlashAbility fSlashAbility;

    float directionalInput;
    float prevTime;

    Action DialogAction;
    DialogManager dialogManager;

    void Awake() {
        controls = new PlayerControls();
        controls.Gameplay.Jump.performed += ctx => JumpInputDown();
        controls.Gameplay.Jump.canceled += ctx => JumpInputUp();

        controls.Gameplay.Move.performed += ctx => directionalInput = ctx.ReadValue<float>();
        controls.Gameplay.Move.canceled += ctx => directionalInput = 0;

        controls.Gameplay.ForwardSlash.performed += ctx => ForwardSlashInputDown();

        controls.Gameplay.Down.performed += ctx => holdingDown = true;
        controls.Gameplay.Down.canceled += ctx => holdingDown = false;
    }

    void Start () {
		player = GetComponent<PlayerMovement> ();
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        fSlashAbility = GetComponentInChildren<ForwardSlashAbility>();

        hasForwardSlash = GameMaster.gm.playerHasForwardSlash;
        prevTime = Time.time;
	}

	void Update () {
        player.SetDirectionalInput(new Vector2(directionalInput, 0));
    }

    private void JumpInputDown() {
        //Time delay ensures jump can't be spammed
        if (Time.time - prevTime >= jumpDelay) {
            player.OnJumpInputDown();
            if (!controller.collisions.below && !PauseMenu.gamePaused) {
                isSwordJumping = true;
                animator.Play("JumpSword");
                StartCoroutine(FlashJumpCollider());
            }
            prevTime = Time.time;
        }
    }

    private void JumpInputUp() {
        player.OnJumpInputUp();
    }

    private void ForwardSlashInputDown() {
        if (hasForwardSlash) {
            if (directionalInput != 0) {
                animator.Play("FSlash Run");
            }
            else {
                animator.Play("FSlash Idle");
            }
            StartCoroutine(fSlashAbility.DoForwardSlash());
        }
    }


    //Flashes the SpriteRenderer of the JumpCollider for a breif period of time
    private IEnumerator FlashJumpCollider() {
        yield return new WaitForSeconds(.2f);
        isSwordJumping = false;

    }

    private void OnEnable() {
        controls.Gameplay.Enable();
    }

    private void OnDisable() {
        controls.Gameplay.Disable();
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

    public Controller2D GetController2D() {
        return controller;

    }

    public void ConnectDialog(Action Dialog, DialogManager manager) {
        DialogAction = Dialog;
        dialogManager = manager;
        controls.Gameplay.Interact.performed += StartDialog;
    }

    public void DisconnectDialog() {
        controls.Gameplay.Interact.performed -= StartDialog;
    }

    private void StartDialog(InputAction.CallbackContext ctx) {
        DialogAction();
        controls.Gameplay.Interact.performed -= StartDialog;
        controls.Gameplay.Interact.performed += ContinueDialog;
        DisableMovement();
    }

    private void ContinueDialog(InputAction.CallbackContext ctx) {
        dialogManager.Click_Window();
    }
}
