using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class GiveForwardSlashAbility : MonoBehaviour {

    public void GiveAbility() {
        GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().hasForwardSlash = true;
        GameMaster.gm.playerHasForwardSlash = true;
        GetComponent<Animator>().enabled = true;
        GetComponent<AudioSource>().Play();
    }
}
