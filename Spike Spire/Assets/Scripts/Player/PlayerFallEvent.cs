using System.Collections;
using UnityEngine;

/// <summary>
/// Disables player controls then plays animations and
/// reenables contrls upon player colliding with floor.
/// </summary>
public class PlayerFallEvent : MonoBehaviour {

    public AnimationClip anim;
    public Animator[] reactiveObjects;

    PlayerInput player;
    Animator animator;
    Controller2D controller;
    bool falling;
    bool returnControl;

    void Update() {
        if (falling && controller.collisions.below) {
            animator.Play(anim.name);
            for (int i = 0; i < reactiveObjects.Length; i++) {
                reactiveObjects[i].Play("Landing Event");
            }
            falling = false;
            StartCoroutine(WaitForAnimation());
        }
        if (returnControl) {
            player.enabled = true;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Player") {
            player = collider.transform.parent.GetComponent<PlayerInput>();
            animator = player.GetComponent<Animator>();
            controller = player.GetComponent<Controller2D>();
            player.enabled = false;
            player.GetComponent<PlayerMovement>().ResetVelocity();
            falling = true;
        }
    }

    IEnumerator WaitForAnimation() {
        yield return new WaitForSeconds(anim.length);
        returnControl = true;
    }
}
