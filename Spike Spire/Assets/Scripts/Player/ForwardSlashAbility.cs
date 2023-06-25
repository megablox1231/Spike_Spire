using System.Collections;
using UnityEngine;

/// <summary>
/// Controls forward slash action and collision handling.
/// </summary>
public class ForwardSlashAbility : MonoBehaviour {

    Collider2D fSlashCollider;
    PlayerMovement playerMove;

    bool hitSucess;

    void Start()
    {
        fSlashCollider = GetComponent<Collider2D>();
        fSlashCollider.enabled = false;

        playerMove = transform.parent.GetComponent<PlayerMovement>();
    }


    void OnCollisionEnter2D(Collision2D collision) {
        if (!hitSucess && collision.collider.tag != "SafeBlock") {
            hitSucess = playerMove.OnForwardSlashCollision();
        }
    }

    // used to be 0.3
    public IEnumerator DoForwardSlash() {
        fSlashCollider.enabled = true;
        yield return new WaitForSeconds(.24f);
        fSlashCollider.enabled = false;
        hitSucess = false;
    }

}
