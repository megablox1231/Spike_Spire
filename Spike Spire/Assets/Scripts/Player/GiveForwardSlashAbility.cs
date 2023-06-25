using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

/// <summary>
/// Gives player forward slash with corresponding animation and audio.
/// </summary>
[RequireComponent(typeof(Text))]
public class GiveForwardSlashAbility : MonoBehaviour {

    public void GiveAbility() {
        PlayerControls controls = InputManager.GetInputActions();
        GetComponent<Text>().text += $"Press {controls.Gameplay.ForwardSlash.GetBindingDisplayString(0)} (keyboard) or " +
            $"{controls.Gameplay.ForwardSlash.GetBindingDisplayString(1)} (controller) to propel off vertical sufaces.";

        GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().hasForwardSlash = true;
        GameMaster.gm.PlayerHasForwardSlash = true;
        GetComponent<Animator>().enabled = true;
        GetComponent<AudioSource>().Play();
    }
}
