using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEditorData", menuName = "EditorData")]
public class EditorData : ScriptableObject
{
    [Header("Object categories")]
    public CategoryData[] objectCategories;

    [Header("Environments")]
    public EnvironmentData[] environments;

    [Header("Prefabs...")]
    public BaseGameBlendingColorTerrainObject blendingColorUnderWaterTerrainObjectPrefab;
    public BaseGameObject[] waterFoamObjectPrefab;
    public GameObject       backgroundObjectPrefab;

    [Header("Current environment index")]
    public int currentEnvironment = 0;

    [Header("Current music index")]
    public int currentMusic = 0;

    public GameObject FindObjectByName(string name)
    {
        for(int i = 0; i < objectCategories.Length; i++)
        {
            GameObject o = objectCategories[i].FindObjectByName(name);
            if(o != null)
            {
                return o;
            }
        }

        if(blendingColorUnderWaterTerrainObjectPrefab.name.Equals(name))
        {
            return blendingColorUnderWaterTerrainObjectPrefab.gameObject;
        }

        if (backgroundObjectPrefab.name.Equals(name))
        {
            return backgroundObjectPrefab.gameObject;
        }

        return null;
    }

    public GameObject FindObjectByID(int id)
    {
        for (int i = 0; i < objectCategories.Length; i++)
        {
            GameObject o = objectCategories[i].FindObjectByID(id);
            if (o != null)
            {
                return o;
            }
        }
        return null;
    }

    public EnvironmentData GetCurrentEnvironmentData()
    {
        return environments[currentEnvironment];
    }
}
