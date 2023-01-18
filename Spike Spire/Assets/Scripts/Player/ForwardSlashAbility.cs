using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardSlashAbility : MonoBehaviour
{

    Collider2D fSlashCollider;

    PlayerMovement playerMove;

    // Start is called before the first frame update
    void Start()
    {
        fSlashCollider = GetComponent<Collider2D>();
        fSlashCollider.enabled = false;

        playerMove = transform.parent.GetComponent<PlayerMovement>();
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        playerMove.OnForwardSlashCollision();
        fSlashCollider.enabled = false; //hit what we wanted to hit, so ending fSlash early
    }

    public IEnumerator DoForwardSlash() {
        fSlashCollider.enabled = true;
        SpriteRenderer box = GetComponent<SpriteRenderer>();
        //box.enabled = true;
        yield return new WaitForSeconds(.3f);
        fSlashCollider.enabled = false;
        //box.enabled = false;
    }

}
