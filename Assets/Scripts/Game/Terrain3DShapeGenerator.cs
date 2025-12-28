using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// How does it work...?
/// If a given grid is adjacent to something from below, we take a set of textures with indices 0-7
/// If this is the lowest part of the terrain (the given grid is not adjacent to anything at the bottom), we take a texture set of 8-15
/// Depending on how the left-top-right neighbors are distributed, we can choose from 8 sprite variants.
/// 0 - (nobody is adjacent to the given block, neither to the left, nor to the right, nor to the top) index 1
/// 1 - (neighboring only on the left) index 2
/// 2 - (adjacent only from above) index 3
/// 3 - (neighboring from the left and top) index 4
/// 4 - (adjacent only on the right) index 5
/// 5 - (neighboring on the left and right) index 6
/// 6 - (adjacent from above and on the right) index 7
/// 7 - (neighboring on the left, right and top) index 8
/// 
///        2
///        
///     1  X  4
/// 
///        8
///        
/// </summary>




public class Terrain3DShapeGenerator
{
    static int[,] gridTab = null;
    static int[,] mainGridTab = null;
    static int[,] underWaterGridTab = null;

    public static void Init()
    {
    }

    static public void CheckLayerData(LayerData ld)
    {
        CreateUnderWaterGrid(ld);

        mainGridTab = new int[Grid.gridWidth, Grid.gridHeight];

        for (int i = 0; i < Grid.gridWidth; i++)
        {
            for (int j = 0; j < Grid.gridHeight; j++)
            {
                mainGridTab[i, j] = -1;
            }
        }

        for (int i = 0; i < Grid.gridWidth; i++)
        {
            for (int j = 0; j < Grid.gridHeight; j++)
            {
                if(mainGridTab[i, j] < 0)
                {
                    BaseGameObject bgo = ld.CheckGrid(i, j);
                    if (bgo != null && bgo.IsTerrain() > 0)
                    {
                        BaseGameTerrainObject bgto = bgo.GetComponent<BaseGameTerrainObject>();
                        if(bgto != null)
                        {
                            CreateFromLayerData(ld, i, j, bgto);
                        }
                    }
                }
            }
        }
    }


    static public bool CreateFromLayerData(LayerData ld, int x, int y, BaseGameTerrainObject bgto)
    {
        if(x < 0 || x >= Grid.gridWidth || y < 0 || y >= Grid.gridHeight)
        {
            return false;
        }

        int terrainIndex = bgto.IsTerrain();
        float perspectiveOffset = bgto.GetPerspectiveOffset();


        gridTab = new int[Grid.gridWidth, Grid.gridHeight];

        for (int i = 0; i < Grid.gridWidth; i++)
        {
            for (int j = 0; j < Grid.gridHeight; j++)
            {
                gridTab[i, j] = -1;
            }
        }

        gridTab[x, y] = terrainIndex;
        mainGridTab[x, y] = terrainIndex;

        for (int i = 0; ; i++)
        {
            BaseGameObject o = ld.GetObjectPtr(i);
            
            if(o != null)
            {
                if(o.IsTerrain() == terrainIndex && o.GetSelect() == false)
                {
                    int xx = (int)o.gameObject.transform.localPosition.x;
                    int yy = (int)o.gameObject.transform.localPosition.y;

                    gridTab[xx, yy] = terrainIndex;
                    mainGridTab[xx, yy] = terrainIndex;
                }
            }
            else
            {
                break;
            }
        }

        for (int i = 0; i < Grid.gridWidth; i++)
        {
            for (int j = 0; j < Grid.gridHeight; j++)
            {
                if(gridTab[i, j] == terrainIndex)
                {
                    int indexTerrain = CalculateItemIndex(i, j);

                    BaseGameObject bgo = ld.CheckGrid(i, j);

                    if (bgo != null)
                    {
                        int randVal = (int)Random.Range(0, 3);

                        bgo.SetSpriteIndex(randVal * 16 + indexTerrain);
                        bgo.UpdateEnvironment(SystemData.Instance.GetEditorData());
                        if(indexTerrain < 8)
                        {
                            bgo.SetOffsetPosY(perspectiveOffset);
                            bgo.SetColliderOffsetY(-perspectiveOffset);
                            bgo.SetPivotOffset(-perspectiveOffset);
                            bgo.UpdatePositionY(true);
                        }
                        else
                        {
                            bgo.SetOffsetPosY(0.0f);
                            bgo.SetColliderOffsetY(0);
                            bgo.SetPivotOffset(0);
                            bgo.UpdatePositionY(true);
                        }

                        if(bgo is BaseGameTerrainObject) {
                            int underWaterIndex = CalculateUnderWaterItemIndex(i, j);
                            (bgo as BaseGameTerrainObject).underWaterIndex = underWaterIndex;
                            (bgo as BaseGameTerrainObject).underWaterOffset = underWaterIndex < 8 ? 0.5f : 0f;
                        }
                    }
                }
            }
        }

        return true;
    }

