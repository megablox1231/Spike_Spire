using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour {

    [SerializeField]
    private AudioClip jumpLandSand;
    [SerializeField]
    private AudioClip jump;
    [SerializeField]
    private AudioClip stepMetal;
    [SerializeField]
    private AudioClip swing;
    [SerializeField]
    private AudioClip clash;

    private AudioSource audioSrc;

    void Start() {
        audioSrc = GetComponent<AudioSource>();
    }

    private void JumpLandingSound() {
        audioSrc.pitch = 1;
        audioSrc.clip = jumpLandSand;
        audioSrc.Play();
    }

    private void JumpSound() {
        audioSrc.pitch = 1;
        audioSrc.clip= jump;
        audioSrc.Play();
    }

    private void StepSound() {
        audioSrc.pitch = 1;
        audioSrc.clip = stepMetal;
        audioSrc.Play();
    }

    public void SwingSound() {
        audioSrc.pitch = 1;
        audioSrc.clip = swing;
        audioSrc.Play();
    }

    public void ClashSound() {
        audioSrc.pitch = Random.Range(0.95f, 1.05f);
        audioSrc.clip = clash;
        audioSrc.Play();
    }
}
