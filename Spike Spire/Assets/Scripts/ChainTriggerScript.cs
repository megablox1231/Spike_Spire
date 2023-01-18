using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
public class ChainTriggerScript : MonoBehaviour
{

    [SerializeField]  private GameObject objects;
    [SerializeField]  private float speed;
    [SerializeField]  private int fallDist;

    private Animator[] animators;
    private bool move = false;
    private Vector3 target;


    // Start is called before the first frame update
    void Start()
    {
        animators = GetComponentsInChildren<Animator>();
        for (int i = 0; i < animators.Length; i++) {
            animators[i].enabled = false;
        }
        target = objects.transform.localPosition - new Vector3(0, fallDist, 0);
    }
    
    private void Update() {
        if (move) {
            objects.transform.localPosition = Vector3.MoveTowards(objects.transform.localPosition, target, speed*Time.deltaTime);
        }
    }

    //collision is of the other gameObject
    private void OnTriggerEnter2D(Collider2D collision) {
        //break chain if hit by sword jump or forward slash
        if (collision.name.CompareTo("ForwardSlash") == 0 || (collision.name.CompareTo("SwordJump") == 0 && GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().isSwordJumping)) {
            move = true;
            GetComponent<BoxCollider2D>().enabled = false;
            for (int i = 0; i < animators.Length; i++) {
                animators[i].enabled = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.name.CompareTo("SwordJump") == 0 && GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().isSwordJumping) {
            move = true;
            GetComponent<BoxCollider2D>().enabled = false;
            for (int i = 0; i < animators.Length; i++) {
                animators[i].enabled = true;
            }
        }
    }

}
