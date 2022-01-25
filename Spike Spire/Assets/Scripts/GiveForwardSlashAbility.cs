using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveForwardSlashAbility : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player")) {
            collision.GetComponentInParent<PlayerInput>().hasForwardSlash = true;
            GameMaster.gm.playerHasForwardSlash = true;
            Destroy(gameObject);
        }
    }
}
