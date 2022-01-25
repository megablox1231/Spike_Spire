using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamBoundryTrigger : MonoBehaviour {
    //isMoving - is the camera moving
    //isDone - is OnTriggerEnter2D completed
    bool isMoving, isDone = false;
    Transform cam;
    Vector3 desiredPos;
    Vector3 triggerPos;
    Vector3 spawnYLocale;   //spawnPoint location with the x replaced with player locale x
    Vector3 spawnXLocale;   //spawnPoint location with the y replaced with player locale y
    GameObject curPlayer;

    public Transform spawnPoint;
    public GameObject blocker;
    public PolygonCollider2D boundingShape; //collider to which virtual camera is confined if it active in the level
    public enum spawnLocale { Up, Down, Left, Right };
    public spawnLocale locale = spawnLocale.Up;

    // Start is called before the first frame update
    void Start() {

        if (GameMaster.gm.ContainsTrigger(name)) {
            //makes virtual camera confiner bounded to this trigger's bounding shape and activates blocker (done because level reset)
            if (GameMaster.gm.IsCurTrigger(name)) {
                GameMaster.gm.SetBoundingShape(boundingShape);
                if (blocker != null) {
                    blocker.SetActive(true);
                }
            }

            gameObject.SetActive(false);
        }

        cam = Camera.main.transform;
        triggerPos = transform.TransformPoint(GetComponent<BoxCollider2D>().offset);
    }

    // Update is called once per frame
    void Update() {
        if (isMoving) {
            cam.position = Vector3.MoveTowards(cam.position, desiredPos, .6f);

            if (locale == spawnLocale.Up || locale == spawnLocale.Down) { // push player a bit up to get past spike blocker
                curPlayer.transform.position = Vector3.MoveTowards(curPlayer.transform.position, spawnYLocale, .1f);    //TODO: player transitioning to new level a bit squirrely
            }
            else if(locale == spawnLocale.Left || locale == spawnLocale.Right) {
                curPlayer.transform.position = Vector3.MoveTowards(curPlayer.transform.position, spawnXLocale, .1f);
            }
        }
        if (isMoving && cam.position == desiredPos) {
            isMoving = false;

            // So in order to eliminate any remaining difference
            // make sure to set it to the correct target position
            cam.position = desiredPos;
        }
        if(isDone && !isMoving) { // both camera move and spawn move are done; needed in both update and trigger
            GameMaster.gm.AddDestroyedTrigger(name);
            gameObject.SetActive(false);//TODO: make sure never happens too early
        }
    }

    private IEnumerator OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) {
            yield break; //exits the coroutine
        }

        Time.timeScale = 0f;
        curPlayer = GameMaster.gm.GetCurPlayer();

        switch (locale) {
            case spawnLocale.Up:
                spawnPoint.position = triggerPos + new Vector3(0f, 0.5f, 0f);
                break;
            case spawnLocale.Down:
                spawnPoint.position = triggerPos + new Vector3(0f, -0.5f, 0f);
                break;
            case spawnLocale.Left:
                spawnPoint.position = triggerPos + new Vector3(-0.5f, 0f, 0f);
                break;
            case spawnLocale.Right:
                spawnPoint.position = triggerPos + new Vector3(0.5f, 0f, 0f);
                break;
        }

        spawnYLocale = new Vector3(curPlayer.transform.position.x, spawnPoint.position.y, spawnPoint.position.z);
        spawnXLocale = new Vector3(spawnPoint.position.x, curPlayer.transform.position.y, spawnPoint.position.z);

        isMoving = true;
        desiredPos = transform.position;
        desiredPos = new Vector3(desiredPos.x, desiredPos.y, -10);

        if(boundingShape != null) {
            GameMaster.gm.ChangeConfinerBounds(boundingShape);
        }
        else {
            GameMaster.gm.MakeCameraStatic();
        }

        yield return new WaitForSecondsRealtime(0.5f);

        if (blocker != null) {
            blocker.SetActive(true);
        }

        Time.timeScale = 1f;

        isDone = true;
        if (isDone && !isMoving) { // both camera move and spawn move are done
            GameMaster.gm.AddDestroyedTrigger(name);
            gameObject.SetActive(false);//TODO: make sure never happens too early
        }
    }
}
