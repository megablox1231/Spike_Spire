using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Does all action that the keys do which is just opening doors right now.
/// </summary>
public class KeyAction : MonoBehaviour
{

    public GameObject lockBlock;

    private void OnTriggerEnter2D(Collider2D collision) {
        Destroy(lockBlock);
        Destroy(gameObject);
    }
}
