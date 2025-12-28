using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RoomScreen : BaseScreen
{
    [SerializeField]
    ScrollList categoriesScrollList;
    [SerializeField]
    ScrollList objectsScrollList;
    [SerializeField]
    LevelMiniMap levelMiniMap;
    [SerializeField]
    GeneratorPanel generatorPanel;


    public static RoomScreen Instance = null;

    int currentCategory = -1;
    int currentObject = -1;

    int currentAltitude = 1; // 0 - under water, 1 - on the water


    public int GetCurrentCategory()
    {
        return currentCategory;
    }

    public int GetCurrentObject()
    {
        return currentObject;
    }


    private void OnEnable()
    {
        OnEnterScreen(PreviousScreen);

        Instance = this;

        categoriesScrollList.SetUpdateExtAction(InitObjectsList);
        objectsScrollList.SetUpdateExtAction(CreateObjectTemplate);

        UndoManager.ClearUndoList();
        UndoManager.ClearRedoList();

        InitCategoryList();
        InitObjectsList(currentCategory);

        levelMiniMap.SetCursorAtCurremtRoom(SystemData.Instance.GetLevelData());
        DrawLevelMiniMap(false);

        levelMiniMap.SetOnClickEventExtAction(OnChangeRoomExtEventAction);

        OnChangeAltitudeMode(1); // 1 - on the water
    }

    void OnChangeRoomExtEventAction()
    {
        OnChangeAltitudeMode(1);
    }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
    }


    public void MoveLevelMiniMapCursor(int dx, int dy)
    {
        levelMiniMap.MoveCursor(dx, dy);
    }

    public void DrawLevelMiniMap(bool withCursor)
    {
        levelMiniMap.DrawLevelFromLevelDataToTexture(SystemData.Instance.GetLevelData(), withCursor);
    }

    public void UpdateCurrentRoom()
    {
        RoomData rd = levelMiniMap.GetRoomAtCursor(SystemData.Instance.GetLevelData());
        if(rd != null && rd != SystemData.Instance.GetLevelData().GetCurrentRoom())
        {
            SystemData.Instance.GetLevelData().SetCurrentRoom(rd);
            levelMiniMap.SetCursorAtCurremtRoom(SystemData.Instance.GetLevelData());
            OnChangeAltitudeMode(1);
        }
    }

    public void UpdateViewForCurrentRoom()
    {
        RoomData rd = SystemData.Instance.GetLevelData().GetCurrentRoom();
        if (rd != null)
        {
            levelMiniMap.SetCursorAtCurremtRoom(SystemData.Instance.GetLevelData());
            DrawLevelMiniMap(false);
            OnChangeAltitudeMode(1);
        }
    }

    void InitCategoryList()
    {
        categoriesScrollList.SetSize(SystemData.Instance.GetEditorData().objectCategories.Length);

        for (int i = 0; i < SystemData.Instance.GetEditorData().objectCategories.Length; i++)
        {
            categoriesScrollList.SetName(i, SystemData.Instance.GetEditorData().objectCategories[i].categoryName);
            categoriesScrollList.SetIcon(i, SystemData.Instance.GetEditorData().objectCategories[i].categoryIcon);
        }

        currentCategory = 0;
        categoriesScrollList.ForceSelect(currentCategory);
    }

    void InitObjectsList(int nCategoryIndex)
    {
        objectsScrollList.Clear();

        objectsScrollList.SetSize(SystemData.Instance.GetEditorData().objectCategories[nCategoryIndex].objects.Length);

        for (int i = 0; i < SystemData.Instance.GetEditorData().objectCategories[nCategoryIndex].objects.Length; i++)
        {
            objectsScrollList.SetName(i, SystemData.Instance.GetEditorData().objectCategories[nCategoryIndex].objects[i].name);
            objectsScrollList.SetIcon(i, SystemData.Instance.GetEditorData().objectCategories[nCategoryIndex].icons[i]);
        }

        currentCategory = nCategoryIndex;

        currentObject = 0;
        objectsScrollList.ForceSelect(currentObject);
    }

    public void SelectNextCategory()
    {
        currentCategory = categoriesScrollList.GetCurrentElement();
        int categoriesSize = categoriesScrollList.GetSize();
        if(currentCategory < categoriesSize - 1)
        {
            currentCategory++;
            categoriesScrollList.ForceSelect(currentCategory);
            InitObjectsList(currentCategory);
        }
    }

    public void SelectPrevCategory()
    {
        currentCategory = categoriesScrollList.GetCurrentElement();
        if (currentCategory > 0)
        {
            currentCategory--;
            categoriesScrollList.ForceSelect(currentCategory);
            InitObjectsList(currentCategory);
        }
    }

    public void SelectNextObject()
    {
        currentObject = objectsScrollList.GetCurrentElement();
        int objectsSize = objectsScrollList.GetSize();
        if (currentObject < objectsSize - 1)
        {
            currentObject++;
            objectsScrollList.ForceSelect(currentObject);
        }
    }

    public void SelectPrevObject()
    {
        currentObject = objectsScrollList.GetCurrentElement();
        if (currentObject > 0)
        {
            currentObject--;
            objectsScrollList.ForceSelect(currentObject);
        }
    }

    public void CreateObjectTemplate(int objectID)
    {
        if (SystemData.Instance.GetLevelData() == null)
        {
            return;
        }

        currentObject = objectID;

        if (currentCategory >= 0 && currentObject >= 0)
        {
            TemplateObject.Instance.CreateObject(SystemData.Instance.GetEditorData().objectCategories[currentCategory].objects[currentObject]);
        }

        if(SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayerIndex() < 3)
        {
            OnChangeAltitudeMode(0);
        }
        else
        {
            OnChangeAltitudeMode(1);
        }
    }

    public void CreateObjectTemplateOnLayer()
    {
        TemplateObject.Instance.CreateOnLayer();

        if (SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayerIndex() < 3)
        {
            OnChangeAltitudeMode(0);
        }
        else
        {
            OnChangeAltitudeMode(1);
        }
    }

    public void OnChangeAltitudeMode(int alt) // 0, 1
    {
        // 0 - under water, 1 - on the water
        if(alt == 0)
        {
            currentAltitude = 0;
        }
        else if(alt == 1)
        {
            currentAltitude = 1;
        }

        SystemData.Instance.GetLevelData().GetCurrentRoom().EnableAltMode(currentAltitude);
        StartCoroutine(SystemData.Instance.GetLevelData().GetCurrentRoom().WaitAndUpdateUnderWaterTerrain(SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(2), 0));
    }

    public void OnUndo()
    {
        TemplateObject.Instance.Clear();
        UndoManager.Undo();
    }

    public void OnRedo()
    {
        TemplateObject.Instance.Clear();
        UndoManager.Redo();
    }

    public void OnTest()
    {
        TemplateObject.Instance.Clear();

        SystemData.editorMode = false;
        SystemData.backFromTest = true;
        SystemData.startCurrentRoom = false;

        SystemData.Instance.GetLevelData().SaveAsJson("tmpLevel.tmp");

        SceneManager.LoadScene(2);
    }

    public void OnGeneratePanel()
    {
        OnAutoGenerate();
    }

    public void OnAutoGenerate()
    {
        for (int l = 1; l < SystemData.NR_OF_LAYERS; l++)
        {
            SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(l).ClearList();
        }

        AreaGenerator ag = new AreaGenerator();
        ag.Generate(SystemData.Instance.GetLevelData(), SystemData.Instance.GetLevelData().GetCurrentRoom());
        
        RoomInfo ri = SystemData.Instance.GetLevelInfo().GetRoom(SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridX(), SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridY());
        ag.CreateFromAreaGenerator(ref ri);

        SystemData.Instance.GetLevelData().GetCurrentRoom().CreateFromRoomInfo(ri);

        /*
        GameObject to = SystemData.Instance.GetEditorData().objectCategories[1].objects[1];
        BaseGameTerrainObject bgto = to.GetComponent<BaseGameTerrainObject>();
        
        Grid.gridWidth = (int)SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomWidth();
        Grid.gridHeight = (int)SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomHeight();

        Terrain3DShapeGenerator.CreateTerrainFromAreaGenerator(ag, bgto, SystemData.Instance.GetLevelData().GetCurrentRoom(), 3);
        for(int i = 0; i < SystemData.NR_OF_LAYERS; i++)
        {
            Terrain3DShapeGenerator.CreateFromAreaGenerator(ag, SystemData.Instance.GetLevelData().GetCurrentRoom(), i);
        }
        */
    }

    public void OnAutoGenerateAll()
    {
        GameObject to = SystemData.Instance.GetEditorData().objectCategories[0].objects[1];
        BaseGameTerrainObject bgto = to.GetComponent<BaseGameTerrainObject>();

        float startT = Time.realtimeSinceStartup;

        for (int i = 0; i < SystemData.Instance.GetLevelData().CountRooms(); i++)
        {
            RoomData rd = SystemData.Instance.GetLevelData().GetRoom(i);

            for(int l = 1; l < SystemData.NR_OF_LAYERS; l++)
            {
                rd.GetLayerByIndex(l).ClearList();
            }

            AreaGenerator ag = new AreaGenerator();
            ag.Generate(SystemData.Instance.GetLevelData(), rd);

            RoomInfo ri = SystemData.Instance.GetLevelInfo().GetRoom(rd.GetRoomGridX(), rd.GetRoomGridY());
            ag.CreateFromAreaGenerator(ref ri);

            rd.CreateFromRoomInfo(ri);

            /*
            Grid.gridWidth = (int)rd.GetRoomWidth();
            Grid.gridHeight = (int)rd.GetRoomHeight();

            Terrain3DShapeGenerator.CreateTerrainFromAreaGenerator(ag, bgto, rd, 3);
            for (int l = 0; l < SystemData.NR_OF_LAYERS; l++)
            {
                Terrain3DShapeGenerator.CreateFromAreaGenerator(ag, rd, l);
            }
            */
        }

        startT = Time.realtimeSinceStartup - startT;
        Debug.Log("Level generation duration: " + startT);
    }

}


