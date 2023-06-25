using UnityEngine;

/// <summary>
/// Unlocks given lockBlock upon collision.
/// </summary>
public class KeyAction : MonoBehaviour {

    [SerializeField] GameObject lockBlock;

    void OnTriggerEnter2D(Collider2D collision) {
        lockBlock.GetComponent<Animator>().enabled = true;
        Destroy(gameObject);
    }
}
