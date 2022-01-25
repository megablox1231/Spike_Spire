using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/// <summary>
/// Initialized instance of PlayerControls, controls, and listens for feedback from input device to perform all movements and actions.
/// When jumping or slashing, their respective methods are called and their colliders are set to active.
/// </summary>

[RequireComponent (typeof (PlayerMovement))]
public class PlayerInput : MonoBehaviour {

    public float jumpDelay;

    [HideInInspector]
    public bool hasForwardSlash;
    [HideInInspector]
    public bool isJumping;

    PlayerControls controls;
	PlayerMovement player;
    Controller2D controller;
    ForwardSlashAbility fSlashAbility;

    float directionalInput;
    float prevTime;

    void Awake() {
        controls = new PlayerControls();
        controls.Gameplay.Jump.performed += ctx => JumpInputDown();
        controls.Gameplay.Jump.canceled += ctx => JumpInputUp();

        controls.Gameplay.Move.performed += ctx => directionalInput = ctx.ReadValue<float>();
        controls.Gameplay.Move.canceled += ctx => directionalInput = 0;

        controls.Gameplay.ForwardSlash.performed += ctx => ForwardSlashInputDown();
    }

    void Start () {
		player = GetComponent<PlayerMovement> ();
        controller = GetComponent<Controller2D>();
        fSlashAbility = GetComponentInChildren<ForwardSlashAbility>();

        hasForwardSlash = GameMaster.gm.playerHasForwardSlash;
        prevTime = Time.time;
	}

	void Update () {
        player.SetDirectionalInput(new Vector2(directionalInput, 0));
    }

    private void JumpInputDown() {
        if (Time.time - prevTime >= jumpDelay) {
            isJumping = true;
            Debug.Log("isJumping true");
            //TODO: disabling and enabling jump collider may break things
            //controller.jumpCollider.enabled = true;
            player.OnJumpInputDown();
            StartCoroutine(FlashJumpCollider());
            prevTime = Time.time;
        }
    }

    private void JumpInputUp() {
        player.OnJumpInputUp();
    }

    private void ForwardSlashInputDown() {
        if (hasForwardSlash) {
            StartCoroutine(fSlashAbility.DoForwardSlash());
        }
    }


    //Flashes the SpriteRenderer of the JumpCollider for a breif period of time
    private IEnumerator FlashJumpCollider() {
        SpriteRenderer box = controller.jumpCollider.GetComponent<SpriteRenderer>();
        box.enabled = true;
        yield return new WaitForSeconds(.4f);
       // controller.jumpCollider.enabled = false;
        box.enabled = false;
        isJumping = false;
        Debug.Log("isJumping false");
    }

    private void OnEnable() {
        controls.Gameplay.Enable();
    }

    private void OnDisable() {
        controls.Gameplay.Disable();
    }
}
