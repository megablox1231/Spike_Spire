using System.Collections;
using UnityEngine;

/// <summary>
/// Handles sequence of events after completing first set of dialog
/// in the final area.
/// </summary>
public class FinalDialogEvent : MonoBehaviour {

    [SerializeField] Animator elevatorAnimator;
    [SerializeField] float beforeRunDelay;
    [SerializeField] Animator travellerAnimator;
    [SerializeField] string runStateName;
    [SerializeField] string idleStateName;
    [SerializeField] Transform runTarget;
    [SerializeField] float runSpeed;
    [SerializeField] float beforeDialogDelay;
    [SerializeField] Dialog dialogTrigger;
    [SerializeField] string dialogSection;

    bool running;

    void Update() {
        if (running == true) {
            travellerAnimator.transform.position = Vector3.MoveTowards(travellerAnimator.transform.position, runTarget.position, runSpeed * Time.deltaTime);
            if (travellerAnimator.transform.position == runTarget.position) {
                running = false;
                travellerAnimator.Play(idleStateName);
                StartCoroutine(BeginDialog());
            }
        }
    }

    // called by first dialog's OnEnd method
    public void EventSequence() {
        StartCoroutine(TravellerArrival());
    }

    IEnumerator TravellerArrival() {
        GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().DisableMovement();
        elevatorAnimator.enabled = true;
        yield return new WaitForSeconds(beforeRunDelay);
        travellerAnimator.Play(runStateName);
        running = true;
    }

    IEnumerator BeginDialog() {
        yield return new WaitForSeconds(beforeDialogDelay);
        dialogTrigger.StartDialog(dialogSection);
    }
}
