using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

/// <summary>
/// Loads in the main scene and instantiates all
/// "nondisposable" objects.
/// </summary>
public class BootStrap : MonoBehaviour
{

    public GameObject[] objects;

    void Start()
    {
    }

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

        SceneManager.LoadScene("Demo_2");

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
        }
    }
}
