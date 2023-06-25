using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Controls flying platform that moves upon player
/// standing on top.
/// </summary>
public class FlyingPlatform : RaycastControllerSimple {
	[SerializeField] LayerMask passengerMask;
	[SerializeField] float xSpeed, ySpeed;
	[SerializeField] bool isTimed;
	[SerializeField] float timer;

    // the current player gameobject
    Transform target;
	PlayerInput targetInput;
	Controller2D targetController;

	Animator animator;
	SpriteRenderer sprite;
    AudioSource audioSrc;
    bool hasPassenger, hadPassengerLastFrame, timerStarted;

    Tweener pitchUp;
	Tweener pitchDown;

	public override void Start() {
		base.Start();
		animator = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer>();
		audioSrc = GetComponent<AudioSource>();
    }

    void Update() {

		UpdateRaycastOrigins();
		CheckPassenger();

		if (hasPassenger) {
            animator.SetBool("hasPassenger", true);
            StartBuzzing();

			// start battery timer
            if (isTimed && !timerStarted) {
				timerStarted = true;
				StartCoroutine(BatteryTimer());
			}

			Vector3 velocity = CalculatePlatformMovement();
			//Move platform first if it has downward velocity
			if (targetInput.holdingDown) {
				transform.Translate(velocity);
				MovePassenger(velocity);
			}
			else {
				MovePassenger(velocity);
				transform.Translate(velocity);
            }
		}
		else {
            animator.SetBool("hasPassenger", false);
			if (hadPassengerLastFrame) {
				StopBuzzing();
			}
        }

		hadPassengerLastFrame = hasPassenger;
	}

    Vector3 CalculatePlatformMovement() {
        // Platform can move left or right depending on player position
        float newX = transform.position.x;
		if (Mathf.Abs(target.position.x - transform.position.x) > 0.4) {
			newX = Mathf.MoveTowards(transform.position.x, target.position.x, Time.deltaTime * xSpeed);
		}
		float newY = Mathf.MoveTowards(transform.position.y, target.position.y, Time.deltaTime * ySpeed);
		Vector3 newPos = new Vector3(newX, newY, transform.position.z);

        Vector3 velocity = newPos - transform.position;
        if (targetInput.holdingDown) {
            velocity = new Vector3(velocity.x, -1 * velocity.y, velocity.z);
        }
        return velocity;
    }

    void MovePassenger(Vector3 velocity) {
		if (!GameMaster.gm.Restarting) {
			targetController.Move(velocity, true);
		}
	}

    void CheckPassenger() {
        float rayLength = 0.1f + skinWidth;
        for (int i = 0; i < verticalRayCount; i++) {
            Vector2 rayOrigin = raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

            if (hit && hit.distance != 0) {
				hasPassenger = true;

				//initializing target now to avoid doing it before player has respawned
				if (target == null) {
					target = GameMaster.gm.GetCurPlayer().transform;
				}
				targetInput = target.GetComponent<PlayerInput>();
				targetController = target.GetComponent<Controller2D>();

				return;
            }
			else {
				hasPassenger = false;
            }
        }
    }

	IEnumerator BatteryTimer() {
		yield return new WaitForSeconds(timer);

		animator.SetBool("lowBattery", true);
        audioSrc.DOPitch(0, 0.8f);
		sprite.DOColor(Color.black, 0.8f);
		yield return new WaitForSeconds(0.8f);

		enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;
	}

	void StartBuzzing() {
		if (!audioSrc.isPlaying) {
            audioSrc.Play();
            audioSrc.DOPitch(0.6f, 0.5f).SetAutoKill(false).SetRecyclable(true);
		}
	}

	void StopBuzzing() {
        audioSrc.DOPitch(0, 0.5f).SetAutoKill(false).SetRecyclable(true).OnComplete(() => {
            audioSrc.Stop();
        });
    }
}
