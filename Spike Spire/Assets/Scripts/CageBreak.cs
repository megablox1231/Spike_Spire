using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageBreak : MonoBehaviour {
    [SerializeField]
    private Sprite brokenCage;
    [SerializeField]
    private GameObject key;
    [SerializeField]
    private GameObject dialog;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Traveller") {
            other.GetComponent<Animator>().enabled = false;
            other.GetComponent<SpriteRenderer>().sprite = brokenCage;
            key.SetActive(true);
            dialog.SetActive(true);
        }
    }
}
