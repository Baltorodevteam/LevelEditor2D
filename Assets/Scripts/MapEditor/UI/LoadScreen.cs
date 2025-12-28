using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class LoadScreen : BaseScreen
{
    [SerializeField]
    ScrollRect scrollRect;
    [SerializeField]
    ScrollListFilenameElement elementPrefab;
    [SerializeField]
    RoomScreen roomScreen;


    List<ScrollListFilenameElement> list;
    int currentFilenameIndex = 0;

    // lvl - level, rom - room, crv - curves
    public static string fileExtension;


    private void OnEnable()
    {
        OnEnterScreen(PreviousScreen);
        InitFileList();
    }


    float prefListElementHeight = 34;// LayoutUtility.GetPreferredHeight(elementPrefab.GetComponent<RectTransform>());

    int visibleElements = 0;
    float deltaScrollPerPage = 0.0f;
    float targetScrollPosition = 0.0f;
    float targetScrollTime = 0;

    float GetListElementLocalPositionY(int nIndex)
    {
        return -(1 + prefListElementHeight/2 + nIndex * (prefListElementHeight + 1));
    }

    void InitFileList()
    {
        foreach (Transform child in scrollRect.content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }


        string path = Application.persistentDataPath;
        string[] jsonFiles = Directory.GetFiles(path, "*." + fileExtension);
        int listSize = jsonFiles.Length;

        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, 1 + listSize * (prefListElementHeight + 1));

        list = new List<ScrollListFilenameElement>();

        for (int i = 0; i < listSize; i++)
        {
            ScrollListFilenameElement el = Instantiate(elementPrefab, scrollRect.content);
            el.transform.localPosition = new Vector3(0, GetListElementLocalPositionY(i), 0);
            el.SetFilename(Path.GetFileName(jsonFiles[i]));
            el.SetIndex(i);
            el.SetUpdateExtAction(OnLoad);
            
            list.Add(el);
        }

        scrollRect.content.anchoredPosition = Vector2.up * 0;


        float fullHeight = scrollRect.GetComponent<RectTransform>().sizeDelta.y;
        visibleElements = (int)(fullHeight / (prefListElementHeight + 1));

        int scrolledElements = listSize - visibleElements;
        float deltaScrollPerElement = 1.0f / scrolledElements;

        int nrOfPages = listSize / visibleElements;

        deltaScrollPerPage = 0.0f;
        if (nrOfPages > 0)
        {
            deltaScrollPerPage = 1.0f / (float)nrOfPages;
        }

        deltaScrollPerPage = deltaScrollPerElement * visibleElements;

        currentFilenameIndex = 0;
        UpdateList();
        UpdateTargetScrollPosition();
    }

    void UpdateTargetScrollPosition()
    {
        targetScrollPosition = deltaScrollPerPage * (float)(currentFilenameIndex / visibleElements);
        if (targetScrollPosition < 0)
            targetScrollPosition = 0;
        else if (targetScrollPosition > 1)
            targetScrollPosition = 1;
        targetScrollPosition = 1.0f - targetScrollPosition;
        targetScrollTime = 0;
    }

    void UpdateList()
    {
        for(int i = 0; i < list.Count; i++)
        {
            list[i].SelectMe(i == currentFilenameIndex);
        }
    }

    public void OnUp()
    {
        if(currentFilenameIndex > 0)
        {
            currentFilenameIndex--;
            UpdateList();
            UpdateTargetScrollPosition();
        }
    }

    public void OnDown()
    {
        if (currentFilenameIndex < list.Count - 1)
        {
            currentFilenameIndex++;
            UpdateList();
            UpdateTargetScrollPosition();
        }
    }

    private void Update()
    {
        targetScrollTime += Time.deltaTime;
        if (targetScrollTime < 0.8f)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, targetScrollPosition, Time.deltaTime * 20.0f);
        }
    }

    public void OnLoadCurrent()
    {
        OnLoad(currentFilenameIndex);
    }

    public void OnLoad(int index)
    {
        if(list.Count == 0)
        {
            return;
        }

        currentFilenameIndex = index;
        UpdateList();

        StartCoroutine(_OnLoad());
    }

    IEnumerator _OnLoad()
    {
        yield return new WaitForSeconds(0.1f);
        SystemData.backFromTest = false;
        SystemData.editorMode = true;

        if(string.Equals(fileExtension, "ROM"))
        {
            yield return new WaitForSeconds(0.1f);

            string fileName = Application.persistentDataPath + "/" + list[currentFilenameIndex].GetFilename();
            JsonWriterReader.LoadRoom(fileName, SystemData.Instance.GetLevelData().GetCurrentRoom());

            yield return new WaitForSeconds(0.2f);

        }
        else //if(string.Equals(fileExtension, "LVL"))
        {
            yield return new WaitForSeconds(0.1f);

            string filePath = Application.persistentDataPath + "/" + list[currentFilenameIndex].GetFilename();
            SystemData.Instance.GetLevelInfo().LoadFromJson(filePath);

            yield return new WaitForSeconds(0.2f);
        }

        BackToPreviousScreen();
    }

} 