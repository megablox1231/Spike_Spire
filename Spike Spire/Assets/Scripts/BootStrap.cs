using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Collections;

/// <summary>
/// Loads in the main scene and instantiates all
/// "nondisposable" objects.
/// </summary>
public class BootStrap : MonoBehaviour
{

    public GameObject[] objects;
    public String startScene;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Load();
        }
    }

    private void Load() {
        GameObject[] objectClones = new GameObject[objects.Length];
        for (int i = 0; i < objects.Length; i++) {
            objectClones[i] = Instantiate(objects[i]);
            DontDestroyOnLoad(objectClones[i]);
        }

        SceneManager.LoadScene(startScene);

        var gm = objectClones[0].GetComponent<GameMaster>();
        var cineMach = objectClones[1].GetComponent<CinemachineVirtualCamera>();
        if (gm == null) {
            throw new Exception("Error: GM is not the first object in objects");
        }
        else if (cineMach == null) {
            throw new Exception("Error: virtual camera not second object in objects");
        }
        else {
            gm.InitCineMachine(cineMach);
            GameMaster.sceneName = startScene;
        }
    }

    public void Load(string levelTrigger) {
        StartCoroutine(LoadHelper(levelTrigger));
    }

    private IEnumerator LoadHelper(string levelTrigger) {
        GameObject[] objectClones = new GameObject[objects.Length];
        for (int i = 0; i < objects.Length; i++) {
            objectClones[i] = Instantiate(objects[i]);
            DontDestroyOnLoad(objectClones[i]);
        }
        DontDestroyOnLoad(gameObject);

        var gm = objectClones[0].GetComponent<GameMaster>();
        gm.SetTrigger(levelTrigger, false);
        SceneManager.LoadScene(startScene);

        var cineMach = objectClones[1].GetComponent<CinemachineVirtualCamera>();

        if (gm == null) {
            throw new Exception("Error: GM is not the first object in objects");
        }
        else if (cineMach == null) {
            throw new Exception("Error: virtual camera not second object in objects");
        }
        else {
            gm.InitCineMachine(cineMach);
            GameMaster.sceneName = startScene;
            GameObject player = objectClones[3];

            while (SceneManager.GetActiveScene().name != startScene) {
                yield return null;
            }
            GameObject trigger = GameObject.Find(levelTrigger);
            Vector3 spawn = trigger.transform.TransformPoint(trigger.GetComponent<CircleCollider2D>().offset);
            player.transform.position = spawn;
            gm.spawnPoint.position = spawn;
            if (trigger.GetComponent<CamBoundryTrigger>().boundingShape == null) {
                GameMaster.gm.MakeCameraStatic();
                objectClones[2].transform.position = new Vector3(trigger.transform.position.x,
                                                     trigger.transform.position.y, objectClones[2].transform.position.z);
            }
        }
        Destroy(gameObject);
    }
}