    static public bool CreateTerrainFromAreaGenerator(AreaGenerator ag, BaseGameTerrainObject bgto, RoomData rd, int layerIndex)
    {
        LayerData ld = rd.GetLayerByIndex(layerIndex);

        float perspectiveOffset = bgto.GetPerspectiveOffset();

        gridTab = new int[Grid.gridWidth, Grid.gridHeight];
        mainGridTab = new int[Grid.gridWidth, Grid.gridHeight];

        for (int i = 0; i < Grid.gridWidth; i++)
        {
            for (int j = 0; j < Grid.gridHeight; j++)
            {
                gridTab[i, j] = ag.IsFree(i, j) ? 0 : 1;
                mainGridTab[i, j] = gridTab[i, j];
            }
        }

        for (int i = 0; i < Grid.gridWidth; i++)
        {
            for (int j = 0; j < Grid.gridHeight; j++)
            {
                if (gridTab[i, j] > 0) // terrain
                {
                    int indexTerrain = CalculateItemIndex(i, j);
                    BaseGameTerrainObject bgo = GameObject.Instantiate(bgto, ld.GetLayerPtr().transform);
                    bgo.transform.localPosition = new Vector3(i, j, 0);
                    ld.AddObjectPtr(bgo, bgto.gameObject.name);

                    if (bgo != null)
                    {
                        int randVal = (int)Random.Range(0, 3);

                        bgo.SetSpriteIndex(randVal * 16 + indexTerrain);
                        bgo.UpdateEnvironment(SystemData.Instance.GetEditorData());
                        if (indexTerrain < 8)
                        {
                            bgo.SetOffsetPosY(perspectiveOffset);
                            bgo.SetColliderOffsetY(-perspectiveOffset);
                            bgo.UpdatePositionY(true);
                        }
                        else
                        {
                            bgo.SetOffsetPosY(0.0f);
                            bgo.SetColliderOffsetY(0);
                            bgo.UpdatePositionY(true);
                        }
                    }
                }
            }
        }

        return true;
    }

    static public bool CreateFromAreaGenerator(AreaGenerator ag, RoomData rd, int layerIndex)
    {
        LayerData ld = rd.GetLayerByIndex(layerIndex);

        for (int i = 0; i < Grid.gridWidth; i++)
        {
            for (int j = 0; j < Grid.gridHeight; j++)
            {
                if (ag.GetObjectValue(layerIndex, i, j) > 0)
                {
                    float addOffsetY = rd.GetOffsetFor(layerIndex, i, j);

                    GameObject o = SystemData.Instance.GetEditorData().FindObjectByID(ag.GetObjectValue(layerIndex, i, j));
                    if(o != null)
                    {
                        BaseGameObject go = o.GetComponent<BaseGameObject>();

                        BaseGameObject bgb = GameObject.Instantiate(go, ld.GetLayerPtr().transform);
                        bgb.transform.localPosition = new Vector3(i, j + addOffsetY, 0);
                        ld.AddObjectPtr(bgb, go.gameObject.name);
                    }
                }
            }
        }
        return true;
    }

