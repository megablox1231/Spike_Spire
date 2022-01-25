using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloserControl : MonoBehaviour
{
    [SerializeField]
    private GameObject[] closers;

    public float wait;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        StartCoroutine(AnimateClosers());
    }

    private IEnumerator AnimateClosers() {
        for (int i = 0; i < closers.Length; i++) {
            closers[i].GetComponent<Animator>().enabled = true;
            yield return new WaitForSeconds(wait);
        }
    }
}
