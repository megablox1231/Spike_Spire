using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public int maxHealth = 50;
    private int curHealth;
    public bool invincible;

    void Start() {
        curHealth = maxHealth;
    }


    void Update() {

    }

    private void OnTriggerEnter2D(Collider2D collider) { //TODO: no player tag check, so might not work always
        if (collider.CompareTag("DeathBox") && !invincible) {
            GameMaster.KillPlayer(this.transform.parent.gameObject);
        }
    }
}
