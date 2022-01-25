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
    [HideInInspector]
    public bool playerHasForwardSlash; //true if the player has the forward slash ability

    CinemachineVirtualCamera virtualCam;
    CinemachineConfiner confiner;
    Stack<string> camTriggers;  //camTriggers that have been passed
    bool inLargeArea;   //true if in a moving area (currently not used)
    GameObject curPlayer;

    void Awake() {
        if (gm == null) {
            gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMaster>();
        }
        playerHasForwardSlash = false;  //TODO: might not fire off before accessed by player
    }

    private void Start() {
        camTriggers = new Stack<string>();
        inLargeArea = false;
        curPlayer = GameObject.FindGameObjectWithTag("Player");
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

    public static void KillPlayer(GameObject player) {
        SceneManager.LoadScene("Demo_3");
        Destroy(player); // if collider not child then just destory game object
        gm.StartCoroutine(gm._RespawnPlayer());
    }


    public void InitCineMachine(CinemachineVirtualCamera cam) {
        virtualCam = cam;
        virtualCam.Follow = GameObject.FindGameObjectWithTag("Player").transform;

        confiner = virtualCam.GetComponent<CinemachineConfiner>();
        confiner.m_ConfineScreenEdges = true;
    }


    public void AddDestroyedTrigger(string name) {
        camTriggers.Push(name);
    }

    public bool ContainsTrigger(string name) {
        return camTriggers.Contains(name);
    }

    public bool IsCurTrigger(string name) {
        return camTriggers.Peek() == name;
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
