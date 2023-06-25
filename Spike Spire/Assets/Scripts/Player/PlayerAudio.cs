using UnityEngine;

/// <summary>
/// Metohds for playing all player related audio clips.
/// </summary>
public class PlayerAudio : MonoBehaviour {

    [SerializeField] AudioClip jumpLandSand;
    [SerializeField] AudioClip hardLandMetal;
    [SerializeField] AudioClip elevatorLand;
    [SerializeField] AudioClip jump;
    [SerializeField] AudioClip stepMetal;
    [SerializeField] AudioClip swing;
    [SerializeField] AudioClip clash;
    [SerializeField] AudioClip brittle;
    [SerializeField] AudioClip deathStart;
    [SerializeField] AudioClip deathExplosion;

    AudioSource audioSrc;

    void Start() {
        audioSrc = GetComponent<AudioSource>();
    }

    void JumpLandingSound() {
        audioSrc.pitch = 1;
        audioSrc.clip = jumpLandSand;
        audioSrc.Play();
    }

    void HardLandingMetalSound() {
        audioSrc.pitch = 1;
        audioSrc.clip = hardLandMetal;
        audioSrc.Play();
    }

    void ElevatorLandingSound() {
        audioSrc.pitch = 1;
        audioSrc.clip = elevatorLand;
        audioSrc.Play();
    }

    void JumpSound() {
        audioSrc.pitch = 1;
        audioSrc.clip= jump;
        audioSrc.Play();
    }

    void StepSound() {
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

    public void BrittleSound() {
        audioSrc.pitch = 1;
        audioSrc.clip = brittle;
        audioSrc.Play();
    }

    public void DeathStartSound() {
        audioSrc.pitch = 1;
        audioSrc.clip = deathStart;
        audioSrc.Play();
    }

    public void DeathExplosionSound() {
        audioSrc.pitch = 1;
        audioSrc.clip = deathExplosion;
        audioSrc.Play();
    }
}
