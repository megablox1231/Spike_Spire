using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads the next scene when player collides with trigger.
/// </summary>
public class SceneChange : MonoBehaviour {

    [SerializeField] string sceneName;

    void OnTriggerEnter2D() {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(WaitForSceneLoad());
    }

    IEnumerator WaitForSceneLoad() {
        GameObject player = GameMaster.gm.GetCurPlayer();
        PlayerStats stats = player.GetComponentInChildren<PlayerStats>();
        stats.invincible = true;
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        DontDestroyOnLoad(player);

        yield return GameMaster.gm.deathTransition.TransitionStart();
        yield return SceneManager.LoadSceneAsync(sceneName);

        GameMaster.gm.MakeCameraStatic();
        Camera.main.transform.position = new Vector3(0, 0, -10);
        GameObject trigger = GameObject.Find("CamTrigger1");
        GameMaster.gm.spawnPoint.position = trigger.transform.TransformPoint(trigger.GetComponent<BoxCollider2D>().offset);

        movement.frozen = true;
        movement.ResetVelocity();
        // player moved out of game while delay occurs
        player.transform.position = new Vector3(0, 0, -20);
        yield return new WaitForSeconds(2f);
        player.transform.position = GameMaster.gm.spawnPoint.position;
        stats.invincible = false;
        movement.frozen = false;
        Destroy(gameObject);
    }
}
