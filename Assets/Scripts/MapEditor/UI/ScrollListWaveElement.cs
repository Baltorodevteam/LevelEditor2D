using UnityEngine;
using UnityEngine.UI;

public class ScrollListWaveElement : MonoBehaviour
{
    [SerializeField]
    GameObject selector;
    [SerializeField]
    Image iconImages;
    [SerializeField]
    Text nrText;
    [SerializeField]
    Text nameText;

    public void SetData(int nIndex, bool selected, string name, float localPositionY)
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
        SetName(name);
    }


    public void SetIcon(Sprite s)
    {
        iconImages.sprite = s;
    }

    public void SetIndexText(int i)
    {
        nrText.text = "" + i;
    }

    public void SetName(string name) {
        nameText.text = name;
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