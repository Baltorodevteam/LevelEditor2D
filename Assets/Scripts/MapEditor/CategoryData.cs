using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newCategory", menuName = "CategoryData")]
public class CategoryData : ScriptableObject
{
    public string categoryName;
    public Sprite categoryIcon;
    public GameObject[] objects;
    public Sprite[] icons;


    public GameObject FindObjectByName(string name)
    {
        for(int i = 0; i < objects.Length; i++)
        {
            if(objects[i].name.Equals(name))
            {
                return objects[i];
            }
        }
        return null;
    }

    public GameObject FindObjectByID(int id)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            BaseGameObject bgo = objects[i].GetComponent<BaseGameObject>();
            if(bgo != null && bgo.objectID == id)
            {
                return objects[i];
            }
        }
        return null;
    }

}
