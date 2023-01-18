using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{

    public GameObject rocketShooter;

    void Awake() {
        if (name.Equals(GameMaster.gm.checkPoint)) {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(WaitThenDisable());
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        GameMaster.gm.spawnPoint.position = transform.position;
        GameMaster.gm.checkPoint = name;
        this.gameObject.SetActive(false);
    }

    IEnumerator WaitThenDisable() {
        yield return new WaitForSeconds(GameMaster.gm.spawnDelay);
        rocketShooter.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
