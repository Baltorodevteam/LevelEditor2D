using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RoomData
{
    static public float defaultRoomWidth = 15.0f;
    static public float defaultRoomHeight = 9.0f;

    List<LayerData> layersList;

    List<EnemyData> enemiesList;

    int selectedLayerIndex = -1;

    // in the world grid...
    int roomX = -1;
    int roomY = -1;

    // in the room grid
    float roomWidth = defaultRoomWidth;
    float roomHeight = defaultRoomHeight;

    // where are the doors: 0 - there is no door, 1 - first from the left/top, 2 - second from the left/top, 4, 8 etc...
    public int leftEdgeDoor;
    public int topEdgeDoor;
    public int rightEdgeDoor;
    public int bottomEdgeDoor;

    public float difficulty;
    public bool isStartRoom;
    public bool isEndRoom;

    public void CreateFromRoomInfo(RoomInfo ri)
    {
        leftEdgeDoor = ri.leftEdgeDoor;
        rightEdgeDoor = ri.rightEdgeDoor;
        topEdgeDoor = ri.topEdgeDoor;
        bottomEdgeDoor = ri.bottomEdgeDoor;

        difficulty = ri.difficulty;

        for (int l = 1; l < SystemData.NR_OF_LAYERS; l++)
        {
            GetLayerByIndex(l).ClearList();
        }

        GameObject to = SystemData.Instance.GetEditorData().objectCategories[0].objects[1];
        BaseGameTerrainObject bgto = to.GetComponent<BaseGameTerrainObject>();

        Terrain3DShapeGenerator.CreateTerrainFromRoomInfo(ri, bgto, this, 3);
        for (int i = 0; i < SystemData.NR_OF_LAYERS; i++)
        {
            Terrain3DShapeGenerator.CreateFromRoomInfo(ri, this, i);
        }
    }


    public List<EnemyData> GetEnemiesData()
    {
        return enemiesList;
    }


    public float GetOffsetFor(int layerIndex, int gridX, int gridY)
    {
        if (layerIndex > 0)
        {
            BaseGameObject bgo = layersList[layerIndex - 1].CheckGrid(gridX, gridY);
            if (bgo != null && bgo.IsTerrain() > 0)
            {
                BaseGameTerrainObject bgto = bgo.GetComponent<BaseGameTerrainObject>();
                return bgto != null ? bgto.GetPerspectiveOffset() : 0f;
            }
        }
        return 0.0f;
    }

    public int GetRoomGridX()
    {
        return roomX;
    }

    public int GetRoomGridY()
    {
        return roomY;
    }

    public float GetRoomWidth()
    {
        return roomWidth;
    }

    public float GetRoomHeight()
    {
        return roomHeight;
    }

    BaseGameBlendingColorTerrainObject CreateUnderWaterObjectByTerrainObject(BaseGameTerrainObject srcObj)
    {
        BaseGameBlendingColorTerrainObject bgbco = GameObject.Instantiate(SystemData.Instance.GetEditorData().blendingColorUnderWaterTerrainObjectPrefab) as BaseGameBlendingColorTerrainObject;

        // umiejscawiamy obiekt jedna kratke nizej...
        bgbco.transform.position = new Vector3(srcObj.transform.position.x, /*srcObj.transform.position.y - 1.0f*/(int)srcObj.transform.position.y - 0.0f, srcObj.transform.position.z);

        bgbco.width = srcObj.width;
        bgbco.height = srcObj.height;
        //bgbco.SetTerrain(srcObj.IsTerrain());

        //if (srcObj.GetColliderOffsetY() != 0)
        if(srcObj.underWaterOffset != 0)//srcObj.GetOffsetPosY() != 0
        {
			float perspectiveOffset = 0.5f;
			bgbco.SetOffsetPosY(perspectiveOffset);
			bgbco.SetColliderOffsetY(-perspectiveOffset);
			bgbco.SetPivotOffset(-perspectiveOffset);
			bgbco.UpdatePositionY(true);
		}
		//bgbco.SetSpriteIndex(srcObj.GetSpriteIndex());
		bgbco.SetSpriteIndex(srcObj.underWaterIndex);
        bgbco.UpdateEnvironment(SystemData.Instance.GetEditorData());
        bgbco.SetBlendingValue(0, true);

        return bgbco;
    }

    public void ClearTerrain(LayerData layer)
    {
        for (int y = 0; y < (int)roomHeight; y++)
        {
            for (int x = 0; x < (int)roomWidth; x++)
            {
                BaseGameObject go = layer.CheckGrid(x, y);
                if (go != null && go.IsTerrain() > 0)
                {
                    bool bOK = layer.RemoveObjectPtr(go);
                    if(bOK)
                    {
                        GameObject.Destroy(go.gameObject);
                    }
                }
            }
        }
    }

    public void CreateUnderWaterTerrain(LayerData srcLayer, LayerData destLayer)
    {
        for (int y = 0; y < (int)roomHeight; y++)
        {
            for (int x = 0; x < (int)roomWidth; x++)
            {
                BaseGameObject go = srcLayer.CheckGrid(x, y);
                if (go != null && go.IsTerrain() > 0)
                {
                    BaseGameObject go2 = destLayer.CheckGrid(x, y);
                    if(go2 != null)
                    {
                        destLayer.RemoveObjectPtr(go2);
                        GameObject.Destroy(go2.gameObject);
                    }

                    BaseGameTerrainObject bgto = go.GetComponent<BaseGameTerrainObject>();
                    BaseGameBlendingColorTerrainObject bgbcto = CreateUnderWaterObjectByTerrainObject(bgto);
                    destLayer.AddObjectPtr(bgbcto, SystemData.Instance.GetEditorData().blendingColorUnderWaterTerrainObjectPrefab.name);
                }
            }
        }
    }
    public void UpdateUnderWaterTerrain(LayerData layer, float blendingValue)
    {
        for (int y = 0; y < (int)roomHeight; y++)
        {
            for (int x = 0; x < (int)roomWidth; x++)
            {
                BaseGameObject go = layer.CheckGrid(x, y);
                if (go != null && go.IsTerrain() > 0)
                {
                    BaseGameTerrainObject bgto = go.GetComponent<BaseGameTerrainObject>();
                    BaseGameBlendingColorTerrainObject bgbcto = go.GetComponent<BaseGameBlendingColorTerrainObject>();
                    if(bgbcto != null)
                    {
                        bgbcto.SetBlendingValue(blendingValue, true);
                    }
                }
            }
        }
    }

    public IEnumerator WaitAndUpdateUnderWaterTerrain(LayerData layer, float blendingValue)
    {
        yield return new WaitForSeconds(0.1f);
        UpdateUnderWaterTerrain(layer, blendingValue);
    }

    public int FindInLayers(BaseGameObject o)
    {
        for(int i = 0; i < layersList.Count; i++)
        {
            if(layersList[i].FindObjectPtr(o) >= 0)
            {
                return i;
            }
        }
        return -1;
    }

    public void UpdateEnvironment(EditorData ed)
    {
        for (int i = 0; i < layersList.Count; i++)
        {
            layersList[i].UpdateEnvironment(ed);
        }
    }

    public RoomData(int x, int y)
    {
        roomX = x;
        roomY = y;
        ClearLayers();
        enemiesList = new List<EnemyData>();
    }

    public RoomData(int x, int y, float w, float h)
    {
        roomX = x;
        roomY = y;
        roomWidth = w;
        roomHeight = h;
        ClearLayers();
        enemiesList = new List<EnemyData>();
    }

    public void InitDataLayers(int nLayersNr)
    {
        layersList = new List<LayerData>();

        for (int i = 0; i < nLayersNr; i++)
        {
            AddLayer();
        }

        selectedLayerIndex = 0;
    }

    public void ClearLayers()
    {
        if(layersList != null)
        {
            for (int i = 0; i < layersList.Count; i++)
            {
                layersList[i].ClearMe();
            }
        }

        layersList = new List<LayerData>();
    }

    public void ClearLayersList()
    {
        if (layersList != null)
        {
            for (int i = 0; i < layersList.Count; i++)
            {
                layersList[i].ClearList();
            }
        }
    }

    public void SwapLayers(int layer1, int layer2)
    {
        LayerData ld1 = layersList[layer1];
        LayerData ld2 = layersList[layer2];

        ld1.SetLayerNr(layer2);
        ld2.SetLayerNr(layer1);

        layersList[layer1] = ld2;
        layersList[layer2] = ld1;

        ld1.UpdateLayerAltitude();
        ld2.UpdateLayerAltitude();
    }

    public void UpdateLayersAltitude()
    {
        for (int i = 0; i < layersList.Count; i++)
        {
            layersList[i].UpdateLayerAltitude();
        }
    }

    public int GetLayersCount()
    {
        return layersList.Count;
    }

    public LayerData GetLayerByIndex(int nIndex)
    {
        return layersList[nIndex];
    }

    public int FindLayerIndexByNr(int nLayerNr)
    {
        for(int i = 0; i < layersList.Count; i++)
        {
            if(layersList[i].GetLayerNr() == nLayerNr)
            {
                return i;
            }
        }
        return -1;
    }

    public int GetFirstFreeNrForLayer()
    {
        for (int i = 0; i < layersList.Count; i++)
        {
            if(FindLayerIndexByNr(i) < 0)
            {
                return i;
            }
        }

        return layersList.Count;
    }

    public int AddLayer()
    {
        int nLayerNr = GetFirstFreeNrForLayer();
        layersList.Add(new LayerData(roomX, roomY, nLayerNr));
        return nLayerNr;
    }

    public void RemoveLayerByIndex(int nListIndex)
    {
        if(nListIndex >= 0 && nListIndex < layersList.Count)
        {
            layersList[nListIndex].ClearMe();
            layersList.RemoveAt(nListIndex);
        }

        if (layersList.Count > 0)
        {
            SelectLayerByIndex(0);
        }
        else
        {
            SelectLayerByIndex(-1);
        }
    }

    public void RemoveLayeByNr(int nLayerNr)
    {
        int nIndex = FindLayerIndexByNr(nLayerNr);
        if(nIndex >= 0)
        {
            RemoveLayerByIndex(nIndex);
        }

        if(layersList.Count > 0)
        {
            SelectLayerByIndex(0);
        }
        else
        {
            SelectLayerByIndex(-1);
        }
    }

    public void SelectLayerByIndex(int nIndex)
    {
        if(nIndex >= 0 && nIndex < layersList.Count)
        {
            selectedLayerIndex = nIndex;
        }
    }

    public int GetSelectedLayerIndex()
    {
        return selectedLayerIndex;
    }

    public LayerData GetSelectedLayer()
    {
        if(selectedLayerIndex >= 0 && selectedLayerIndex < layersList.Count)
        {
            return layersList[selectedLayerIndex];
        }
        return null;
    }

    public void EnableColliders(bool v)
    {
        for(int i = 0; i < layersList.Count; i++)
        {
            layersList[i].EnableColliders(v);
        }
    }

    public void CreateBackGroundObjectAtCenterOfLayer(GameObject prefabObject)
    {
        GameObject go = GameObject.Instantiate(prefabObject) as GameObject;
        BaseGameBackGroundObject bgo = go.GetComponent<BaseGameBackGroundObject>();
        if (bgo != null)
        {
            if (bgo.GetDefaultLayer() >= 0 && bgo.GetDefaultLayer() < GetLayersCount())
            {
                int layerID = bgo.GetDefaultLayer();
                bgo.name = prefabObject.name;
                SetBackGroundObjectAtCenterOfLayer(bgo, GetLayerByIndex(layerID));
                bgo.SetSortingLayerID(layerID);
                bgo.UpdateEnvironment(SystemData.Instance.GetEditorData());
            }
        }
    }

    void SetBackGroundObjectAtCenterOfLayer(BaseGameBackGroundObject o, LayerData ld)
    {
        ld.ClearList();
        ld.AddObjectPtr(o, o.name);
        o.SetSize(roomWidth, roomHeight);
        o.transform.localPosition = new Vector3(roomWidth / 2 - 0.5f, roomHeight / 2 - 0.5f, 0);
    }


    void SetBackgroundColorChange(int nMode)
    {
        // kolor dna...
        if (layersList[0].GetObjectsCount() > 0)
        {
            BaseGameBackGroundObject bgbo = layersList[0].GetObjectPtr(0).GetComponent<BaseGameBackGroundObject>();
            if (bgbo != null)
            {
                if(nMode == 0)
                {
                    bgbo.SetBlendValue(0.0f);
                }
                else
                {
                    bgbo.SetBlendValue(1.0f);
                }
            }
        }
    }

    public void EnableAltMode(int nMode) // 0 - under water, 1 - on the water
    {
        if(nMode == 0)
        {
            layersList[0].SetVisibleFlag(true);
            //SetBackgroundColorChange(nMode);
            SetBackgroundColorChange(1);

            layersList[1].SetVisibleFlag(true);
            layersList[2].SetVisibleFlag(true);

            //ClearTerrain(layersList[2]);
            //CreateUnderWaterTerrain(layersList[3], layersList[2]);

            //layersList[3].SetVisibleFlag(false);
            //layersList[4].SetVisibleFlag(false);
            //layersList[5].SetVisibleFlag(false);
            layersList[3].SetVisibleFlag(true);
            layersList[4].SetVisibleFlag(true);
            layersList[5].SetVisibleFlag(true);
        }
        else
        {
            layersList[0].SetVisibleFlag(true);
            SetBackgroundColorChange(nMode);

            //layersList[1].SetVisibleFlag(false);
            //layersList[2].SetVisibleFlag(false);
            layersList[1].SetVisibleFlag(true);
            layersList[2].SetVisibleFlag(true);

            layersList[3].SetVisibleFlag(true);
            layersList[4].SetVisibleFlag(true);
            layersList[5].SetVisibleFlag(true);
        }

        SystemData.Instance.GetLevelData().GetSelectionList().RemoveAll();
    }

    public void RemoveObjectPtr(BaseGameObject g)
    {
        for (int i = 1; i < GetLayersCount(); i++) // 0 - background level
        {
            GetLayerByIndex(i).RemoveObjectPtr(g);
        }
    }

    ////////////////////////////////////////////////////////////////////

    public EnemyData FindEnemyDataByID(int id)
    {
        for (int i = enemiesList.Count - 1; i >= 0; i--)
        {
            if (enemiesList[i].enemyID == id)
            {
                return enemiesList[i];
            }
        }
        return null;
    }

    public int FindEnemyDataIndexByID(int id)
    {
        for (int i = enemiesList.Count - 1; i >= 0; i--)
        {
            if (enemiesList[i].enemyID == id)
            {
                return i;
            }
        }
        return -1;
    }


    ////////////////////////////////////////////////////////////////////

    void LoadEnemyFromJsonSerializer(JsonRoomSerializer rs, int nIndex)
    {
        EnemyData ed = new EnemyData();
        ed.enemyID = rs.enemyList[nIndex].enemyID;
        ed.timeout = rs.enemyList[nIndex].timeout;
        ed.waveID = rs.enemyList[nIndex].waveID;

        enemiesList.Add(ed);
    }

    public void LoadFromJsonRoomSerializer(JsonRoomSerializer uld)
    {
        ClearLayers();

        roomWidth = uld.roomWidth;
        roomHeight = uld.roomHeight;

        leftEdgeDoor = uld.leftEdgeDoor;
        topEdgeDoor = uld.topEdgeDoor;
        rightEdgeDoor = uld.rightEdgeDoor;
        bottomEdgeDoor = uld.bottomEdgeDoor;

        // layers...
        int nLayersNr = uld.layersList.Count;
        InitDataLayers(nLayersNr);

        for (int i = 0; i < nLayersNr; i++)
        {
            layersList[i].LoadFromJsonLayerSerializer(uld.layersList[i]);
        }

        // enemies...
        enemiesList = new List<EnemyData>();
        for (int i = 0; i < uld.enemyList.Count; i++)
        {
            LoadEnemyFromJsonSerializer(uld, i);
        }
    }

}