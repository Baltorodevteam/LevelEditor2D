using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AreaObjectParams
{
    public int objectID;
    public int layerIndex;
    public int probabilityOfOccurrence;
    public int width;
    public int height;
    public int margin;
    public int groundMask; // 0 - no, 1 - on the ground, 2 - off the ground
};

[System.Serializable]
public class AreaGeneratorParams
{
    public int randomStartPercentValue = 20;
    public int tresholdIfEmpty = 4;
    public int tresholdIfTaken = 5;
    public int finalExpectedPercentValue = 30;
    public int perlinNoisePropability = 50;
    public int regularAreaPropability = 60;
    public int[] groundLayers = { 0, 3 };
    public int[] enviroPropability = { 0, 30, 20, 0, 35, 20, 0, 0, 0 };

    public List<AreaObjectParams> objectList = new List<AreaObjectParams>();

    public void AddObjectParams(int id, int layer, int propabilityOfOccurence, int w, int h, int mask)
    {
        AreaObjectParams o = new AreaObjectParams();
        o.objectID = id;
        o.layerIndex = layer;
        o.probabilityOfOccurrence = propabilityOfOccurence;
        o.width = w;
        o.height = h;
        o.groundMask = mask;
        objectList.Add(o);
    }


    public static void Load(string fileName, ref AreaGeneratorParams lgp)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string jsonContent = File.ReadAllText(path);

        AreaGeneratorParams loaded = JsonUtility.FromJson<AreaGeneratorParams>(jsonContent);
        if (loaded != null)
        {
            lgp = loaded;
        }
    }

    public static void Save(string fileName, AreaGeneratorParams lgp)
    {
        string jsonContent = JsonUtility.ToJson(lgp, true);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, jsonContent);
    }

    public AreaObjectParams GetObjectByID(int id)
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i].objectID == id)
            {
                return objectList[i];
            }
        }
        return null;
    }

    public List<AreaObjectParams> GetObjectsByLayer(int layer)
    {
        List<AreaObjectParams> l = new List<AreaObjectParams>();
        for(int i = 0; i < objectList.Count; i++)
        {
            if(objectList[i].layerIndex == layer)
            {
                l.Add(objectList[i]);
            }
        }
        return l;
    }

    public List<AreaObjectParams> GetObjectsByMask(int mask, List<AreaObjectParams> srcList)
    {
        List<AreaObjectParams> l = new List<AreaObjectParams>();
        for (int i = 0; i < srcList.Count; i++)
        {
            if (srcList[i].groundMask == mask)
            {
                l.Add(srcList[i]);
            }
        }
        return l;
    }
};
