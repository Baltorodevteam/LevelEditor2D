using UnityEngine;
using System.Collections.Generic;
using System.IO;


[System.Serializable]
public class JsonPrefabSerializer
{
    public string prefabName;

    public float x;
    public float y;
    public float z;
    public float w;
    public float h;
    public float angle;
    public int spriteIndex;
    public int enemyID;
    public string data;

    static public bool IsTheSame(JsonPrefabSerializer o1, JsonPrefabSerializer o2)
    {
        if (o1.x != o2.x || o1.y != o2.y || o1.z != o2.z /*|| o1.r != o2.r*/ || o1.spriteIndex != o2.spriteIndex || o1.w != o2.w || o1.h != o2.h || o1.enemyID != o2.enemyID)
        {
            return false;
        }
        if (!o1.prefabName.Contains(o2.prefabName))
        {
            return false;
        }
        return true;
    }
}

[System.Serializable]
public class JsonLayerSerializer
{
    public int layerNr;
    public int categoryID;
    public int w;
    public int h;
    public int[] layerGrid;
    public List<JsonPrefabSerializer> objectsList = new List<JsonPrefabSerializer>();

    public void CreateFromLayerData(LayerData ld)
    {
        layerNr = ld.GetLayerNr();
        categoryID = ld.GetCategoryID();

        int objectsCount = ld.GetObjectsCount();
        objectsList = new List<JsonPrefabSerializer>();
        for(int i = 0; i < objectsCount; i++)
        {
            JsonPrefabSerializer prefab = new JsonPrefabSerializer();

            prefab.prefabName = ld.GetObjectName(i);
            prefab.x = ld.GetObjectPtr(i).transform.localPosition.x;
            prefab.y = ld.GetObjectPtr(i).transform.localPosition.y;
            prefab.z = ld.GetObjectPtr(i).transform.localPosition.z;
            prefab.angle = ld.GetObjectPtr(i).transform.localEulerAngles.z;
            prefab.spriteIndex = ld.GetObjectPtr(i).GetSpriteIndex();
            prefab.w = ld.GetObjectPtr(i).GetSpriteRenderer().size.x;
            prefab.h = ld.GetObjectPtr(i).GetSpriteRenderer().size.y;
            prefab.data = ld.GetObjectPtr(i).GetData();
            prefab.enemyID = -1;

            objectsList.Add(prefab);
        }
    }

    public void CreateFromLayerInfo(LayerInfo ld)
    {
        layerNr = ld.layerNr;
        w = ld.w;
        h = ld.h;
        layerGrid = new int[w * h];
        for(int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                layerGrid[y * w + x] = ld.layerGrid[x, y];
            }
        }
    }

    static public bool IsTheSame(JsonLayerSerializer l1, JsonLayerSerializer l2)
    {
        if (l1.layerNr != l2.layerNr || l1.categoryID != l2.categoryID)
        {
            return false;
        }

        if (l1.objectsList.Count != l2.objectsList.Count)
        {
            return false;
        }

        for (int i = 0; i < l1.objectsList.Count; i++)
        {
            if (!JsonPrefabSerializer.IsTheSame(l1.objectsList[i], l2.objectsList[i]))
            {
                return false;
            }
        }
        return true;
    }
}

[System.Serializable]
public class JsonEnemyDataSerializer
{
    public int enemyID;
    public int type;
    public int layerIndex;
    public int layerMask;
    public int timeout;
    public int waveID;

    public float baseHp;
    public float baseDamage;
    public float armor;
    public float fireRate;
    public float range;
    public float moveSpeed;
    public int areaGridW;
    public int areaGridH;

    static public bool IsTheSame(JsonEnemyDataSerializer e1, JsonEnemyDataSerializer e2)
    {
        return (e1.enemyID == e2.enemyID);
    }
}

[System.Serializable]
public class JsonRoomSerializer
{
    public int roomIndex;

    public int roomX;
    public int roomY;

    public int roomWidth;
    public int roomHeight;

    public int leftEdgeDoor;
    public int topEdgeDoor;
    public int rightEdgeDoor;
    public int bottomEdgeDoor;

    public float difficulty;

    public List<JsonLayerSerializer> layersList = new List<JsonLayerSerializer>();
    public List<JsonEnemyDataSerializer> enemyList = new List<JsonEnemyDataSerializer>();

    public void CreateFromRoomData(RoomData rd)
    {
        roomX = rd.GetRoomGridX();
        roomY = rd.GetRoomGridY();
        roomWidth = (int)rd.GetRoomWidth();
        roomHeight = (int)rd.GetRoomHeight();
        leftEdgeDoor = rd.leftEdgeDoor;
        topEdgeDoor = rd.topEdgeDoor;
        rightEdgeDoor = rd.rightEdgeDoor;
        bottomEdgeDoor = rd.bottomEdgeDoor;

        difficulty = rd.difficulty;

        int layersCount = rd.GetLayersCount();
        layersList = new List<JsonLayerSerializer>();
        for (int i = 0; i < layersCount; i++)
        {
            JsonLayerSerializer layer = new JsonLayerSerializer();
            layer.CreateFromLayerData(rd.GetLayerByIndex(i));
            layersList.Add(layer);
        }

        enemyList = new List<JsonEnemyDataSerializer>();
        List<EnemyData> ed = rd.GetEnemiesData();
        for(int i = 0; i <ed.Count; i++)
        {
            JsonEnemyDataSerializer eds = ed[i].ToJson();
            enemyList.Add(eds);
        }
    }

