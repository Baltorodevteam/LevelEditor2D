using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerInfo
{
    public int layerNr;
    public int w;
    public int h;
    public int[,] layerGrid = null;

    public void Prepare(RoomInfo ri, int nr)
    {
        layerNr = nr;
        w = ri.w * (int)RoomData.defaultRoomWidth;
        h = ri.h * (int)RoomData.defaultRoomHeight;
        layerGrid = new int[w, h];

        Clear();
    }

    public void Clear()
    {
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                layerGrid[x, y] = 0;
            }
        }
    }

    public void LoadFromJsonLayerSerializer(JsonLayerSerializer ls)
    {
        layerNr = ls.layerNr;
        w = ls.w;
        h = ls.h;
        layerGrid = new int[w, h];
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                layerGrid[x,y] = ls.layerGrid[y * w + x];
            }
        }
    }
}
