using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallEvent : MonoBehaviour
{

    public AnimationClip anim;
    public Animator[] reactiveObjects;

    PlayerInput player;
    Animator animator;
    Controller2D controller;
    bool falling;
    bool returnControl;

    void Start() {
        player = GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>();
        animator = player.GetComponent<Animator>();
        controller = player.GetComponent<Controller2D>();
    }

    // Update is called once per frame
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

    private void OnTriggerEnter2D() {
        player.enabled = false;
        falling = true;
    }

    private IEnumerator WaitForAnimation() {
        yield return new WaitForSeconds(anim.length);
        returnControl = true;
    }
}
