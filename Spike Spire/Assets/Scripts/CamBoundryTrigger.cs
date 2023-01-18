using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Moves the camera and character and activates blocker
/// when transitioning to a new level.
/// </summary>
public class CamBoundryTrigger : MonoBehaviour {
    // isMoving - is the camera and/or player moving?
    // isDone - is OnTriggerEnter2D completed?
    bool isMoving, isDone = false;
    Transform cam;
    Vector3 desiredCamPos;  // new position for camera
    Vector3 movePlayerPos;  // new position for player
    Vector3 triggerPos;
    GameObject curPlayer;

    [HideInInspector]
    public Vector3 spawnPoint;
    public GameObject blocker;
    public PolygonCollider2D boundingShape; // collider to which virtual camera is confined if it active in the level
    public GameObject levelColliders;
    public enum spawnLocale { Up, Down, Left, Right };
    public spawnLocale locale = spawnLocale.Up;

    void Start() {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        // virtual camera confiner bounded to this trigger's bounding shape,
        // enables lvl colliders, activates blocker (done because level reset)
        if (GameMaster.gm.IsCurTrigger(name)) {
            EnableLevelColliders();
            GameMaster.gm.SetBoundingShape(boundingShape);
            if (blocker != null) {
                blocker.SetActive(true);
            }

            boxCollider.isTrigger = false;
            this.enabled = false; //!!changed from disabling just script, may brake something!!
        }
        cam = Camera.main.transform;
        triggerPos = transform.TransformPoint(boxCollider.offset);
    }

    void Update() {
        if (isMoving) {
            cam.position = Vector3.MoveTowards(cam.position, desiredCamPos, 38f * Time.unscaledDeltaTime);
            // push player a bit to get past spike blocker
            curPlayer.transform.position = Vector3.MoveTowards(curPlayer.transform.position, movePlayerPos, 32f * Time.unscaledDeltaTime);
            //Debug.Log("Cam done? " + (cam.position == desiredCamPos));
            //Debug.Log("Player done? " + (curPlayer.transform.position == movePlayerPos));
            //Debug.Log((cam.position - desiredCamPos).ToString() + "   " + 38 * Time.unscaledDeltaTime);
        }
        if (isMoving && ((cam.position - desiredCamPos).magnitude < (38f * Time.unscaledDeltaTime)) && curPlayer.transform.position == movePlayerPos) {
           // Debug.Log((cam.position - desiredCamPos).ToString());
            // So in order to eliminate any remaining difference
            // make sure to set it to the correct target position
            cam.position = desiredCamPos;
            Time.timeScale = 1f;
            isMoving = false;
        }
        if (isDone && !isMoving) { // both camera move and spawn move are done; needed in both update and trigger
            Debug.Log("Done  " + (curPlayer.transform.position - movePlayerPos));
            if (boundingShape != null) {
                GameMaster.gm.ChangeConfinerBounds(boundingShape);
            }
            GameMaster.gm.SetTrigger(name);
            EnableLevelColliders();
            gameObject.SetActive(false);//TODO: make sure never happens too early
        }
    }

    private IEnumerator OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) {
            yield break; //exits the coroutine
        }

        Time.timeScale = 0f;
        // TODO: might need to disable invisible colliders of last level
        curPlayer = GameMaster.gm.GetCurPlayer();

        spawnPoint = transform.TransformPoint(GetComponent<CircleCollider2D>().offset);
        GameMaster.gm.spawnPoint.position = spawnPoint;

        switch (locale) {
            case spawnLocale.Up:
                movePlayerPos = new Vector3(curPlayer.transform.position.x, triggerPos.y + 3, triggerPos.z);
                break;
            case spawnLocale.Down:
                movePlayerPos = new Vector3(curPlayer.transform.position.x, triggerPos.y - 3, triggerPos.z);
                break;
            case spawnLocale.Left:
                movePlayerPos = new Vector3(triggerPos.x - 3, curPlayer.transform.position.y, triggerPos.z);
                break;
            case spawnLocale.Right:
                movePlayerPos = new Vector3(triggerPos.x + 3, curPlayer.transform.position.y, triggerPos.z);
                break;
        }

        isMoving = true;
        desiredCamPos = transform.position;
        desiredCamPos = new Vector3(desiredCamPos.x, desiredCamPos.y, -10);

        
        GameMaster.gm.MakeCameraStatic();

        yield return new WaitForSecondsRealtime(0.5f);

        if (blocker != null) {
            blocker.SetActive(true);
        }

        isDone = true;
        if (isDone && !isMoving) { // both camera move and spawn move are done
            Debug.Log("Done  " + (curPlayer.transform.position - movePlayerPos));
            if (boundingShape != null) {
                GameMaster.gm.ChangeConfinerBounds(boundingShape);
            }
            GameMaster.gm.SetTrigger(name);
            EnableLevelColliders();
            gameObject.SetActive(false);//TODO: make sure never happens too early
        }
    }

    //Enables the death and invisible wall colliders of level parenting this object
    void EnableLevelColliders() {
        if (levelColliders != null) {
            BoxCollider2D[] coll = levelColliders.GetComponents<BoxCollider2D>();
            for (int i = 0; i < coll.Length; i++) {
                coll[i].enabled = true;
            }
        }
    }
}
