using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class ScrollListElement : MonoBehaviour
{
    [SerializeField]
    GameObject selector;
    [SerializeField]
    Image[] iconImages;

    int listPositionIndex;
    string iconName;
    ScrollList parentList;



    public void SetListPositionIndex(int i)
    {
        listPositionIndex = i;
    }

    public void SetParentList(ScrollList l)
    {
        parentList = l;
    }

    public void SetIcon(Sprite s)
    {
        for(int  i = 0; i < iconImages.Length; i++)
        {
            iconImages[i].sprite = s;
        }
    }

    public void SetName(string s)
    {
        iconName = s;
    }

    public string GetName()
    {
        return iconName;
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

    public void OnSelect()
    {
        parentList.OnSelect(listPositionIndex);
    }

}