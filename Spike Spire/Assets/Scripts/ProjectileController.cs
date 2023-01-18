using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{

    public float shotTimer;
    public float shotSpeed;
    public enum shotDirection { Up, Down, Left, Right };
    public shotDirection shotDirect;
    public Vector3 localShotBorder; // Point at which shots are destroyed
    private Vector3 globalShotBorder; //converted to global space

    public Transform shotPrefab;

    public AnimationClip clip;
    private bool dontShoot;
    private List<Transform> projtls;
    private Animator anim;
    private float animWait;

    // Start is called before the first frame update
    void Start()
    {
        dontShoot = false;
        projtls = new List<Transform>();
        globalShotBorder = localShotBorder + transform.position;

        anim = GetComponent<Animator>();
        animWait = clip.length;
        if (clip.length > shotTimer) {
            anim.speed = clip.length / shotTimer;
            animWait = shotTimer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!dontShoot) {
            projtls.Add(Instantiate(shotPrefab, transform));
            StartCoroutine(Wait());
        }

        if (shotDirect == shotDirection.Up) {
            for (int i = projtls.Count - 1; i >= 0; i--) {
                projtls[i].position = new Vector3(projtls[i].position.x, projtls[i].position.y + (shotSpeed * Time.deltaTime), projtls[i].position.z);
                if (projtls[i].position.y >= globalShotBorder.y) {
                    Destroy(projtls[i].gameObject); //TODO: add animation here
                    projtls.Remove(projtls[i]);
                }
            }
        }

        if (shotDirect == shotDirection.Down) {
            for (int i = projtls.Count - 1; i >= 0; i--) {
                projtls[i].position = new Vector3(projtls[i].position.x, projtls[i].position.y - (shotSpeed * Time.deltaTime), projtls[i].position.z);
                if (projtls[i].position.y <= globalShotBorder.y) {
                    Destroy(projtls[i].gameObject);
                    projtls.Remove(projtls[i]);
                }
            }
        }

        if (shotDirect == shotDirection.Left) {
            for (int i = projtls.Count - 1; i >= 0; i--) {
                projtls[i].position = new Vector3(projtls[i].position.x - (shotSpeed * Time.deltaTime), projtls[i].position.y, projtls[i].position.z);
                if (projtls[i].position.x <= globalShotBorder.x) {
                    Destroy(projtls[i].gameObject);
                    projtls.Remove(projtls[i]);
                }
            }
        }

        if (shotDirect == shotDirection.Right) {
            for (int i = projtls.Count - 1; i >= 0; i--) {
                projtls[i].position = new Vector3(projtls[i].position.x + (shotSpeed * Time.deltaTime), projtls[i].position.y, projtls[i].position.z);
                if (projtls[i].position.x >= globalShotBorder.x) {
                    Destroy(projtls[i].gameObject);
                    projtls.Remove(projtls[i]);
                }
            }
        }
    }

    //Corountine that will wait.
    private IEnumerator Wait() {
        dontShoot = true;
        yield return new WaitForSeconds(shotTimer - animWait);
        anim.Play(clip.name);
        yield return new WaitForSeconds(animWait);
        dontShoot = false;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        Debug.Log("here");
        if (GameObject.ReferenceEquals(collider.transform.parent.gameObject, gameObject)) {
            Debug.Log("there");
            Destroy(collider.gameObject);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        float size = .3f;
        globalShotBorder = (Application.isPlaying) ? globalShotBorder : localShotBorder + transform.position;
        Gizmos.DrawLine(globalShotBorder - Vector3.up * size, globalShotBorder + Vector3.up * size);
        Gizmos.DrawLine(globalShotBorder - Vector3.left * size, globalShotBorder + Vector3.left * size);
    }
}
