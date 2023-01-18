using Doublsb.Dialog;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Dialog : MonoBehaviour {

    [SerializeField]
    private DialogManager dialogManager;
    [SerializeField]
    private GameObject printer;
    [SerializeField]
    private GameObject dialogAlert;
    [SerializeField]
    private TextAsset dialogFile;
    [SerializeField]
    private string fileSection;
    [SerializeField]
    private UnityEvent onEndEvent;
    private GameObject curAlert;

    private bool inRange;
    private bool inputConnected;

    private void OnTriggerEnter2D(Collider2D col) {
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

    private void OnTriggerExit2D(Collider2D col) {
        if (col.tag == "Player") {
            GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().DisconnectDialog();
            inRange = false;
            inputConnected = false;
            curAlert.SetActive(false);
        }
    }

    private void StartDialog() {
        printer.SetActive(true);
        List<DialogData> dialogDatas = DialogParser.ParseDialog(dialogManager, dialogFile, fileSection);
        dialogDatas.Last().Callback = () => OnDialogEnd();
        dialogManager.Show(dialogDatas);
        curAlert.SetActive(false);
    }

    public void StartDialog(string fileSect) {
        printer.SetActive(true);
        List<DialogData> dialogDatas = DialogParser.ParseDialog(dialogManager, dialogFile, fileSect);
        dialogDatas.Last().Callback = () => OnDialogEnd();
        dialogManager.Show(dialogDatas);
    }

    private void OnDialogEnd() {
        GameMaster.gm.GetCurPlayer().GetComponent<PlayerInput>().EnableMovement();
        GetComponent<PolygonCollider2D>().enabled = false;
        if (onEndEvent != null) {
            onEndEvent.Invoke();
        }
    }
}