    public void CreateFromRoomInfo(RoomInfo rd)
    {
        roomIndex = rd.index;
        roomX = rd.x;
        roomY = rd.y;
        roomWidth = rd.w;
        roomHeight = rd.h;
        leftEdgeDoor = rd.leftEdgeDoor;
        topEdgeDoor = rd.topEdgeDoor;
        rightEdgeDoor = rd.rightEdgeDoor;
        bottomEdgeDoor = rd.bottomEdgeDoor;

        difficulty = rd.difficulty;

        int layersCount = rd.layers.Length;
        layersList = new List<JsonLayerSerializer>();
        for (int i = 0; i < layersCount; i++)
        {
            JsonLayerSerializer layer = new JsonLayerSerializer();
            layer.CreateFromLayerInfo(rd.layers[i]);
            layersList.Add(layer);
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

    static public bool IsTheSame(JsonRoomSerializer l1, JsonRoomSerializer l2)
    {
        if(l1.leftEdgeDoor != l2.leftEdgeDoor || l1.topEdgeDoor != l2.topEdgeDoor || l1.rightEdgeDoor != l2.rightEdgeDoor || l1.bottomEdgeDoor != l2.bottomEdgeDoor)
        {
            return false;
        }

        if (l1.layersList.Count != l2.layersList.Count)
        {
            return false;
        }

        for (int i = 0; i < l1.layersList.Count; i++)
        {
            if (!JsonLayerSerializer.IsTheSame(l1.layersList[i], l2.layersList[i]))
            {
                return false;
            }
        }

        if (l1.enemyList.Count != l2.enemyList.Count)
        {
            return false;
        }

        for (int i = 0; i < l1.enemyList.Count; i++)
        {
            if (!JsonEnemyDataSerializer.IsTheSame(l1.enemyList[i], l2.enemyList[i]))
            {
                return false;
            }
        }

        return true;
    }
}

[System.Serializable]
public class JsonLevelSerializer
{
    public int levelWidth;
    public int levelHeight;

    public int levelMusicTrackIndex;

    public int currentRoomX;
    public int currentRoomY;

    public int startRoom;
    public int endRoom;

    public List<JsonRoomSerializer> Rooms;

    public void CreateFromLevelData(LevelData ld)
    {
        levelWidth = ld.GetLevelWidth();
        levelHeight = ld.GetLevelHeight();

        levelMusicTrackIndex = ld.GetLevelMusicTrackIndex();

        currentRoomX = ld.GetCurrentRoom().GetRoomGridX();
        currentRoomY = ld.GetCurrentRoom().GetRoomGridY();

        int roomsCount = ld.CountRooms();

        Rooms = new List<JsonRoomSerializer>();

        for(int i = 0; i < roomsCount; i++)
        {
            JsonRoomSerializer rs = new JsonRoomSerializer();
            rs.roomIndex = ld.GetRoom(i).GetRoomGridY() * levelWidth + ld.GetRoom(i).GetRoomGridX();
            rs.CreateFromRoomData(ld.GetRoom(i));
            Rooms.Add(rs);
        }
    }

    public void CreateFromLevelInfo(LevelInfo ld)
    {
        levelWidth = ld.levelWidth;
        levelHeight = ld.levelHeight;

        levelMusicTrackIndex = 0;

        int roomsCount = ld.roomsList.Count;

        currentRoomX = 0;
        currentRoomY = 0;
        if(roomsCount > 0)
        {
            currentRoomX = ld.roomsList[0].x;
            currentRoomY = ld.roomsList[0].y;
        }

        startRoom = ld.startRoomIndex;
        endRoom = ld.endRoomIndex;

        Rooms = new List<JsonRoomSerializer>();

        for (int i = 0; i < roomsCount; i++)
        {
            JsonRoomSerializer rs = new JsonRoomSerializer();
            rs.roomIndex = ld.roomsList[i].y * levelWidth + ld.roomsList[i].x;
            rs.CreateFromRoomInfo(ld.roomsList[i]);
            Rooms.Add(rs);
        }
    }
}

public class JsonWriterReader
{
    static public JsonLevelSerializer levelSerializer;

    static public void Save(string fileName, LevelData ld)
    {
        levelSerializer = new JsonLevelSerializer();
        levelSerializer.CreateFromLevelData(ld);

        string str = JsonUtility.ToJson(levelSerializer, true);
        
        string filepath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(filepath, str);

        levelSerializer = null;
    }

    static public void Save(string filePath, LevelInfo ld)
    {
        levelSerializer = new JsonLevelSerializer();
        levelSerializer.CreateFromLevelInfo(ld);

        string str = JsonUtility.ToJson(levelSerializer, true);
        File.WriteAllText(filePath, str);

        levelSerializer = null;
    }

    public static bool Load(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);
        levelSerializer = JsonUtility.FromJson<JsonLevelSerializer>(jsonContent);
        return levelSerializer != null;
    }


    static public void SaveRoom(string fileName, RoomData rd)
    {
        JsonRoomSerializer roomSerializer = new JsonRoomSerializer();
        roomSerializer.CreateFromRoomData(rd);

        string str = JsonUtility.ToJson(roomSerializer, true);

        string filepath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(filepath, str);
    }

    public static bool LoadRoom(string fileName, RoomData rd)
    {
        string filepath = Path.Combine(Application.persistentDataPath, fileName);
        string str = File.ReadAllText(filepath);

        JsonRoomSerializer roomSerializer = JsonUtility.FromJson<JsonRoomSerializer>(str);
        if (roomSerializer != null)
        {
            rd.LoadFromJsonRoomSerializer(roomSerializer);
            return true;
        }
        return false;
    }
}