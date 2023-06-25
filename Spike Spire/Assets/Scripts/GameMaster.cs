using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

/// <summary>
/// Initializes stuff at the start that isn't done by BootStrap.
/// Does death, camera, and restarting the level logic.
/// Keeps track of global objects and variables.
/// </summary>
public class GameMaster : MonoBehaviour {

    public static GameMaster gm;

    public Transform playerPrefab;
    public Transform spawnPoint;
    public static string sceneName;

    [HideInInspector]
    public string CurTrigger {
        get { return curTrigger; }
        private set { curTrigger = value; }
    }

    public float spawnDelay;
    [SerializeField] float reloadSceneDelay;
    [SerializeField] float deathPushSpeed;
    [HideInInspector] public DeathTransition deathTransition;
    public bool Restarting { get; private set; }

    private bool playerHasForwardSlash;
    [HideInInspector] public bool PlayerHasForwardSlash {
        get { return playerHasForwardSlash; }
        set {
            playerHasForwardSlash = value;
            saveSystem.SetForwardSlash(value);
            saveSystem.Save();
        }
    }
    [HideInInspector] public bool dollyShotDone;
    [HideInInspector] public string checkPoint;

    CinemachineVirtualCamera virtualCam;
    CinemachineConfiner confiner;
    string curTrigger = "";
    bool inLargeArea;   //true if in a free camera area
    GameObject curPlayer;
    SaveSystem saveSystem;
    LiquidGenarate deathLiquidEffect;

    void Awake() {
        if (gm == null) {
            gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMaster>();
        }
        saveSystem = new SaveSystem();
        saveSystem.Load();
    }

    void Start() {
        curPlayer = GameObject.FindGameObjectWithTag("Player");
        // Need to check parent because player object and child "Player Collider" both have player tag
        if (curPlayer.transform.parent != null && curPlayer.transform.parent.tag == "Player") {
            curPlayer = curPlayer.transform.parent.gameObject;
        }
        curPlayer.GetComponent<PlayerInput>().hasForwardSlash = PlayerHasForwardSlash;

        if (sceneName == null) {
            sceneName = SceneManager.GetActiveScene().name;
        }

        if (confiner == null) { //TODO: just for editor stuff, remove for full build
            InitCineMachine(GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>());
        }
        deathLiquidEffect = GetComponent<LiquidGenarate>();
    }

    // Pushes player in direction opp of death collider, destroys them, then reloads scene and
    // reinstantiates player.
    IEnumerator RestartPlayerHelper(GameObject player, Vector2 pushDirection) {
        // Restarting variable guards against multiple restarts
        if (Restarting) { yield break; }

        Restarting = true;
        player.GetComponent<Animator>().SetBool("dead", true);
        player.GetComponent<PlayerMovement>().frozen = true;
        player.GetComponent<PlayerInput>().DisableMovement();
        yield return MoveTowardsPush(deathPushSpeed, pushDirection);

        yield return new WaitForSeconds(reloadSceneDelay);
        yield return deathTransition.TransitionStart();
        yield return SceneManager.LoadSceneAsync(sceneName);
        Destroy(player); // if collider not child then just destory game object

        yield return new WaitForSeconds(spawnDelay);
        if (GameObject.FindGameObjectWithTag("Player") == null) {
            Transform clone = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            virtualCam.Follow = clone;
            curPlayer = clone.gameObject;

            if (PlayerHasForwardSlash) {
                curPlayer.GetComponent<PlayerInput>().hasForwardSlash = true;
            }
        }

        Restarting = false;
    }

    // Moves player in given direction, displaying a death liquid effect after a certain distance is passed.
    IEnumerator MoveTowardsPush(float speed, Vector2 pushDirection) {
        Rigidbody2D rb = curPlayer.GetComponent<Rigidbody2D>();
        Vector2 finalPos = rb.position + pushDirection;
        bool deathLiquidActivated = false;

        while (rb.position != finalPos) {
            rb.MovePosition(Vector2.MoveTowards(rb.position, finalPos, speed * Time.fixedDeltaTime));
            if (!deathLiquidActivated && (finalPos - rb.position).magnitude < (pushDirection / 6).magnitude) {
                // Blow up a little early to prevent pause after push looks done visually
                deathLiquidEffect.BlowUpEffect(curPlayer.transform.position);
                deathLiquidActivated = true;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    // Reloads the active scene, destroys player, and calls respawn coroutine.
    public static void RestartPlayer(GameObject player, Vector2 pushDirection) {
        gm.StartCoroutine(gm.RestartPlayerHelper(player, pushDirection));
    }

    public void SetTrigger(string name, bool save=true) {
        curTrigger = name;
        sceneName = SceneManager.GetActiveScene().name;

        if (save) {
            saveSystem.SetProgress(sceneName, name);
            saveSystem.Save();
        }
    }

    public bool IsCurTrigger(string name) {
        return curTrigger.Equals(name) && sceneName.Equals(SceneManager.GetActiveScene().name);
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

    // Transitions all camera settings to be able to move
    public void ChangeConfinerBounds(PolygonCollider2D boundingShape) {
        Camera.main.GetComponent<CinemachineBrain>().enabled = true;
        inLargeArea = true;
        confiner.InvalidatePathCache();
        confiner.m_BoundingShape2D = boundingShape;
    }

    public void MakeCameraStatic() {
        Camera.main.GetComponent<CinemachineBrain>().enabled = false;
        inLargeArea = false;
    }

    public void SetBoundingShape(PolygonCollider2D boundingShape) {
        confiner.m_BoundingShape2D = boundingShape;
    }

    public void ChangeCamTarget(Transform target) {
        virtualCam.GetComponent<CinemachineVirtualCamera>().Follow = target;
    }

    public void HardLockCam() {
        virtualCam.AddCinemachineComponent<CinemachineHardLockToTarget>();
    }
    public void TransposerCam() {
        virtualCam.AddCinemachineComponent<CinemachineFramingTransposer>();
    }

    // True if camera viewport in cinemachine confiner bounding shape
    public bool CamInConfinerCheck(Vector2 offset) {
        return confiner.m_BoundingShape2D != null 
            && confiner.m_BoundingShape2D.bounds.Contains(Camera.main.ViewportToWorldPoint(Vector2.zero) + new Vector3(offset.x, offset.y, 10))
            && confiner.m_BoundingShape2D.bounds.Contains(Camera.main.ViewportToWorldPoint(new Vector2(1, 1)) + new Vector3(offset.x, offset.y, 10));
    }

    public GameObject GetCurPlayer() {
        return curPlayer;
    }
}
