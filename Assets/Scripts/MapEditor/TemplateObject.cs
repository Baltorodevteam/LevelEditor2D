using System.Collections.Generic;
using UnityEngine;
 
public class TemplateObject : MonoBehaviour
{
    static public TemplateObject Instance;

    string prefabName;
    int prefabIndex;
    GameObject prefabObject;

    List<BaseGameObject> childList = new List<BaseGameObject>();

    public int GetChildCount()
    {
        return transform.childCount;
    }

    public BaseGameObject GetChildAt(int i)
    {
        return transform.GetChild(i).GetComponent<BaseGameObject>();
    }

    public string GetPrefabName()
    {
        return prefabName;
    }

    public int GetPrefabIndex()
    {
        return prefabIndex;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(transform.childCount > 0)
        {
            UpdateSortingLayer();

            if (!Map.Instance.IsPointerOverUIObject(Input.mousePosition))
            {
                UpdatePosition();
                CheckGoodPosition();
            }
            else
            {
                HidePosition();
            }
        }
    }

    public void Clear()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        childList.Clear();

        this.prefabName = "";
        prefabIndex = -1;
    }

    public void CreateObject(string prefabName)
    {
        Clear();
        GameObject go = Instantiate(Resources.Load(prefabName, typeof(GameObject))) as GameObject;

        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;

        BaseGameObject bgo = go.GetComponent<BaseGameObject>();
        if(bgo != null)
        {
            int layerID = SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayerIndex();
            if(bgo.GetDefaultLayer() >= 0 && bgo.GetDefaultLayer() < SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayersCount())
            {
                layerID = bgo.GetDefaultLayer();
            }
            bgo.SetSortingLayerID(layerID);

            childList.Add(bgo);
        }

        this.prefabName = prefabName;
        prefabIndex = -1;

        UpdatePosition();
    }

    public void CreateObject(GameObject prefabObject)
    {
        Clear();
        GameObject go = Instantiate(prefabObject) as GameObject;

        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;

        BaseGameObject bgo = go.GetComponent<BaseGameObject>();
        if (bgo != null)
        {
            int layerID = SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayerIndex();
            if (bgo.GetDefaultLayer() >= 0 && bgo.GetDefaultLayer() < SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayersCount())
            {
                layerID = bgo.GetDefaultLayer();
                SystemData.Instance.GetLevelData().GetCurrentRoom().SelectLayerByIndex(layerID);
            }
            bgo.SetSortingLayerID(layerID);
            bgo.UpdateSortingOrder();

            childList.Add(bgo);
        }

        this.prefabObject = prefabObject;
        this.prefabName = prefabObject.name;
        this.prefabIndex = -1;

        UpdatePosition();
    }

    Vector3 pos = new Vector3();
    public void UpdatePosition()
    {
        Vector3 position = Map.Instance.MouseOnPlane();
        pos.x = (int)(position.x + 0.5f);
        pos.y = (int)(position.y + 0.5f);

        //pos.x = SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridX() * (int)RoomData.defaultRoomWidth + Map.Instance.GetCursorX();
        //pos.y = SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridY() * (int)RoomData.defaultRoomHeight + Map.Instance.GetCursorY();

        pos.z = -1;

        transform.position = pos;

        int nGridX = (int)(transform.position.x + 0.1f) - SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridX() * (int)RoomData.defaultRoomWidth;
        int nGridY = (int)(transform.position.y + 0.1f) - SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridY() * (int)RoomData.defaultRoomHeight;

        int layerDataIndex = SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayerIndex();
        float addOffsetY = SystemData.Instance.GetLevelData().GetCurrentRoom().GetOffsetFor(layerDataIndex, nGridX, nGridY);

        pos.y += addOffsetY;

        transform.position = pos;
    }

    public void HidePosition()
    {
        pos.x = -1000;
        pos.y = -1000;

        pos.z = -1;

        transform.position = pos;
    }

    public void UpdateSortingLayer()
    {
        //int layerDataIndex = SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayerIndex();

        foreach (BaseGameObject child in childList)
        {
            child.SetSortingLayerID(15);
        }
    }

    public bool IsGoodPlaceForObject(BaseGameObject bgo)
    {
        if (SystemData.Instance.GetLevelData() == null || SystemData.Instance.GetLevelData().GetCurrentRoom() == null)
        {
            return false;
        }

        LayerData layerData = SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayer();
        int layerDataIndex = SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayerIndex();

        if (bgo.GetDefaultLayer() >= 0 && bgo.GetDefaultLayer() < SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayersCount())
        {
            layerData = SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(bgo.GetDefaultLayer());
            layerDataIndex = bgo.GetDefaultLayer();
        }


        int nGridX = (int)(bgo.transform.position.x + 0.1f) - SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridX() * (int)RoomData.defaultRoomWidth;
        int nGridY = (int)(bgo.transform.position.y + 0.1f) - SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridY() * (int)RoomData.defaultRoomHeight;

        for (int x = 0; x < bgo.width; x++)
        {
            for (int y = 0; y < bgo.height; y++)
            {
                if (Map.Instance.IsPositionOnGrid(nGridX + x, nGridY + y) == false)
                {
                    return false;
                }
            }
        }

        for (int x = 0; x < bgo.width; x++)
        {
            for (int y = 0; y < bgo.height; y++)
            {
                BaseGameObject g = layerData.CheckGrid(bgo, nGridX + x, nGridY + y);
                if (g != null && bgo != g)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool CheckGoodPosition()
    {
        bool bOK = true;
        foreach (BaseGameObject child in childList)
        {
            if (!IsGoodPlaceForObject(child))
            {
                bOK = false;
                break;
            }
        }

        foreach (BaseGameObject child in childList)
        {
            child.SelectBlockedMe(!bOK);
        }

        return bOK;
    }

    public bool CreateOnLayer()
    {
        if(GetChildCount() == 0)
        {
            return false;
        }

        UpdatePosition();

        if (!CheckGoodPosition())
        {
            return false;
        }

        if(SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayer().GetObjectsCount() > 1)
        {
            if (!CheckGoodPosition())
            {
                return false;
            }
        }

        if (transform.childCount == 1)
        {
            CreateObjectFromTemplate();
        }

        return true;
    }
    
    public void CreateObjectFromTemplate()
    {
        int nGridX = (int)(childList[0].transform.position.x + 0.1f) - SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridX() * (int)RoomData.defaultRoomWidth;
        int nGridY = (int)(childList[0].transform.position.y + 0.1f) - SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridY() * (int)RoomData.defaultRoomHeight;

        LayerData layerData = SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayer();
        int layerDataIndex = SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayerIndex();

        if (childList[0].GetDefaultLayer() >= 0 && childList[0].GetDefaultLayer() < SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayersCount())
        {
            layerData = SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(childList[0].GetDefaultLayer());
            layerDataIndex = childList[0].GetDefaultLayer();
        }

        GameObject obj = Instantiate(this.prefabObject) as GameObject;
        BaseGameObject createdObject = obj.GetComponent<BaseGameObject>();

        createdObject.name = this.prefabObject.name;

        float addOffsetY = SystemData.Instance.GetLevelData().GetCurrentRoom().GetOffsetFor(layerDataIndex, nGridX, nGridY);

        layerData.AddObjectPtr(createdObject, prefabName);

        Vector3 pos = createdObject.transform.localPosition;
        pos.x = nGridX;
        pos.y = nGridY + addOffsetY;
        pos.z = -layerDataIndex * LayerData.layerAlt;

        createdObject.transform.localPosition = pos;
        createdObject.UpdateSortingOrder();

        // adding to levelInfo/roomInfo/layerInfo
        int rx = SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridX();
        int ry = SystemData.Instance.GetLevelData().GetCurrentRoom().GetRoomGridY();
        RoomInfo ri = SystemData.Instance.GetLevelInfo().GetRoom(rx, ry);
        if (ri != null)
        {
            int l = createdObject.GetDefaultLayer();
            int id = createdObject.objectID;
            ri.layers[l].layerGrid[nGridX, nGridY] = id;
        }
        //////////////////////////

    }

}