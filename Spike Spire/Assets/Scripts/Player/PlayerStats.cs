using UnityEngine;

/// <summary>
/// Tracks invincibility and handles collisions with body collider.
/// </summary>
public class PlayerStats : MonoBehaviour {

    public bool invincible;

    //Exists on child object so other colliders won't trigger this
    private void OnCollisionEnter2D(Collision2D collision) {
        Collider2D collider = collision.collider;
        if (collider.CompareTag("DeathBox") && !invincible) {
            GameMaster.RestartPlayer(this.transform.parent.gameObject, collider.Distance(GetComponent<Collider2D>()).normal);
        }
    }
}
