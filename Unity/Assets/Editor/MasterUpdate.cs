using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Linq;

public class MasterUpdate : EditorWindow
{
    public enum MASTER_TYPE
    {
        MESSAGE = 0,
    }
    string message = "None";

    [MenuItem("Tools/Master Update")]
    static void Open()
    {
        GetWindow<MasterUpdate>();
    }

    void OnGUI()
    {
        // current
        {
            EditorGUILayout.LabelField(message);
            EditorGUILayout.Space();
        }

        var masterTypeArray = Enum.GetValues(typeof(MASTER_TYPE));
        foreach (MASTER_TYPE masterType in masterTypeArray)
        {
            if (GUILayout.Button(calcButtonText(masterType), GUILayout.Width(200.0f)))
            {
                EditorCoroutine.Start(updateMasterFileCoroutine(masterType));
            }

            EditorGUILayout.Space();
        }
    }

    private IEnumerator updateMasterFileCoroutine(MASTER_TYPE masterType)
    {
        message = "Update";

        var url = calcMasterSheetUrl(masterType);
        var download = new WWW(url);
        while (!download.isDone)
        {
            yield return new WaitForSeconds(1.0f);
            message += ".";
        }

        var errorMessage = download.error;
        if (errorMessage != null && errorMessage != string.Empty)
        {
            message = "Error Message : " + errorMessage;
        }
        else
        {
            var filePath = calcUpdateMasterFilePath(masterType);
            var fileText = download.text;

            File.WriteAllText(filePath, fileText);
            message = "Complete";
            AssetDatabase.Refresh();
        }
    }

    // calc
    private string calcButtonText(MASTER_TYPE masterType)
    {
        string text = string.Empty;

        switch (masterType)
        {
            case MASTER_TYPE.MESSAGE:       text = "Message Master Update"; break;
        }

        return text;
    }
    private string calcMasterSheetUrl(MASTER_TYPE masterType)
    {
        var url = string.Empty;

        // common
        {
//            url += "https://script.google.com/a/macros/mail.kiteretsu.jp/s/AKfycbyqxG4bwWtAQsL5oJ4n-lmnL-hso_NGZSfhRvkiaMGOSbuldAM/exec";
            url += "?sheetId=" + "1kBCJgA90OGGZhf3t8edXPY8UL27oPxLb1Qao3H3wW9I";
        }

        switch (masterType)
        {
            case MASTER_TYPE.MESSAGE: url += "&sheetName=" + "MessageMaster"; break;
        }

        return url;
    }
    private string calcUpdateMasterFilePath(MASTER_TYPE masterType)
    {
        string filePath = Application.dataPath + "/Data/Resources/Master/";

        switch (masterType)
        {
            case MASTER_TYPE.MESSAGE:       filePath += "MessageMaster.json"; break;
        }

        return filePath;
    }
}
