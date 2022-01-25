using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elongate : MonoBehaviour
{
    public float initWait;  //wait time before 1st elongation
    public Transform stopper;

    private bool stopperGone = true;

    void Start()
    {
        if (stopper != null) {
            GetComponent<Animator>().enabled = false;
            stopperGone = false;
        }
    }

    void Update()
    {
        if (stopperGone == false && stopper == null) {
            stopperGone = true;
        }
        if (stopperGone) {
            StartCoroutine(Wait(1));
        }
    }

    //Corountine that will wait.
    private IEnumerator Wait(float time) {
        yield return new WaitForSeconds(time);
        GetComponent<Animator>().enabled = true;
    }

}
