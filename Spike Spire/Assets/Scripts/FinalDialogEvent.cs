using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDialogEvent : MonoBehaviour {

    [SerializeField]
    private Animator elevatorAnimator;
    [SerializeField]
    private float beforeRunDelay;
    [SerializeField]
    private Animator travellerAnimator;
    [SerializeField]
    private string runStateName;
    [SerializeField]
    private string idleStateName;
    [SerializeField]
    private Transform runTarget;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float beforeDialogDelay;
    [SerializeField]
    private Dialog dialogTrigger;
    [SerializeField]
    private string fileSection;

    private bool running;

    private void Update() {
        if (running == true) {
            travellerAnimator.transform.position = Vector3.MoveTowards(travellerAnimator.transform.position, runTarget.position, runSpeed * Time.deltaTime);
            if (travellerAnimator.transform.position == runTarget.position) {
                running = false;
                travellerAnimator.Play(idleStateName);
                StartCoroutine(BeginDialog());
            }
        }
    }

    public void EventSequence() {
        StartCoroutine(TravellerArrival());
    }

    private IEnumerator TravellerArrival() {
        GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().DisableMovement();
        elevatorAnimator.enabled = true;
        yield return new WaitForSeconds(beforeRunDelay);
        travellerAnimator.Play(runStateName);
        running = true;
    }

    private IEnumerator BeginDialog() {
        yield return new WaitForSeconds(beforeDialogDelay);
        dialogTrigger.StartDialog(fileSection);
    }
}
