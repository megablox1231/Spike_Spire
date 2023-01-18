using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Slimmer version of the Elongate script with just
/// the MoveAndScale method. Used for ElongatingCloserSpikes.
/// ---DEPRECATED---
/// </summary>
public class ElongateLite : MonoBehaviour
{
    public float timeToMove;
    public float startWaitTime; //wait time before elongating
    public Transform endTransform;

    //Wrapper so MoveAndScale can be called by EventTrigger.
    public void Elongate() {
        StartCoroutine(MoveAndScale());
    }

    private IEnumerator MoveAndScale() {
        yield return new WaitForSeconds(startWaitTime);

        var position = endTransform.position;
        var scale = endTransform.localScale;

        var currentPos = transform.position;
        var currentScale = transform.localScale;
        var t = 0f;

        while (t < 1) {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            transform.localScale = Vector3.Lerp(currentScale, scale, t);
            yield return null;
        }
    }
}
