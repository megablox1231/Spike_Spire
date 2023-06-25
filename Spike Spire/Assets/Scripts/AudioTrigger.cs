using UnityEngine;

/// <summary>
/// Plays AuidioClip on the AudioSource of the Main Camera.
/// </summary>
public class AudioTrigger : MonoBehaviour {

    [SerializeField] AudioClip bgMusic;
    [SerializeField] float volume;

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Player") {
            AudioSource audioSource = Camera.main.GetComponent<AudioSource>();
            if (audioSource.clip != bgMusic) {
                audioSource.clip = bgMusic;
                audioSource.volume = volume;
                audioSource.Play();
            }
            GetComponent<PolygonCollider2D>().enabled = false;
        }
    }
}
