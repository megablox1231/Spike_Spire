using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class UIScaler : MonoBehaviour {

    [SerializeField]
    private int baseHeight = 270;
    [SerializeField]
    private int baseWidth = 480;
    [SerializeField]
    private int widestElement = 510;

    private RectTransform rectTransform;
    private CanvasScaler scaler;
    private int[] res = new int[2];
    private int newheight = 0;
    private int newwidth = 0;

    private void Start() {
        scaler = GetComponent<CanvasScaler>();
        rectTransform = GetComponent<RectTransform>();
        ScaleUI();
    }

    private void LateUpdate() {

        if (res[0] != Screen.height || res[1] != Screen.width) {
            //Debug.Log("Scale Changed");
            ScaleUI();
        }

    }

    private void ScaleUI() {
        scaler.scaleFactor = Mathf.FloorToInt((float)Screen.height / baseHeight);
        if (widestElement * scaler.scaleFactor > Screen.width) {
            scaler.scaleFactor = Mathf.FloorToInt((float)Screen.width / baseWidth);
        }

        res[0] = Screen.height;
        res[1] = Screen.width;
    }
}
