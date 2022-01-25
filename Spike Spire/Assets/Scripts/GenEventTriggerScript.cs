using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

public class GenEventTriggerScript : MonoBehaviour
{

    public UnityEvent triggered;

    private void OnTriggerEnter2D(Collider2D collision) {
        triggered.Invoke();
        Destroy(gameObject);
    }
}
