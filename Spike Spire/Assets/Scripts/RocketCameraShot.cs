using Cinemachine;
using System.Collections;
using UnityEngine;

/// <summary>
/// Works with the camera, floor, and rocket to show the rocket
/// approaching the player
/// </summary>
public class RocketCameraShot : MonoBehaviour {

    public GameObject dollyCam;
    public CinemachineDollyCart dollyCart;
    public float cartFinalPos; // position cart will be in when movement done
    bool camMoving = true;

    public GameObject rocketShooter;
    PlayerMovement player;
    
    // floor sections that open up
    public Transform floorLeft, floorRight;
    public float floorSpeed;
    Vector3 floorLeftTarget, floorRightTarget;
    bool moveFloor;

    AudioSource floorAudio;

    
    void Awake() {
        if (GameMaster.gm.dollyShotDone) {
            gameObject.SetActive(false);
        }
    }

    void Start() {
        floorLeftTarget = new Vector3(floorLeft.position.x - 14,
                                   floorLeft.position.y, floorLeft.position.z);
        floorRightTarget = new Vector3(floorRight.position.x + 14,
                                    floorRight.position.y, floorRight.position.z);
        floorAudio = gameObject.GetComponent<AudioSource>();
    }

    void Update() {
        if (moveFloor) {
            if (Mathf.Abs(floorLeft.position.x - floorLeftTarget.x) < 0.0001) {
                moveFloor = false;
            }

            floorLeft.position = Vector3.MoveTowards(floorLeft.position, floorLeftTarget, 
                                                     floorSpeed * Time.deltaTime);
            floorRight.position = Vector3.MoveTowards(floorRight.position, floorRightTarget,
                                                      floorSpeed * Time.deltaTime);
        }
        else if (camMoving && Mathf.Abs(dollyCart.m_Position - cartFinalPos) < 0.0001) {
            camMoving = false;
            StartCoroutine(ActivateRocket());
        }
        
    }

    // Freezes the player and moves the camera down to the floor
    void OnTriggerEnter2D(Collider2D collider) {
        player = GameMaster.gm.GetCurPlayer().GetComponent<PlayerMovement>();

        dollyCam.SetActive(true);
        dollyCart.gameObject.SetActive(true);
        player.frozen = true;
        GameMaster.gm.dollyShotDone = true;
    }

    // Moves the floor, activates the rocket, then moves the camera back to the player
    IEnumerator ActivateRocket() {
        yield return new WaitForSeconds(1);
        moveFloor = true;
        floorAudio.Play();
        while (moveFloor) {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        rocketShooter.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        dollyCam.SetActive(false);
        gameObject.SetActive(false);
        player.frozen = false;
    }
}
