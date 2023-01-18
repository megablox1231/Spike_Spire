using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

/// <summary>
/// Initializes stuff at the start that isn't done by BootStrap.
/// Does death and restarting the level logic.
/// Keeps track of global objects and variables.
/// </summary>
public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;

    public Transform playerPrefab;
    public Transform spawnPoint;
    public int spawnDelay = 2;
    public string testSceneName = "";
    public static string sceneName;
    [HideInInspector]
    public bool playerHasForwardSlash, dollyShotDone;
    [HideInInspector]
    public string checkPoint;

    CinemachineVirtualCamera virtualCam;
    CinemachineConfiner confiner;
    string curTrigger = "";
    bool inLargeArea;   //true if in a moving area (currently not used)
    GameObject curPlayer;
    SaveSystem saveSystem;
    AudioSource audioSrc;

    void Awake() {
        if (gm == null) {
            gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMaster>();
        }
        playerHasForwardSlash = true;  //TODO: might not fire off before accessed by player
    }

    private void Start() {
        curPlayer = GameObject.FindGameObjectWithTag("Player");
        // Need to check parent because player object and child "Player Collider" both have player tag
        if (curPlayer.transform.parent != null && curPlayer.transform.parent.tag == "Player") {
            curPlayer = curPlayer.transform.parent.gameObject;
        }
        if (testSceneName != "") {
            sceneName = testSceneName;
        }
        saveSystem = new SaveSystem();

        if (confiner == null) { //TODO: just for editor stuff, remove for full build
            InitCineMachine(GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>());
        }
    }

    public IEnumerator _RespawnPlayer() {
        //GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(spawnDelay);
        if (GameObject.FindGameObjectWithTag("Player") == null) { //hopefully slowdown not too bad
            Transform clone = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            virtualCam.Follow = clone;
            curPlayer = clone.gameObject;

            if (playerHasForwardSlash) {
                curPlayer.GetComponent<PlayerInput>().hasForwardSlash = true;
            }
        }
        //GameObject particlesClone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation).gameObject;
    }

    //reloads the active scene, destroys player, and calls respawn coroutine
    public static void KillPlayer(GameObject player) {
        SceneManager.LoadScene(sceneName);
        Destroy(player); // if collider not child then just destory game object
        gm.StartCoroutine(gm._RespawnPlayer());
    }


    public void SetTrigger(string name, bool save=true) {
        curTrigger = name;

        if (save) {
            saveSystem.SetProgress(sceneName, name);
            saveSystem.Save();
        }
    }

    public bool IsCurTrigger(string name) {
        return curTrigger.Equals(name);
    }

    public void ClearTrigger() {
        curTrigger = null;
    }


    public void InitCineMachine(CinemachineVirtualCamera cam) {
        virtualCam = cam;
        virtualCam.Follow = GameObject.FindGameObjectWithTag("Player").transform;

        confiner = virtualCam.GetComponent<CinemachineConfiner>();
        confiner.m_ConfineScreenEdges = true;
        inLargeArea = true;
    }

    //Transitions all camera settings to be able to move
    public void ChangeConfinerBounds(PolygonCollider2D boundingShape) {
        Camera.main.GetComponent<CinemachineBrain>().enabled = true;
        inLargeArea = true;
        confiner.InvalidatePathCache();
        confiner.m_BoundingShape2D = boundingShape;
    }

    //Transitions all camera settings to be static
    public void MakeCameraStatic() {
        Camera.main.GetComponent<CinemachineBrain>().enabled = false;
        inLargeArea = false;
    }

    public void SetBoundingShape(PolygonCollider2D boundingShape) {
        confiner.m_BoundingShape2D = boundingShape;
    }

    public GameObject GetCurPlayer() {
        return curPlayer;
    }
}
