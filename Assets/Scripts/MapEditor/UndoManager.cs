using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class UndoManager
{
    static List<JsonRoomSerializer> undoList = new List<JsonRoomSerializer>();
    static List<JsonRoomSerializer> redoList = new List<JsonRoomSerializer>();


    static public void ClearUndoList()
    {
        undoList = new List<JsonRoomSerializer>();
    }

    static public void ClearRedoList()
    {
        redoList = new List<JsonRoomSerializer>();
    }

    static public void AddToUndo(bool clearRedo = true)
    {
        if (clearRedo)
        {
            ClearRedoList();
        }

        JsonRoomSerializer uld = new JsonRoomSerializer();
        uld.CreateFromRoomData(SystemData.Instance.GetLevelData().GetCurrentRoom());

        if (undoList.Count > 0)
        {
            if (JsonRoomSerializer.IsTheSame(uld, undoList[undoList.Count - 1]))
            {
                return;
            }
        }

        if (undoList.Count > 20)
        {
            undoList.RemoveAt(0);
        }

        undoList.Add(uld);
    }

    static public void AddToRedo()
    {
        JsonRoomSerializer uld = new JsonRoomSerializer();
        uld.CreateFromRoomData(SystemData.Instance.GetLevelData().GetCurrentRoom());

        if (redoList.Count > 0)
        {
            if (JsonRoomSerializer.IsTheSame(uld, redoList[redoList.Count - 1]))
            {
                return;
            }
        }

        redoList.Add(uld);
    }

    static public int GetUndoListSize()
    {
        return undoList.Count;
    }

    static public int GetRedoListSize()
    {
        return redoList.Count;
    }

    static public void Undo()
    {
        if (undoList.Count > 0)
        {
            AddToRedo();
            int nLayerIndex = SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayerIndex();
            SystemData.Instance.GetLevelData().GetCurrentRoom().LoadFromJsonRoomSerializer(undoList[undoList.Count - 1]);
            undoList.RemoveAt(undoList.Count - 1);
            SystemData.Instance.GetLevelData().GetCurrentRoom().SelectLayerByIndex(nLayerIndex);
        }
    }

    static public void Redo()
    {
        if (redoList.Count > 0)
        {
            AddToUndo(false);
            int nLayerIndex = SystemData.Instance.GetLevelData().GetCurrentRoom().GetSelectedLayerIndex();
            SystemData.Instance.GetLevelData().GetCurrentRoom().LoadFromJsonRoomSerializer(redoList[redoList.Count - 1]);
            redoList.RemoveAt(redoList.Count - 1);
            SystemData.Instance.GetLevelData().GetCurrentRoom().SelectLayerByIndex(nLayerIndex);
        }
    }

}