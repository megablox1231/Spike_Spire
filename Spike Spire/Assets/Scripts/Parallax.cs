using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

    public Vector2 camOffset;
    public float parallexEffectX;
    public float parallexEffectY;
    public float PixelsPerUnit;

    [SerializeField]
    private Bounds area;

    private GameObject cam;
    private Vector2 startPos;
    private Vector3 vel = Vector3.zero;

    void Start() {
        cam = Camera.main.gameObject;
        startPos = new Vector2(transform.position.x, transform.position.y);
    }

    void LateUpdate() {
        if (area.Contains(cam.transform.position)) {
            //float temp = (cam.transform.position.x * (1 - parallexEffect));
            float distX = ((cam.transform.position.x - camOffset.x) * parallexEffectX);
            float distY = ((cam.transform.position.y - camOffset.y) * parallexEffectY);
            transform.position = new Vector3(startPos.x + distX, startPos.y + distY, transform.position.z);
            //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(startPos.x + distX, startPos.y + distY, transform.position.z), ref vel, 0.01f);

            //if (temp > startpos + length) startpos += length;
            //else if (temp < startpos - length) startpos -= length;
        }
    }

    private Vector3 PixelPerfectClamp(Vector3 moveVector, float pixelsPerUnit) {
        Vector3 vectorInPixels = new Vector3(Mathf.CeilToInt(moveVector.x * pixelsPerUnit), Mathf.CeilToInt(moveVector.y * pixelsPerUnit), Mathf.CeilToInt(moveVector.z * pixelsPerUnit));
        return vectorInPixels / pixelsPerUnit;
    }
}