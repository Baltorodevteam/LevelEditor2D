using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class SaveScreen : BaseScreen
{
    [SerializeField]
    Text currentText;

    bool bSaveCurrentRoomOnly = false;

    public void SetSaveCurrentRoomOnly(bool b)
    {
        bSaveCurrentRoomOnly = b;
    }


    string[] kbrdLetters = 
    {
        "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "BACK",
        "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P",
        "A", "S", "D", "F", "G", "H", "J", "K", "L", "ENTER",
        "Z", "X", "C", "V", "B", "N", "M", "_", "", "",
        " "
    };


    string currentFileName;


    private void OnEnable()
    {
        OnEnterScreen(PreviousScreen);

        currentFileName = "";
        UpdateText();
    }

    public void OnKeyboardButton(int buttonIndex)
    {
        if(kbrdLetters[buttonIndex].Equals("BACK"))
        {
            if(currentFileName.Length > 0)
            {
                currentFileName = currentFileName.Substring(0, currentFileName.Length - 1);
                UpdateText();
            }
        }
        else if (kbrdLetters[buttonIndex].Equals("ENTER"))
        {
            OnSave();
        }
        else
        {
            if(currentFileName.Length < 16)
            {
                currentFileName += kbrdLetters[buttonIndex];
                UpdateText();
            }
        }

    }

    void UpdateText()
    {
        currentText.text = currentFileName;
    }

    public void OnSave()
    {
        StartCoroutine(_OnSave());
    }

    IEnumerator _OnSave()
    {
        yield return new WaitForSeconds(0.1f);

        string filePath = Path.Combine(Application.persistentDataPath, currentFileName);
        if (Path.GetExtension(filePath) != ".LVL")
        {
            filePath = Path.ChangeExtension(filePath, ".LVL");
        }
        JsonWriterReader.Save(filePath, SystemData.Instance.GetLevelInfo());

        yield return new WaitForSeconds(0.1f);
        PreviousScreen.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

} 