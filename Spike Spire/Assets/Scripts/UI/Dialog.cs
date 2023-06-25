using Doublsb.Dialog;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

/// <summary>
/// Handles dialog events including connecting dialog, activating player alert,
/// and displyaing dialog.
/// </summary>
public class Dialog : MonoBehaviour {

    [SerializeField] DialogManager dialogManager;
    [SerializeField] GameObject printer;
    [SerializeField] GameObject dialogAlert;

    [SerializeField] TextAsset dialogFile;
    [SerializeField] string fileSection;

    [SerializeField] UnityEvent onEndEvent;

    GameObject curAlert;
    bool inRange;
    bool inputConnected;

    void OnTriggerEnter2D(Collider2D col) {
        if (!inRange && col.tag == "Player") {
            if (curAlert == null) {
                curAlert = Object.Instantiate(dialogAlert, GameMaster.gm.GetCurPlayer().transform);
            }
            if (!inputConnected) {
                GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().ConnectDialog(StartDialog, dialogManager);
                inputConnected = true;
            }
            curAlert.SetActive(true);
            inRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.tag == "Player") {
            GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().DisconnectDialog();
            inRange = false;
            inputConnected = false;
            curAlert.SetActive(false);
        }
    }

    void StartDialog() {
        printer.SetActive(true);
        List<DialogData> dialogDatas = DialogParser.ParseDialog(dialogManager, dialogFile, fileSection);
        dialogDatas.Last().Callback = () => OnDialogEnd();
        dialogManager.Show(dialogDatas);
        curAlert.SetActive(false);
    }

    public void StartDialog(string fileSect) {
        if (!inputConnected) {
            GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().ConnectContinueDialog(StartDialog, dialogManager);
        }

        printer.SetActive(true);
        List<DialogData> dialogDatas = DialogParser.ParseDialog(dialogManager, dialogFile, fileSect);
        dialogDatas.Last().Callback = () => OnDialogEnd();
        dialogManager.Show(dialogDatas);
    }

    void OnDialogEnd() {
        GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().EnableMovement();
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        if (poly != null) {
            poly.enabled = false;
        }
        else {
            GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().DisconnectDialog();
        }
        if (onEndEvent != null) {
            onEndEvent.Invoke();
        }
    }
}
