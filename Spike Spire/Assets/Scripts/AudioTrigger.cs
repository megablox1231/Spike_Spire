using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{

    [SerializeField]
    private AudioClip bgMusic;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        AudioSource audioSource = Camera.main.GetComponent<AudioSource>();
        if (audioSource.clip != bgMusic) {
            audioSource.clip = bgMusic;
            audioSource.Play();
        }
        GetComponent<PolygonCollider2D>().enabled = false;
    }
}
