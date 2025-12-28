using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class ScrollListEnemyElement : MonoBehaviour
{
    [SerializeField]
    GameObject selector;
    [SerializeField]
    Image iconImages;
    [SerializeField]
    Text nrText;
    [SerializeField]
    Text delayText;

    EnemyData enemyData;

    public void SetEnemyData(int nIndex, int delayInMs, bool selected, float localPositionY)
    {
        selector.SetActive(selected);
        if(selected)
        {
            transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
            transform.localPosition = new Vector3(2, localPositionY + 1, 0);
        }
        else
        {
            transform.localScale = Vector3.one;
            transform.localPosition = new Vector3(7, localPositionY, 0);
        }
        SetIndexText(nIndex + 1);
        SetDelayText(delayInMs);
    }


    public void SetIcon(Sprite s)
    {
        iconImages.sprite = s;
    }

    public void SetIndexText(int i)
    {
        nrText.text = "" + i;
    }

    public void SetDelayText(int i)
    {
        delayText.text = "" + i + "ms";
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

}