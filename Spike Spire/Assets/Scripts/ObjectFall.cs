using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectFall : MonoBehaviour {

    [SerializeField] private Vector3 target = new Vector3(1, 1, 0);
    [SerializeField] private float speed = .07f;
    [SerializeField] private float delay = 0f;


    private bool move = false;

    // Update is called once per frame
    void Update() {
        if (move) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed);
        }
    }

    public void ChainBroke() {
        StartCoroutine(StartMoving());
    }

    IEnumerator StartMoving() {
        yield return new WaitForSeconds(delay);
        move = true;
    }
}