using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPlatform : RaycastControllerSimple
{
	public LayerMask passengerMask;
	public float xSpeed, ySpeed;
	public bool isTimed;
	public float timer;

	public Transform target; //TODO: make private and remove unneeded code when done testing
	PlayerInput targetInput;
	bool hasPassenger, timerStarted;

	public override void Start() {
		base.Start();
	}

	void Update() {

		UpdateRaycastOrigins();

		CheckPassenger();
		if (hasPassenger) {
			if (isTimed && !timerStarted) {
				timerStarted = true;
				StartCoroutine("restartTimer");
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
	}

    Vector3 CalculatePlatformMovement() {
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
		targetInput.GetController2D().Move(velocity, true);
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

				return;
            }
			else {
				hasPassenger = false;
            }
        }
    }

	IEnumerator restartTimer() {
		yield return new WaitForSeconds(timer);

		GetComponent<SpriteRenderer>().color = Color.red;
		yield return new WaitForSeconds(0.8f);
		Destroy(gameObject);
	}
}
