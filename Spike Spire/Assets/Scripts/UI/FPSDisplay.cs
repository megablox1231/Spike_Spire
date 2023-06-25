using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays fps.
/// </summary>
public class FPSDisplay : MonoBehaviour {
    public float timer, refresh, avgFramerate;
    string display = "{0} FPS";
    Text m_Text;

    void Start() {
        m_Text = GetComponent<Text>();
    }

   void Update() {
        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;

        if (timer <= 0) avgFramerate = (int)(1f / timelapse);
        m_Text.text = string.Format(display, avgFramerate.ToString());
    }
}
