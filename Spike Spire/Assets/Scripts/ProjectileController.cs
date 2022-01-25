using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{

    public float shotTimer;
    public float shotSpeed;
    public enum shotDirection { Up, Down, Left, Right };
    public shotDirection shotDirect;

    public Transform shotPrefab;

    private bool dontShoot;
    private List<Transform> projtls;

    // Start is called before the first frame update
    void Start()
    {
        dontShoot = false;
        projtls = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dontShoot) {
            projtls.Add(Instantiate(shotPrefab, transform));
            StartCoroutine(Wait());
        }

        if (shotDirect == shotDirection.Up) {
            foreach (Transform triangle in projtls) {
                triangle.position = new Vector3(triangle.position.x, triangle.position.y + shotSpeed, triangle.position.z);
            }
        }

        if (shotDirect == shotDirection.Down) {
            foreach (Transform triangle in projtls) {
                triangle.position = new Vector3(triangle.position.x, triangle.position.y - shotSpeed, triangle.position.z);
            }
        }

        if (shotDirect == shotDirection.Left) {
            foreach (Transform triangle in projtls) {
                triangle.position = new Vector3(triangle.position.x - shotSpeed, triangle.position.y, triangle.position.z);
            }
        }

        if (shotDirect == shotDirection.Right) {
            foreach (Transform triangle in projtls) {
                triangle.position = new Vector3(triangle.position.x + shotSpeed, triangle.position.y + shotSpeed, triangle.position.z);
            }
        }
    }

    //Corountine that will wait.
    private IEnumerator Wait() {
        dontShoot = true;
        yield return new WaitForSeconds(shotTimer);
        dontShoot = false;
    }
}
