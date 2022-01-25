using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElongateMult : MonoBehaviour
{
    public float timeToMove;
    public float startWaitTime; //wait time before elongating
    public Transform endTransform;

    public Transform stopper;

    bool stopperGone;

    void Start() {

    }

    void Update() {
    }

    public IEnumerator MoveAndScale(Vector3 position, Vector3 scale, float timeToMove) {
        var currentPos = transform.position;
        var currentScale = transform.localScale;
        var t = 0f;

        if (stopper == null) {
            stopperGone = true;
        }
        while (t < 1) {
            t += Time.deltaTime / timeToMove;
            if (stopperGone) {
                transform.position = Vector3.Lerp(currentPos, position, t);
                transform.localScale = Vector3.Lerp(currentScale, scale, t);
            }
            yield return null;
        }
    }

    //Corountine that will wait.
    private IEnumerator Wait(float time) {
        yield return new WaitForSeconds(time);
    }
}
