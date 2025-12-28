using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    public static Map Instance = null;

    [SerializeField]
    Camera currentCamera;

    [SerializeField]
    GameObject selectedPivot;

    [SerializeField]
    CursorObject cursorObject;

    int cursorX = 0;
    int cursorY = 0;

    Vector2 selectedPivotStartPosition = new Vector2();

    bool updateTerrain3DShape = false;
    float updateTerrain3DShapeTimer = 0;


    public CursorObject GetCursor()
    {
        return cursorObject;
    }

    public void SetCursorAtPosition(RoomData rd, int x, int y)
    {
        cursorX = x;
        cursorY = y;

        float xx = rd.GetRoomGridX() * RoomData.defaultRoomWidth + (float)x;
        float yy = rd.GetRoomGridY() * RoomData.defaultRoomHeight + (float)y;

        cursorObject.transform.position = new Vector3(xx, yy, 0);
    }

    public int GetCursorX()
    {
        return cursorX;
    }
    public int GetCursorY()
    {
        return cursorY;
    }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HideSelectedPivot();
    }

    Vector3 cameraPos = new Vector3();

    public void CameraPositionToCenterOfRoom(RoomData rd)
    {
        SetCursorAtPosition(rd, ((int)rd.GetRoomWidth()-1)/2, ((int)rd.GetRoomHeight() - 1) / 2);
        SetCameraToCursorPos();
    }

    public void SetCameraToCursorPos()
    {
        cameraPos = cursorObject.transform.position;
        cameraPos.z = -10;
        currentCamera.transform.position = cameraPos;
    }

    public int CursorLeft(RoomData rd)
    {
        if(cursorX > 0)
        {
            SetCursorAtPosition(rd, cursorX - 1, cursorY);
            SetCameraToCursorPos();
        }
        else // trying change room...
        {
            int gridX = rd.GetRoomGridX() * (int)RoomData.defaultRoomWidth - 1;
            int gridY = rd.GetRoomGridY() * (int)RoomData.defaultRoomHeight + cursorY;

            int levelGridX = gridX / (int)RoomData.defaultRoomWidth;
            int levelGridY = gridY / (int)RoomData.defaultRoomHeight;

            RoomData rdNew = SystemData.Instance.GetLevelData().GetRoom(levelGridX, levelGridY);
            if(rdNew != null)
            {
                SystemData.Instance.GetLevelData().SetCurrentRoom(rdNew);

                cursorX = (int)rdNew.GetRoomWidth() - 1;
                cursorY = gridY - (rdNew.GetRoomGridY() * (int)RoomData.defaultRoomHeight);

                SetCursorAtPosition(rdNew, cursorX, cursorY);
                SetCameraToCursorPos();
                return 1;
            }
        }
        return 0;
    }

    public int CursorRight(RoomData rd)
    {
        if (cursorX < (int)rd.GetRoomWidth() - 1)
        {
            SetCursorAtPosition(rd, cursorX + 1, cursorY);
            SetCameraToCursorPos();
        }
        else // trying change room...
        {
            int gridX = rd.GetRoomGridX() * (int)RoomData.defaultRoomWidth + (int)rd.GetRoomWidth();
            int gridY = rd.GetRoomGridY() * (int)RoomData.defaultRoomHeight + cursorY;

            int levelGridX = gridX / (int)RoomData.defaultRoomWidth;
            int levelGridY = gridY / (int)RoomData.defaultRoomHeight;

            RoomData rdNew = SystemData.Instance.GetLevelData().GetRoom(levelGridX, levelGridY);
            if (rdNew != null)
            {
                SystemData.Instance.GetLevelData().SetCurrentRoom(rdNew);

                cursorX = 0;
                cursorY = gridY - (rdNew.GetRoomGridY() * (int)RoomData.defaultRoomHeight);

                SetCursorAtPosition(rdNew, cursorX, cursorY);
                SetCameraToCursorPos();
                return 1;
            }
        }
        return 0;
    }

    public int CursorUp(RoomData rd)
    {
        if (cursorY < (int)rd.GetRoomHeight() - 1)
        {
            SetCursorAtPosition(rd, cursorX, cursorY + 1);
            SetCameraToCursorPos();
        }
        else // trying change room...
        {
            int gridX = rd.GetRoomGridX() * (int)RoomData.defaultRoomWidth + cursorX;
            int gridY = rd.GetRoomGridY() * (int)RoomData.defaultRoomHeight + (int)rd.GetRoomHeight();

            int levelGridX = gridX / (int)RoomData.defaultRoomWidth;
            int levelGridY = gridY / (int)RoomData.defaultRoomHeight;

            RoomData rdNew = SystemData.Instance.GetLevelData().GetRoom(levelGridX, levelGridY);
            if (rdNew != null)
            {
                SystemData.Instance.GetLevelData().SetCurrentRoom(rdNew);

                cursorX = gridX - (rdNew.GetRoomGridX() * (int)RoomData.defaultRoomWidth);
                cursorY = 0;

                SetCursorAtPosition(rdNew, cursorX, cursorY);
                SetCameraToCursorPos();
                return 1;
            }
        }
        return 0;
    }

    public int CursorDown(RoomData rd)
    {
        if (cursorY > 0)
        {
            SetCursorAtPosition(rd, cursorX, cursorY - 1);
            SetCameraToCursorPos();
        }
        else // trying change room...
        {
            int gridX = rd.GetRoomGridX() * (int)RoomData.defaultRoomWidth + cursorX;
            int gridY = rd.GetRoomGridY() * (int)RoomData.defaultRoomHeight - 1;

            int levelGridX = gridX / (int)RoomData.defaultRoomWidth;
            int levelGridY = gridY / (int)RoomData.defaultRoomHeight;

            RoomData rdNew = SystemData.Instance.GetLevelData().GetRoom(levelGridX, levelGridY);
            if (rdNew != null)
            {
                SystemData.Instance.GetLevelData().SetCurrentRoom(rdNew);

                cursorX = gridX - (rdNew.GetRoomGridX() * (int)RoomData.defaultRoomWidth);
                cursorY = (int)rdNew.GetRoomHeight() - 1;

                SetCursorAtPosition(rdNew, cursorX, cursorY);
                SetCameraToCursorPos();
                return 1;
            }
        }
        return 0;
    }

    public void CameraPositionHorizontal(float fDir)
    {
        cameraPos.x = currentCamera.transform.position.x + fDir;
        cameraPos.y = currentCamera.transform.position.y;
        cameraPos.z = currentCamera.transform.position.z;

        currentCamera.transform.position = cameraPos;
    }

    public void CameraPositionVertical(float fDir)
    {
        cameraPos.x = currentCamera.transform.position.x;
        cameraPos.y = currentCamera.transform.position.y + fDir;
        cameraPos.z = currentCamera.transform.position.z;

        currentCamera.transform.position = cameraPos;
    }


    private void Update()
    {
        if(SystemData.Instance.GetLevelData() == null || SystemData.Instance.GetLevelData().GetCurrentRoom() == null)
        {
            return;
        }

        updateMouseControl();

        if(!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            if(Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
            {
                UndoManager.Undo();
            }
        }

        updateTerrain3DShapeTimer -= Time.deltaTime;
        if (updateTerrain3DShape == true && updateTerrain3DShapeTimer <= 0)
        {
            updateTerrain3DShape = false;
            updateTerrain3DShapeTimer = 0.01f;

            Terrain3DShapeGenerator.CheckLayerData(SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(3));
            CreateUnderWaterTerrain();
        }
    }

    public void CreateUnderWaterTerrain()
    {
        SystemData.Instance.GetLevelData().GetCurrentRoom().ClearTerrain(SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(2));
        SystemData.Instance.GetLevelData().GetCurrentRoom().CreateUnderWaterTerrain(SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(3), SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(2));
    }

    public GameObject GetSelectedPivotPtr()
    {
        return selectedPivot;
    }

    public void HideSelectedPivot()
    {
        selectedPivot.transform.position = new Vector3(-1000, -1000, -1);
    }

    public void ActualizeSelectedPivot()
    {
        selectedPivot.transform.position = SystemData.Instance.GetLevelData().GetSelectionList().GetPivotPosition();
    }

    public bool IsPositionOnGrid(float x, float y)
    {
        /*
        if (IsPointerOverUIObject(Input.mousePosition))
        {
            return false;
        }
        */

        return (x >= 0 && x < Grid.gridWidth && y >= 0 && y < Grid.gridHeight);
    }

    Plane xzPlane = new Plane(new Vector3(0, 0, -1), new Vector3(0, 0, Grid.gridDepth));

    public Vector3 MouseOnPlane()
    {
        Ray mouseray = currentCamera.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;
        if (xzPlane.Raycast(mouseray, out hitdist))
        {
            return mouseray.GetPoint(hitdist);
        }
        if (hitdist < -1.0f)
        {
            return mouseray.GetPoint(-hitdist);
        }
        return Vector3.zero;
    }

    public Vector3 ObjectPosition2Screen(GameObject o)
    {
        return currentCamera.WorldToScreenPoint(o.transform.position);
    }

    public bool RemoveObject()
    {
        if (SystemData.Instance.GetLevelData() == null || SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayerIndex() < 0)
        {
            return false;
        }

        Vector3 position = MouseOnPlane();

        float xx = (position.x + 0.5f - (float)SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridX() * RoomData.defaultRoomWidth);
        float yy = (position.y + 0.5f - (float)SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridY() * RoomData.defaultRoomHeight);

        BaseGameObject go = null;
        
        for(int layer = SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayersCount() - 1; layer >= 1; layer--) // 0 - background
        {
            if(SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(layer).isVisible)
            {
                go = SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(layer).CheckGrid((int)xx, (int)yy);
                if (go != null)
                {
                    break;
                }
            }
        }

        if(go == null)
        {
            return false;
        }

        // removing from levelInfo/roomInfo/layerInfo
        int rx = SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridX();
        int ry = SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridY();
        RoomInfo ri = SystemData.Instance.GetLevelInfo().GetRoom(rx, ry);
        if(ri != null)
        {
            int l = go.GetDefaultLayer();
            int id = go.objectID;
            int x = (int)xx;
            int y = (int)yy;
            if(ri.layers[l].layerGrid[x,y] == id)
            {
                ri.layers[l].layerGrid[x, y] = 0;
            }
        }
        //////////////////////////

        SystemData.Instance.GetLevelData().GetCurrentRoom().RemoveObjectPtr(go);
        GameObject.Destroy(go.gameObject);

        isDragging = true;

        return true;
    }

    public bool RemoveObjectAtCursor()
    {
        if (SystemData.Instance.GetLevelData() == null || SystemData.Instance.GetLevelData().GetCurrentRoom() == null)
        {
            return false;
        }

        Vector3 position = MouseOnPlane();

        float xx = (position.x + 0.5f - (float)SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridX() * RoomData.defaultRoomWidth);
        float yy = (position.y + 0.5f - (float)SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridY() * RoomData.defaultRoomHeight);

        xx = Map.Instance.GetCursorX();
        yy = Map.Instance.GetCursorY();

        BaseGameObject go = null;

        for (int layer = SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayersCount() - 1; layer >= 1;  layer--) // 0 - background
        {
            if (SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(layer).isVisible)
            {
                go = SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(layer).CheckGrid((int)xx, (int)yy);
                if (go != null)
                {
                    SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(layer).RemoveObjectPtr(go);
                    GameObject.Destroy(go.gameObject);

                    return true;
                }
            }
        }

        return false;
    }

    public BaseGameObject GetObjectAtCursor()
    {
        if (SystemData.Instance.GetLevelData() == null || SystemData.Instance.GetLevelData().GetCurrentRoom() == null)
        {
            return null;
        }

        Vector3 position = MouseOnPlane();

        float xx = (position.x + 0.5f - (float)SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridX() * RoomData.defaultRoomWidth);
        float yy = (position.y + 0.5f - (float)SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridY() * RoomData.defaultRoomHeight);

        xx = Map.Instance.GetCursorX();
        yy = Map.Instance.GetCursorY();

        BaseGameObject go = null;

        for (int layer = SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayersCount() - 1; layer >= 1; layer--) // 0 - background
        {
            if (SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(layer).isVisible)
            {
                go = SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(layer).CheckGrid((int)xx, (int)yy);
                if (go != null)
                {
                    return go;
                }
            }
        }

        return null;
    }

    public bool IsPointerOverUIObject(Vector2 touchPosition)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touchPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return true;
            }
        }
        return false;
    }

    bool isDragging = false;
    void updateMouseControl()
    {
        if (isDragging)
        {
            if (!IsPointerOverUIObject(Input.mousePosition))
            {
                if (Input.GetMouseButton(0))
                {
                    OnMouseDrag();
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIObject(Input.mousePosition))
            {
                OnMouseDown(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!IsPointerOverUIObject(Input.mousePosition))
            {
                OnMouseOver();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!IsPointerOverUIObject(Input.mousePosition))
            {
                OnMouseCancel();
            }
        }
    }

    public void OnMouseDown(Vector2 touchScreenPosition)
    {
        if (!Input.GetKey(KeyCode.C) && TemplateObject.Instance.GetChildCount() == 1)
        {
            UndoManager.AddToUndo();
            TemplateObject.Instance.CreateOnLayer();

            isDragging = true;
        }
        else if (!Input.GetKey(KeyCode.C) && TemplateObject.Instance.GetChildCount() > 1)
        {
            UndoManager.AddToUndo();
            //PrefabsManager.CreateByIndex(TemplateObject.Instance.GetPrefabIndex());
        }
        else
        {
            if(Input.GetKey(KeyCode.C))
            {
                UndoManager.AddToUndo();

                RemoveObject();
                isDragging = true;
            }
            /*
            else
            {
                RaycastHit2D hit;
                Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

                bool bSelect = false;

                hit = Physics2D.Raycast(ray.origin, ray.direction);
                
                //if (Physics.Raycast(ray, out hit))
                if (hit)
                {
                    Transform objectHit = hit.transform;

                    if(objectHit.gameObject == selectedPivot)
                    {
                        isDragging = true;

                        UndoManager.AddToUndo();
                        SystemData.Instance.GetLevelData().GetSelectionList().JoinObjectsPtrToPivot();

                        // zapamiêtujemy pozycjê pivotu
                        // !!!
                        selectedPivotStartPosition.x = selectedPivot.transform.position.x;
                        selectedPivotStartPosition.y = selectedPivot.transform.position.y;

                        return;
                    }
                    else
                    {
                        BaseGameObject bgo = objectHit.gameObject.GetComponent<BaseGameObject>();
                        if (bgo)
                        {
                            if (!Input.GetKey(KeyCode.LeftShift))
                            {
                                SystemData.Instance.GetLevelData().GetSelectionList().RemoveAll();
                            }
                            SystemData.Instance.GetLevelData().GetSelectionList().Add(bgo);
                            ActualizeSelectedPivot();
                            bSelect = true;
                        }
                    }
                }

                if(!bSelect)
                {
                    ///LayersPanel.Instance.GetRoomData().GetSelectedLayer().UnselectAllObjects();
                    SystemData.Instance.GetLevelData().GetSelectionList().RemoveAll();
                    HideSelectedPivot();
                }
            }
            */
        }

        updateTerrain3DShape = true;
    }

    void OnMouseDrag()
    {
        if(SystemData.Instance.GetLevelData().GetSelectionList().GetSize() > 0)
        {
            GameObject go = selectedPivot;// LayersPanel.Instance.GetRoomData().GetSelectedLayer().GetSelectedObjectByIndex(0);
            Vector3 position = MouseOnPlane();
            Vector3 pos = go.transform.position;
            pos.x = (int)position.x;
            pos.y = (int)position.y;

            pos.x = (int)(position.x + 0.5f);
            pos.y = (int)(position.y + 0.5f);

            go.transform.position = pos;

            SystemData.Instance.GetLevelData().GetSelectionList().CheckPosition();
        }
        else
        {
            if(Input.GetKey(KeyCode.C))
            {
                //RemoveObject();
            }
            else
            {
                if (!Input.GetKey(KeyCode.C) && TemplateObject.Instance.GetChildCount() == 1)
                {
                    TemplateObject.Instance.CreateOnLayer();
                }
            }
        }

        updateTerrain3DShape = true;
    }

    public void OnMouseOver()
    {
        // if any selected objects were moved, attach them to the layer again
        if (SystemData.Instance.GetLevelData().GetSelectionList().GetSize() > 0)
        {
            if(isDragging)
            {
                if (!SystemData.Instance.GetLevelData().GetSelectionList().CheckPosition())
                {
                    // can't place any objects here - pivot back to the starting point
                    selectedPivot.transform.position = new Vector3(selectedPivotStartPosition.x, selectedPivotStartPosition.y, selectedPivot.transform.position.z);
                }

                SystemData.Instance.GetLevelData().GetSelectionList().JoinObjectsPtrToLayers();

                HideSelectedPivot();
            }
        }
        else
        {
            if(TemplateObject.Instance.GetChildCount() == 1)
            {
                BaseGameObject bgo = TemplateObject.Instance.GetChildAt(0);
                int x = (int)bgo.gameObject.transform.position.x;
                int y = (int)bgo.gameObject.transform.position.y;
            }
        }

        updateTerrain3DShape = true;

        isDragging = false;
    }

    public void OnMouseCancel()
    {
        if (isDragging)
        {
            isDragging = false;

            // if any selected objects were moved, attach them to the layer again
            if (SystemData.Instance.GetLevelData().GetSelectionList().GetSize() > 0)
            {
                SystemData.Instance.GetLevelData().GetSelectionList().JoinObjectsPtrToLayers();

                HideSelectedPivot();
            }

            Terrain3DShapeGenerator.CheckLayerData(SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(3));
            CreateUnderWaterTerrain();
        }

        TemplateObject.Instance.Clear();
    }

    public void OnCancel()
    {
        updateTerrain3DShape = true;
        TemplateObject.Instance.Clear();
    }

    public void UpdateTerrain3DShape()
    {
        updateTerrain3DShape = true;
        updateTerrain3DShapeTimer = 1f;
    }

    public void ClearAll()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public GameObject AddRoom(int x, int y)
    {
        GameObject go = new GameObject("Room_" + (x + 1) + "x" + (y + 1));
        go.transform.parent = this.transform;
        go.transform.localPosition = new Vector3((float)x * RoomData.defaultRoomWidth, (float)y * RoomData.defaultRoomHeight, 0.0f);
        return go;
    }

    public bool DeleteRoom(int x, int y)
    {
        GameObject go = FindRoomPtr(x, y);
        if(go != null)
        {
            GameObject.Destroy(go);
            return true;
        }
        return false;
    }


    GameObject FindRoomPtr(int x, int y)
    {
        return GameObject.Find("Room_" + (x + 1) + "x" + (y + 1));
    }

    public GameObject AddLayer(int x, int y, int nIndex)
    {
        GameObject room = FindRoomPtr(x, y);
        if(room != null)
        {
            GameObject go = new GameObject("Layer" + (nIndex + 1));
            go.transform.parent = room.transform;
            go.transform.localPosition = Vector3.zero;
            return go;
        }
        return null;
    }
}
