using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class ScrollListFilenameElement : MonoBehaviour
{
    [SerializeField]
    GameObject selector;
    [SerializeField]
    Text filenameText;

    int nIndex = -1;

    public delegate void UpdateExtAction(int nIndex);

    UpdateExtAction extAction;


    public void SetUpdateExtAction(UpdateExtAction f)
    {
        extAction = f;
    }

    public void SetIndex(int i)
    {
        nIndex = i;
    }

    public void SetFilename(string filename)
    {
        filenameText.text = filename;
    }

    public string GetFilename()
    {
        return filenameText.text;
    }

    public void SelectMe(bool b)
    {
        if(b)
        {
            selector.SetActive(true);
        }
        else
        {
            selector.SetActive(false);
        }
    }

    public bool GetSelect()
    {
        return selector.activeInHierarchy;
    }

    public void OnAccept()
    {
        extAction(nIndex);
    }

}