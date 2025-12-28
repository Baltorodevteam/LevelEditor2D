using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo
{
    public int index;
    public int x;
    public int y;
    public int w;
    public int h;

    public int leftEdgeDoor;
    public int topEdgeDoor;
    public int rightEdgeDoor;
    public int bottomEdgeDoor;

    public float difficulty;

    public LayerInfo[] layers = null;

    public void PrepareLayers()
    {
        layers = new LayerInfo[SystemData.NR_OF_LAYERS];
        for (int i = 0; i < SystemData.NR_OF_LAYERS; i++)
        {
            layers[i] = new LayerInfo();
            layers[i].Prepare(this, i);
        }
    }

    public void LoadFromJsonRoomSerializer(JsonRoomSerializer rs)
    {
        index = rs.roomIndex;
        x = rs.roomX;
        y = rs.roomY;
        w = rs.roomWidth;
        h = rs.roomHeight;
        leftEdgeDoor = rs.leftEdgeDoor;
        topEdgeDoor = rs.topEdgeDoor;
        rightEdgeDoor = rs.rightEdgeDoor;
        bottomEdgeDoor = rs.bottomEdgeDoor;

        difficulty = rs.difficulty;

        layers = new LayerInfo[rs.layersList.Count];

        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = new LayerInfo();
            layers[i].LoadFromJsonLayerSerializer(rs.layersList[i]);
        }

        /*
        enemyList = new List<JsonEnemyDataSerializer>();
        List<EnemyData> ed = rd.GetEnemiesData();
        for (int i = 0; i < ed.Count; i++)
        {
            JsonEnemyDataSerializer eds = ed[i].ToJson();
            enemyList.Add(eds);
        }
        */
    }
}
