using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls generation, movement, and destruction of projectiles.
/// Projectiles can optionally be handled as platforms that move passengers.
/// </summary>
public class ProjectileController : MonoBehaviour {

    [SerializeField] float shotTimer;
    [SerializeField] float shotSpeed;
    public enum shotDirection { Up, Down, Left, Right };
    public shotDirection shotDirect;
    [SerializeField] Vector3 localShotBorder; // point at which shots are destroyed
    Vector3 globalShotBorder; // local border converted to global space

    [SerializeField] Transform shotPrefab;
    [SerializeField] bool shootPlatforms; // true if shots are platforms that can hold passengers
    float shotSize;
    [SerializeField] LayerMask passengerMask;

    [SerializeField] AnimationClip clip;
    List<Transform> projtls;
    Animator anim;
    float animWait;
    bool dontShoot;
    string triggerName; // Name of trigger in level with this projectile controller

    AudioSource shotSound;

    void Start() {
        dontShoot = false;
        projtls = new List<Transform>();
        globalShotBorder = localShotBorder + transform.position;
        if (shootPlatforms) {
            shotSize = shotPrefab.GetComponent<BoxCollider2D>().size.x;
        }

        anim = GetComponent<Animator>();
        animWait = clip.length;
        if (clip.length > shotTimer) {
            anim.speed = clip.length / shotTimer;
            animWait = shotTimer;
        }
        triggerName = transform.parent.parent.GetComponentInChildren<CamBoundryTrigger>(true).gameObject.name;

        shotSound = GetComponent<AudioSource>();
    }

    void Update() {
        // Dont shoot if player not in this proj controller's level
        if (triggerName != GameMaster.gm.CurTrigger) { return; }

        if (!dontShoot) {
            projtls.Add(Instantiate(shotPrefab, transform));
            StartCoroutine(Wait());
        }

        if (shotDirect == shotDirection.Up) {
            for (int i = projtls.Count - 1; i >= 0; i--) {
                if (shootPlatforms && CheckPassenger(projtls[i].gameObject, Vector2.up)) {
                    Controller2D passenger = GameMaster.gm.GetCurPlayer().GetComponent<Controller2D>();
                    if (!GameMaster.gm.Restarting) {
                        // player moves along with the platform
                        passenger.Move(new Vector2(0, shotSpeed * Time.deltaTime), true);
                    }
                }

                projtls[i].position = new Vector3(projtls[i].position.x, projtls[i].position.y + (shotSpeed * Time.deltaTime), projtls[i].position.z);
                // if projectile past border, destroy it
                if (projtls[i].position.y >= globalShotBorder.y) {
                    Destroy(projtls[i].gameObject);
                    projtls.Remove(projtls[i]);
                }
            }
        }

        else if (shotDirect == shotDirection.Down) {
            for (int i = projtls.Count - 1; i >= 0; i--) {
                if (shootPlatforms && CheckPassenger(projtls[i].gameObject, Vector2.down)) {
                    Controller2D passenger = GameMaster.gm.GetCurPlayer().GetComponent<Controller2D>();
                    if (!GameMaster.gm.Restarting) {
                        passenger.Move(new Vector2(0, -shotSpeed * Time.deltaTime), true);
                    }
                }

                projtls[i].position = new Vector3(projtls[i].position.x, projtls[i].position.y - (shotSpeed * Time.deltaTime), projtls[i].position.z);
                if (projtls[i].position.y <= globalShotBorder.y) {
                    Destroy(projtls[i].gameObject);
                    projtls.Remove(projtls[i]);
                }
            }
        }

        else if (shotDirect == shotDirection.Left) {
            for (int i = projtls.Count - 1; i >= 0; i--) {
                if (shootPlatforms && CheckPassenger(projtls[i].gameObject, Vector2.left)) {
                    Controller2D passenger = GameMaster.gm.GetCurPlayer().GetComponent<Controller2D>();
                    if (!GameMaster.gm.Restarting) {
                        passenger.Move(new Vector2(-shotSpeed * Time.deltaTime, 0), true);
                    }
                }

                projtls[i].position = new Vector3(projtls[i].position.x - (shotSpeed * Time.deltaTime), projtls[i].position.y, projtls[i].position.z);
                if (projtls[i].position.x <= globalShotBorder.x) {
                    Destroy(projtls[i].gameObject);
                    projtls.Remove(projtls[i]);
                }
            }
        }

        else if (shotDirect == shotDirection.Right) {
            for (int i = projtls.Count - 1; i >= 0; i--) {
                if (shootPlatforms && CheckPassenger(projtls[i].gameObject, Vector2.right)) {
                    Controller2D passenger = GameMaster.gm.GetCurPlayer().GetComponent<Controller2D>();
                    if (!GameMaster.gm.Restarting) {
                        passenger.Move(new Vector2(shotSpeed * Time.deltaTime, 0), true);
                    }
                }

                projtls[i].position = new Vector3(projtls[i].position.x + (shotSpeed * Time.deltaTime), projtls[i].position.y, projtls[i].position.z);
                if (projtls[i].position.x >= globalShotBorder.x) {
                    Destroy(projtls[i].gameObject);
                    projtls.Remove(projtls[i]);
                }
            }
        }
    }

    IEnumerator Wait() {
        dontShoot = true;
        yield return new WaitForSeconds(shotTimer - animWait);
        anim.Play(clip.name);
        yield return new WaitForSeconds(animWait);
        dontShoot = false;
    }

    bool CheckPassenger(GameObject shot, Vector2 direction) {
        RaycastHit2D hit = Physics2D.BoxCast(shot.transform.position, new Vector2(shotSize, 0.1f), 0, direction, shotSize / 2, passengerMask);
        if (hit) {
            return true;
        }
        return false;
    }

    void PlaySound() {
        shotSound.Play();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        float size = .3f;
        globalShotBorder = (Application.isPlaying) ? globalShotBorder : localShotBorder + transform.position;
        Gizmos.DrawLine(globalShotBorder - Vector3.up * size, globalShotBorder + Vector3.up * size);
        Gizmos.DrawLine(globalShotBorder - Vector3.left * size, globalShotBorder + Vector3.left * size);
    }
}
