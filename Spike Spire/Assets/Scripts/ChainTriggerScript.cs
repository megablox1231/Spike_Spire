using UnityEngine;

/// <summary>
/// Handles destorying chain object upon collision with player sword jump
/// or forward slash, dropping the selected object.
/// </summary>
public class ChainTriggerScript : MonoBehaviour {

    [SerializeField] GameObject objects;
    [SerializeField] float speed;
    [SerializeField] int fallDist;

    Animator[] chainAnimators;
    bool moveObjects = false;
    [HideInInspector] public bool fallDone;
    Vector3 target;

    void Start() {
        chainAnimators = GetComponentsInChildren<Animator>();
        target = objects.transform.localPosition - new Vector3(0, fallDist, 0);
    }
    
    void Update() {
        if (moveObjects) {
            if (objects.transform.localPosition == target) {
                fallDone = true;
                moveObjects = false;
            }
            objects.transform.localPosition = Vector3.MoveTowards(objects.transform.localPosition, target, speed*Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        //break chain if hit by sword jump or forward slash
        if (collision.name.CompareTo("ForwardSlash") == 0 
            || (collision.name.CompareTo("SwordJump") == 0 && GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().isSwordJumping)) {
            moveObjects = true;
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<AudioSource>().Play();
            for (int i = 0; i < chainAnimators.Length; i++) {
                chainAnimators[i].enabled = true;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision) {
        if (collision.name.CompareTo("SwordJump") == 0 && GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().isSwordJumping) {
            moveObjects = true;
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<AudioSource>().Play();
            for (int i = 0; i < chainAnimators.Length; i++) {
                chainAnimators[i].enabled = true;
            }
        }
    }

}
