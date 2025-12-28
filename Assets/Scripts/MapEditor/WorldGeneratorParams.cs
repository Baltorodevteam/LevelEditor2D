using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class WorldGeneratorParams
{
    public int nrOfLevels = 20;
    public int levelsSize = 25;
    public int nrOfRoomsAtFirstLevel = 8;
    public int nrOfRoomsAtLastLevel = 30;
    public float startDiff = 0;
    public float endDiff = 1.0f;

    public static void Load(string fileName, ref WorldGeneratorParams wgp)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string jsonContent = File.ReadAllText(path);

        WorldGeneratorParams loaded = JsonUtility.FromJson<WorldGeneratorParams>(jsonContent);
        if (loaded != null)
        {
            wgp = loaded;
        }
    }

    public static void Save(string fileName, WorldGeneratorParams wgp)
    {
        string jsonContent = JsonUtility.ToJson(wgp, true);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, jsonContent);
    }
}
