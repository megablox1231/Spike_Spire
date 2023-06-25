using System.Collections;
using UnityEngine;

/// <summary>
/// Enables the animators of closer spikes after trigger collision.
/// </summary>
public class CloserControl : MonoBehaviour {
    
    [SerializeField] GameObject[] closers;

    public float wait;

    void OnTriggerEnter2D(Collider2D collision) {
        StartCoroutine(AnimateClosers());
    }

    IEnumerator AnimateClosers() {
        for (int i = 0; i < closers.Length; i++) {
            closers[i].GetComponent<Animator>().enabled = true;
            yield return new WaitForSeconds(wait);
        }
    }
}
