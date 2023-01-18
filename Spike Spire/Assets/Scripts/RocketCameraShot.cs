using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketCameraShot : MonoBehaviour
{

    public GameObject dollyCam;
    public CinemachineDollyCart dollyCart;
    public float cartFinalPos;    //position cart will be in when movement done
    bool camMoving = true;

    public GameObject rocketShooter;
    PlayerMovement player;

    public Transform floorLeft, floorRight;
    public float floorSpeed;
    Vector3 floorLeftTgt, floorRightTgt;
    bool moveFloor;

    
    void Awake() {
        if (GameMaster.gm.dollyShotDone) {
            gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start() {
        
        player = GameMaster.gm.GetCurPlayer().GetComponent<PlayerMovement>();

        floorLeftTgt = new Vector3(floorLeft.position.x - 14,
                                   floorLeft.position.y, floorLeft.position.z);
        floorRightTgt = new Vector3(floorRight.position.x + 14,
                                    floorRight.position.y, floorRight.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (moveFloor) {
            if (Mathf.Abs(floorLeft.position.x - floorLeftTgt.x) < 0.0001) {
                moveFloor = false;
            }

            floorLeft.position = Vector3.MoveTowards(floorLeft.position, floorLeftTgt, 
                                                     floorSpeed * Time.deltaTime);
            floorRight.position = Vector3.MoveTowards(floorRight.position, floorRightTgt,
                                                      floorSpeed * Time.deltaTime);
        }
        else if (camMoving && Mathf.Abs(dollyCart.m_Position - cartFinalPos) < 0.0001) {
            camMoving = false;
            StartCoroutine(ActivateRocket());
        }
        
    }

    void OnTriggerEnter2D(Collider2D collider) {
        dollyCam.SetActive(true);
        dollyCart.gameObject.SetActive(true);
        player.frozen = true;
        GameMaster.gm.dollyShotDone = true;
    }

    IEnumerator ActivateRocket() {
        yield return new WaitForSeconds(1);
        moveFloor = true;
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
