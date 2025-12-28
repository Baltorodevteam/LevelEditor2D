using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class LevelGeneratorParams
{
    public int levelWidth = 12;
    public int levelHeight = 12;

    public int minRooms = 5;
    public int maxRooms = 7;

    public int minRoomSize = 1;
    public int maxRoomSize = 1;

    public int algorithmNr = 1;

    public int[] roomSizePercentTab;

    public float startDiff = 0;
    public float endDiff = 0;

    public static void Load(string fileName, ref LevelGeneratorParams lgp)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string jsonContent = File.ReadAllText(path);

        LevelGeneratorParams loaded = JsonUtility.FromJson<LevelGeneratorParams>(jsonContent);
        if (loaded != null)
        {
            lgp = loaded;
        }
    }

    public static void Save(string fileName, LevelGeneratorParams lgp)
    {
        string jsonContent = JsonUtility.ToJson(lgp, true);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, jsonContent);
    }
}