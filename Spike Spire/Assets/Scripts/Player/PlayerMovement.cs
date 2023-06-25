using UnityEngine;
using System.Collections;

/// <summary>
/// Takes player input, calcuates velocity vector,
/// and handles collisions with death and brittle spikes.
/// Passes on velocity vector to Controller2D.
/// </summary>
[RequireComponent (typeof (Controller2D))]
public class PlayerMovement : MonoBehaviour {

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
    public float moveSpeed = 1.5f;
	public float maxYVelocity = 20;
	public float forwardSlashSpeed = 3;
	public float pauseGravTime; // how long gravity will be paused after forward slashing
	public bool frozen = false; // stop player movement if true

    float accelerationTimeAirborne = .1f;
	float accelerationTimeGrounded = .1f;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;
	Animator animator;
	PlayerAudio playerAudio;

	Vector2 directionalInput;
	bool pauseFrameSkip = false; // skips updating animator conditions for one frame after unpausing because of unpause button slowness

	void Start() {
		controller = GetComponent<Controller2D>();
		animator = GetComponent<Animator>();
		playerAudio = GetComponent<PlayerAudio>();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

	void Update() {
        CalculateVelocity();
		if (!PauseMenu.gamePaused) {
			if (!pauseFrameSkip) {
				animator.SetFloat("Speed", Mathf.Abs(velocity.x));
				animator.SetFloat("Y Speed", Mathf.Abs(velocity.y));
				animator.SetBool("MoveButtonDown", Mathf.Abs(directionalInput.x) > 0);
				animator.SetBool("isGrounded", controller.collisions.below);

				if (!frozen) {
					controller.Move(velocity * Time.deltaTime, directionalInput);
				}
			}
			pauseFrameSkip = false;
		}
		else {
			pauseFrameSkip = true;
        }

		if (controller.collisions.above || controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			} else {
				velocity.y = 0;
			}
		}
	}

	public void SetDirectionalInput(Vector2 input) {
		directionalInput = input;
    }

	public void OnJumpInputDown() {
		animator.SetBool("JumpButtonDown", true);
		if (!GetComponentInChildren<PlayerStats>().invincible && controller.jumpCollider.IsTouchingLayers(controller.deadlySBMask) && !controller.collisions.below) {
            GameMaster.RestartPlayer(gameObject, Vector3.up);
        }

		if (controller.jumpCollider.IsTouchingLayers(controller.SwordJumpMask) || controller.collisions.below) { // part of sword jump mechanic

            Collider2D[] brittles = new Collider2D[8];
            if (controller.jumpCollider.OverlapCollider(controller.brittleContact, brittles) > 0) { // checks if hit a brittle spike block
				playerAudio.BrittleSound();
                foreach(Collider2D brittle in brittles) {
                    if (brittle != null) {
						brittle.transform.parent.GetComponent<Animator>().enabled = true;
                    }
                }
            }
			else if(!controller.collisions.below) { // true if not jumping from ground
                playerAudio.ClashSound();
            }

			if (controller.collisions.slidingDownMaxSlope) {
				if (directionalInput.x != -Mathf.Sign (controller.collisions.slopeNormal.x)) { // not jumping against max slope
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			} else {
				velocity.y = maxJumpVelocity;
			}
		}
		else {
			//sword jump hits nothing
			playerAudio.SwingSound();
		}
	}

    //variable jump height method
	public void OnJumpInputUp() {
		animator.SetBool("JumpButtonDown", false);
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}

    public bool OnForwardSlashCollision() {
        if (!GetComponentInChildren<PlayerStats>().invincible && controller.slashCollider.IsTouchingLayers(controller.deadlySBMask)) {
			Vector2 pushDirection = (controller.collisions.faceDir == 1) ? Vector2.left : Vector2.right;
            GameMaster.RestartPlayer(gameObject, pushDirection);
			return true;
        }

        if (controller.slashCollider.IsTouchingLayers(controller.SlashMask)) {
            Collider2D[] brittles = new Collider2D[8];
            bool hitBrittles = false;
            if (controller.slashCollider.OverlapCollider(controller.brittleContact, brittles) > 0) { //checks if hit a brittle spike block
				playerAudio.BrittleSound();
                hitBrittles = true;
                foreach (Collider2D brittle in brittles) {
                    if (brittle != null) {
						brittle.transform.parent.GetComponent<Animator>().enabled = true;
                    }
                }
            }

            if (!controller.collisions.below && !hitBrittles) {
                playerAudio.ClashSound();
                velocity.x = (controller.collisions.faceDir == 1) ? -forwardSlashSpeed : forwardSlashSpeed;
				velocity.y = 0;	//needed so character does not float up while gravity paused
				StartCoroutine(PauseGravity(pauseGravTime));
            }
			return true;
        }
		return false;
    }

	void CalculateVelocity() {
		if (!frozen) {
			float targetVelocityX = directionalInput.x * moveSpeed;
			velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
			velocity.y += gravity * Time.deltaTime;
			if (Mathf.Abs(velocity.y) > maxYVelocity) {
				velocity.y = maxYVelocity * Mathf.Sign(velocity.y);
			}
		}
	}

	// keeps gravity 0 for pauseTime amount
	IEnumerator PauseGravity(float pauseTime) {
		float tempGrav = gravity;
		gravity = 0;
		yield return new WaitForSeconds(pauseTime);
		gravity = tempGrav;
	}

	public void ResetVelocity() {
		velocity = Vector3.zero;
    }
}
