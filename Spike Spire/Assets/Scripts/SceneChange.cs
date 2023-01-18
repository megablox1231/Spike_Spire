using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads the next scene when player collides with trigger.
/// </summary>
public class SceneChange : MonoBehaviour
{

    public string sceneName;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D() {
        DontDestroyOnLoad(gameObject);
        GameMaster.gm.SetTrigger("temp");
        GameMaster.sceneName = sceneName;
        SceneManager.LoadScene(sceneName);
        StartCoroutine("WaitForSceneLoad");
    }

    IEnumerator WaitForSceneLoad() {
        GameObject player = GameMaster.gm.GetCurPlayer();
        player.GetComponentInChildren<PlayerStats>().invincible = true;

        while (SceneManager.GetActiveScene().name != sceneName) {
            yield return null;
        }

        GameMaster.gm.MakeCameraStatic();
        Camera.main.transform.position = new Vector3(0, 0, -10);
        GameObject trigger = GameObject.Find("CamTrigger1");
        GameMaster.gm.spawnPoint.position = trigger.transform.TransformPoint(trigger.GetComponent<BoxCollider2D>().offset);

            player.GetComponentInChildren<PlayerStats>().invincible = true;
            player.transform.position = new Vector3(0, 0, -20);
            yield return new WaitForSeconds(2f);
            player.transform.position = GameMaster.gm.spawnPoint.position;
            player.GetComponentInChildren<PlayerStats>().invincible = false;
    }
}