    static public bool CreateTerrainFromRoomInfo(RoomInfo ri, BaseGameTerrainObject bgto, RoomData rd, int layerIndex)
    {
        LayerData ld = rd.GetLayerByIndex(layerIndex);

        float perspectiveOffset = bgto.GetPerspectiveOffset();

        Grid.gridWidth = (int)rd.GetRoomWidth();
        Grid.gridHeight = (int)rd.GetRoomHeight();

        gridTab = new int[(int)rd.GetRoomWidth(), (int)rd.GetRoomHeight()];

        for (int i = 0; i < (int)rd.GetRoomWidth(); i++)
        {
            for (int j = 0; j < (int)rd.GetRoomHeight(); j++)
            {
                int id = ri.layers[layerIndex].layerGrid[i, j];
                gridTab[i, j] = id == 0 ? 0 : 1;
            }
        }

        for (int i = 0; i < Grid.gridWidth; i++)
        {
            for (int j = 0; j < Grid.gridHeight; j++)
            {
                if (gridTab[i, j] > 0) // terrain
                {
                    int indexTerrain = CalculateItemIndex(i, j);
                    BaseGameTerrainObject bgo = GameObject.Instantiate(bgto, ld.GetLayerPtr().transform);
                    bgo.transform.localPosition = new Vector3(i, j, 0);
                    ld.AddObjectPtr(bgo, bgto.gameObject.name);

                    if (bgo != null)
                    {
                        int randVal = (int)Random.Range(0, 3);

                        bgo.SetSpriteIndex(randVal * 16 + indexTerrain);
                        bgo.UpdateEnvironment(SystemData.Instance.GetEditorData());
                        if (indexTerrain < 8)
                        {
                            bgo.SetOffsetPosY(perspectiveOffset);
                            bgo.SetColliderOffsetY(-perspectiveOffset);
                            bgo.UpdatePositionY(true);
                        }
                        else
                        {
                            bgo.SetOffsetPosY(0.0f);
                            bgo.SetColliderOffsetY(0);
                            bgo.UpdatePositionY(true);
                        }
                    }
                }
            }
        }

        return true;
    }

    static public bool CreateFromRoomInfo(RoomInfo ri, RoomData rd, int layerIndex)
    {
        LayerData ld = rd.GetLayerByIndex(layerIndex);

        for (int i = 0; i < (int)rd.GetRoomWidth(); i++)
        {
            for (int j = 0; j < (int)rd.GetRoomHeight(); j++)
            {
                int id = ri.layers[layerIndex].layerGrid[i, j];
                if (id > 0)
                {
                    float addOffsetY = rd.GetOffsetFor(layerIndex, i, j);

                    GameObject o = SystemData.Instance.GetEditorData().FindObjectByID(id);
                    if (o != null)
                    {
                        BaseGameObject go = o.GetComponent<BaseGameObject>();

                        BaseGameObject bgb = GameObject.Instantiate(go, ld.GetLayerPtr().transform);
                        bgb.transform.localPosition = new Vector3(i, j + addOffsetY, 0);
                        ld.AddObjectPtr(bgb, go.gameObject.name);
                    }
                }
            }
        }
        return true;
    }

    public static void CreateUnderWaterGrid(LayerData ld) {
        underWaterGridTab = new int[Grid.gridWidth, Grid.gridHeight];

        for (int i = 0; ; i++) {
            BaseGameObject o = ld.GetObjectPtr(i);

            if (o != null) {
                if (o.IsTerrain() > 0 && o.GetSelect() == false) {

                    int xx = (int)o.gameObject.transform.localPosition.x;
                    int yy = (int)o.gameObject.transform.localPosition.y;

                    if(xx >= 0 && xx < Grid.gridWidth && yy >= 0 && yy < Grid.gridHeight)
                    {
                        underWaterGridTab[xx, yy] = 1;
                    }
                }
            }
            else {
                break;
            }
        }
    }


    static int CalculateItemIndex(int x, int y)
    {
        int nRet = 0;

        int terrainIndex = gridTab[x, y];

        if (x > 0)
        {
            if (gridTab[x - 1, y] == terrainIndex)
            {
                nRet |= 1;
            }
        }
        else
        {
            nRet |= 1;
        }

        if (y < Grid.gridHeight - 1)
        {
            if (gridTab[x, y + 1] == terrainIndex)
            {
                nRet |= 2;
            }
        }
        else
        {
            nRet |= 2;
        }

        if (x < Grid.gridWidth - 1)
        {
            if (gridTab[x + 1, y] == terrainIndex)
            {
                nRet |= 4;
            }
        }
        else
        {
            nRet |= 4;
        }

        if (y > 0)
        {
            if (gridTab[x, y - 1] != terrainIndex)
            {
                nRet += 8;
            }
        }
        else
        {
            nRet += 8;
        }

        return nRet;
    }

    static int CalculateUnderWaterItemIndex(int x, int y) {
        int nRet = 0;

        if (x <= 0 || underWaterGridTab[x - 1, y] > 0)
            nRet |= 1;

        if (y >= Grid.gridHeight - 1 || underWaterGridTab[x, y + 1] > 0)
            nRet |= 2;

        if (x >= Grid.gridWidth - 1 || underWaterGridTab[x + 1, y] > 0)
            nRet |= 4;

        if (y <= 0 || underWaterGridTab[x, y - 1] == 0)
            nRet += 8;

        return nRet;
    }
}