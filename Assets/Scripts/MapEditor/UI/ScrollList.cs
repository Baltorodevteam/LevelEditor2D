using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class ScrollList : MonoBehaviour
{
    [SerializeField]
    ScrollRect scrollRect;
    [SerializeField]
    ScrollListElement elementPrefab;
    [SerializeField]
    Image tooltipImage;
    [SerializeField]
    Text tooltipText;


    public delegate void UpdateExtAction(int nIndex);

    UpdateExtAction extAction;


    int sizeOfElements = 0;
    int currentElement = -1;

    List<ScrollListElement> list;


    public void SetUpdateExtAction(UpdateExtAction f)
    {
        extAction = f;
    }

    public void SetSize(int i)
    {
        sizeOfElements = i;
        Prepare();
    }

    public int GetSize()
    {
        return sizeOfElements;

    }

    int visibleElements = 0;
    float deltaScrollPerPage = 0.0f;
    float targetScrollPosition = 0.0f;
    float targetScrollTime = 0;

    public void Prepare()
    {
        list = new List<ScrollListElement>();

        float prefWidth = 34;// LayoutUtility.GetPreferredWidth(elementPrefab.GetComponent<RectTransform>());
        scrollRect.content.sizeDelta = new Vector2(sizeOfElements * (prefWidth +2), scrollRect.content.sizeDelta.y);

        for(int i = 0; i < sizeOfElements; i++)
        {
            ScrollListElement el = Instantiate(elementPrefab, scrollRect.content);
            el.transform.localPosition = new Vector3(3 + i * (prefWidth + 1), 0, 0);
            el.SetParentList(this);
            el.SetListPositionIndex(i);
            list.Add(el);
        }

        scrollRect.content.anchoredPosition = Vector2.right * 0;

        // 
        float fullWidth = scrollRect.GetComponent<RectTransform>().sizeDelta.x;
        visibleElements = (int)(fullWidth / (prefWidth + 2));

        int nrOfPages = sizeOfElements / visibleElements;

        deltaScrollPerPage = 0.0f;
        if (nrOfPages > 0)
        {
            deltaScrollPerPage = 1.0f / (float)nrOfPages;
        }
        targetScrollTime = 0;
        
        //
        tooltipImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        targetScrollTime += Time.deltaTime;
        if(targetScrollTime < 0.8f)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targetScrollPosition, Time.deltaTime * 20.0f);
        }
    }

    public void ForceSelect(int nIndex)
    {
        InternalSelect(nIndex);
    }

    public int GetCurrentElement()
    {
        return currentElement;
    }

    private void Awake()
    {
    }

    public void SetName(int nIndex, string name)
    {
        if(nIndex < list.Count)
        {
            list[nIndex].SetName(name);
        }
    }

    public void SetIcon(int nIndex, Sprite sprite)
    {
        if (nIndex < list.Count)
        {
            list[nIndex].SetIcon(sprite);
        }
    }

    public void Clear()
    {
        foreach (Transform child in scrollRect.content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        list = null;
        currentElement = -1;
        sizeOfElements = 0;
    }

    private void OnEnable()
    {
    }

    bool InternalSelect(int nIndex)
    {
        if (nIndex < list.Count)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SelectMe(i == nIndex);
            }
            currentElement = nIndex;

            targetScrollPosition = deltaScrollPerPage * (float)(nIndex / visibleElements);
            targetScrollTime = 0;

            SetTooltipOnGridIndex(nIndex);

            return true;
        }

        return false;
    }

    public void OnSelect(int nIndex)
    {
        if(InternalSelect(nIndex))
        {
            if(extAction != null)
            {
                extAction(currentElement);
            }
        }
    }

    public void UpdateGrid()
    {
    }

    public void OnPointerEnter(int nIndex)
    {
        SetTooltipOnGridIndex(nIndex);
    }

    public void SetTooltipOnGridIndex(int nIndex) // nIndex (0 - 7)
    {
        if (hideTooltipCoroutine != null)
        {
            StopCoroutine(hideTooltipCoroutine);
            hideTooltipCoroutine = null;
        }

        /*
        if (currentPage * sizeOfGrid + nIndex >= nameList.Length || nameList[currentPage * sizeOfGrid + nIndex] == null || nameList[currentPage * sizeOfGrid + nIndex].Length == 0)
        {
            tooltipImage.gameObject.SetActive(false);
            return;
        }
        */

        tooltipImage.gameObject.SetActive(true);

        tooltipText.text = list[nIndex].GetName();

        RectTransform rt = list[nIndex].GetComponent<RectTransform>();

        tooltipImage.transform.position = new Vector3(list[nIndex].transform.position.x + 50, list[nIndex].transform.position.y - 2 * rt.rect.height, 0);

        RectTransform rect = tooltipImage.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(tooltipText.preferredWidth + 10, rect.sizeDelta.y);

        hideTooltipCoroutine = StartCoroutine(WaitAndHideTooltip(1.5f));
    }

    Coroutine hideTooltipCoroutine = null;
    IEnumerator WaitAndHideTooltip(float time)
    {
        yield return new WaitForSeconds(time);

        tooltipImage.gameObject.SetActive(false);
        hideTooltipCoroutine = null;
    }

}