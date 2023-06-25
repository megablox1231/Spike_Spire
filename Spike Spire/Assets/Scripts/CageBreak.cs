using UnityEngine;

/// <summary>
/// Sets up broke cage scene after traveller falls to the floor.
/// </summary>
public class CageBreak : MonoBehaviour {

    [SerializeField] Sprite brokenCage;
    [SerializeField] GameObject key;
    [SerializeField] GameObject dialog;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Traveller") {
            other.GetComponent<Animator>().enabled = false;
            other.GetComponent<SpriteRenderer>().sprite = brokenCage;
            other.GetComponent<AudioSource>().Play();
            key.SetActive(true);
            dialog.SetActive(true);
        }
    }
}
