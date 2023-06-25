using UnityEngine;

/// <summary>
/// Moves this object alongside camera with parrallax effect.
/// </summary>
public class Parallax : MonoBehaviour {

    [SerializeField] Vector2 camOffset;
    [SerializeField] float parallexEffectX;
    [SerializeField] float parallexEffectY;
    [SerializeField] float PixelsPerUnit;

    [SerializeField] Bounds area; // optional bounding box that parralax effect will not move past

    GameObject cam;
    Vector2 startPos;
    Vector3 vel = Vector3.zero;

    void Start() {
        cam = Camera.main.gameObject;
        startPos = new Vector2(transform.position.x, transform.position.y);
    }

    void LateUpdate() {
        if (cam != null && area.Contains(cam.transform.position)) {
            float distX = ((cam.transform.position.x - camOffset.x) * parallexEffectX);
            float distY = ((cam.transform.position.y - camOffset.y) * parallexEffectY);
            transform.position = new Vector3(startPos.x + distX, startPos.y + distY, transform.position.z);
        }
    }

    Vector3 PixelPerfectClamp(Vector3 moveVector, float pixelsPerUnit) {
        Vector3 vectorInPixels = new Vector3(Mathf.CeilToInt(moveVector.x * pixelsPerUnit),
            Mathf.CeilToInt(moveVector.y * pixelsPerUnit), Mathf.CeilToInt(moveVector.z * pixelsPerUnit));
        return vectorInPixels / pixelsPerUnit;
    }
}