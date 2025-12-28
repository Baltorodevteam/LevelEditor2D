using System.Collections.Generic;
using UnityEngine;

public class SelectionList
{
    List<BaseGameObject> selectionListPtr = new List<BaseGameObject>();

    public int Add(BaseGameObject go)
    {
        go.SelectMe(true);
        selectionListPtr.Add(go);
        
        return selectionListPtr.Count;
    }

    public void Remove(BaseGameObject go)
    {
        JoinObjectPtrToLayer(go);
        go.SelectMe(false);
        selectionListPtr.Remove(go);
    }

    public void RemoveAll()
    {
        for (int i = 0; i < selectionListPtr.Count; i++)
        {
            JoinObjectPtrToLayer(selectionListPtr[i]);
            selectionListPtr[i].SelectMe(false);
        }
        selectionListPtr.Clear();

        Map.Instance.HideSelectedPivot();
    }

    public void DestroyAllPtr()
    {
        for (int i = selectionListPtr.Count - 1; i >= 0; i--)
        {
            int layerIndex = selectionListPtr[i].GetDefaultLayer();

            SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(layerIndex).RemoveObjectPtr(selectionListPtr[i]);
            GameObject.Destroy(selectionListPtr[i].gameObject);
        }

        selectionListPtr.Clear();

        Map.Instance.HideSelectedPivot();
    }

    public int GetSize()
    {
        return selectionListPtr.Count;
    }

    public int Find(BaseGameObject go)
    {
        for (int i = 0; i < selectionListPtr.Count; i++)
        {
            if (selectionListPtr[i] == go)
            {
                return i;
            }
        }
        return -1;
    }

    public List<BaseGameObject> GetList()
    {
        return selectionListPtr;
    }

    public BaseGameObject GetByIndex(int i)
    {
        if(i >= 0 && i < selectionListPtr.Count)
        {
            return selectionListPtr[i];
        }
        return null;
    }

    Vector3 tmpVec3 = new Vector3();
    public Vector3 GetPivotPosition()
    {
        tmpVec3.z = -1;
        if (selectionListPtr.Count == 0)
        {
            tmpVec3.x = -10000;
            tmpVec3.y = -10000;
        }
        else
        {
            tmpVec3.x = selectionListPtr[0].transform.position.x;
            tmpVec3.y = selectionListPtr[0].transform.position.y;

            for(int i = 1; i < selectionListPtr.Count; i++)
            {
                tmpVec3.x += selectionListPtr[i].transform.position.x;
                tmpVec3.y += selectionListPtr[i].transform.position.y;
            }

            tmpVec3.x /= selectionListPtr.Count;
            tmpVec3.y /= selectionListPtr.Count;

            tmpVec3.x = (int)tmpVec3.x;
            tmpVec3.y = (int)tmpVec3.y;
        }

        return tmpVec3;
    }

    public void JoinObjectPtrToLayer(BaseGameObject go)
    {
        LayerData ld = SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(go.GetDefaultLayer());
        go.transform.parent = ld.GetLayerPtr().transform;
        go.SetSortingLayerID(go.GetDefaultLayer());
    }

    public void JoinObjectsPtrToLayers()
    {
        for (int i = selectionListPtr.Count - 1; i >= 0; i--)
        {
            LayerData ld = SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(selectionListPtr[i].GetDefaultLayer());
            selectionListPtr[i].transform.parent = ld.GetLayerPtr().transform;
            selectionListPtr[i].SetSortingLayerID(selectionListPtr[i].GetDefaultLayer());
        }
    }

    public void JoinObjectsPtrToPivot()
    {
        for (int i = selectionListPtr.Count - 1; i >= 0; i--)
        {
            selectionListPtr[i].transform.parent = Map.Instance.GetSelectedPivotPtr().transform;
            selectionListPtr[i].SetTopSortingLayer();
        }
    }

    public bool CheckPosition()
    {
        bool bOK = true;
        for (int i = selectionListPtr.Count - 1; i >= 0; i--)
        {
            if(!TemplateObject.Instance.IsGoodPlaceForObject(selectionListPtr[i]))
            {
                bOK = false;
                break;
            }
        }

        if(bOK == true)
        {
            for (int i = selectionListPtr.Count - 1; i >= 0; i--)
            {
                selectionListPtr[i].SelectMe(true);
            }
            return true;
        }
        else
        {
            for (int i = selectionListPtr.Count - 1; i >= 0; i--)
            {
                selectionListPtr[i].SelectBlockedMe(true);
            }
            return false;
        }
    }

}