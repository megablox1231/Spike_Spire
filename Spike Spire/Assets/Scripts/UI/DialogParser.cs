using Doublsb.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Accesses dialog data from specified JSON file.
/// Data stored as Sentence instances in Dialog instances in one AllDialog instance.
/// </summary>
public class DialogParser
{
    public static List<DialogData> ParseDialog(DialogManager dialogManager, TextAsset dialogFile, string fileSection) {
        AllDialog allDialog = JsonUtility.FromJson<AllDialog>(dialogFile.text);
        List<DialogData> dialogDatas = new List<DialogData>();

        for (int i = 0; i < allDialog.allDialogs.Count; i++) {
            if (allDialog.allDialogs[i].id == fileSection) {
                foreach (Sentence sentence in allDialog.allDialogs[i].dialogTexts) {
                    dialogDatas.Add(new DialogData(sentence.text, sentence.character));
                }
                break;
            }
        }

        return dialogDatas;
    }

    [System.Serializable]
    public class AllDialog {
        public List<Dialog> allDialogs;
    }

    [System.Serializable]
    public class Dialog {
        public string id;
        public List<Sentence> dialogTexts;
    }

    [System.Serializable]
    public class Sentence {
        public string character;
        public string text;
    }
}
