using UnityEngine;
using UnityEngine.UI;

public class ScrollListSpawnerElement : MonoBehaviour
{
    [SerializeField]
    GameObject selector;
    [SerializeField]
    Image iconImages;
    [SerializeField]
    Text nrText;

    public void SetEnemyData(int nIndex, bool selected, float localPositionY)
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
    }


    public void SetIcon(Sprite s)
    {
        iconImages.sprite = s;
    }

    public void SetIndexText(int i)
    {
        nrText.text = "" + i;
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