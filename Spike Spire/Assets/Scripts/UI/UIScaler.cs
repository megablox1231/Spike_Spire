using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

/// <summary>
/// Sets the scale factor of the CanvasScalar and toggles pixel perfect.
/// </summary>
[RequireComponent(typeof(CanvasScaler))]
public class UIScaler : MonoBehaviour {

    [SerializeField] int baseHeight = 270;
    [SerializeField] int baseWidth = 480;
    [SerializeField] int widestElement = 510;

    [SerializeField] CanvasScaler scaler;

    int[] res = new int[2];
    bool isPixelPerfect = true;

    void Start() {
        ScaleUI();
        SetPixelPerfectMode(isPixelPerfect);
    }

    void LateUpdate() {
        if (isPixelPerfect && (res[0] != Screen.height || res[1] != Screen.width)) {
            ScaleUI();
        }

    }

    void ScaleUI() {
        scaler.scaleFactor = Mathf.FloorToInt((float)Screen.height / baseHeight);
        if (widestElement * scaler.scaleFactor > Screen.width) {
            scaler.scaleFactor = Mathf.FloorToInt((float)Screen.width / baseWidth);
        }

        res[0] = Screen.height;
        res[1] = Screen.width;
    }

    public void SetPixelPerfectMode(bool pixelPerfect) {
        isPixelPerfect = pixelPerfect;
        if (isPixelPerfect) {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            Camera.main.GetComponent<PixelPerfectCamera>().stretchFill = false;
        }
        else {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            Camera.main.GetComponent<PixelPerfectCamera>().stretchFill = true;
        }
    }
}
