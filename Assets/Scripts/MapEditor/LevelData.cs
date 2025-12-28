using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
 
public class LevelData
{
    string levelName = "level_001";
    int levelWidth = LevelGenerator.levelWidth;
    int levelHeight = LevelGenerator.levelHeight;

    List<RoomData> roomsList = new List<RoomData>();

    RoomData currentRoom = null;

    int levelMusicTrackIndex = 0;


    SelectionList selectionList = new SelectionList();



    public SelectionList GetSelectionList()
    {
        return selectionList;
    }

    public void SetLevelMusicTrackIndex(int i)
    {
        levelMusicTrackIndex = i;
    }

    public int GetLevelMusicTrackIndex()
    {
        return levelMusicTrackIndex;
    }

    public string GetLevelName()
    {
        return levelName;
    }

    public int CountRooms()
    {
        return roomsList.Count;
    }

    public RoomData GetCurrentRoom()
    {
        return currentRoom;
    }

    public void SetCurrentRoom(RoomData rd)
    {
        if (currentRoom != rd)
        {
            selectionList.RemoveAll();

            currentRoom = rd;
            UndoManager.ClearUndoList();
            UndoManager.ClearRedoList();

            Map.Instance.CameraPositionToCenterOfRoom(rd);
            Grid.Instance.GenerateGrid(rd);
        }
    }

    public void SetCurrentRoom(float uvX, float uvY)
    {
        int gridX = (int)(uvX * (float)levelWidth);
        int gridY = (int)(uvY * (float)levelHeight);

        for(int i = 0; i < roomsList.Count; i++)
        {
            if(roomsList[i].GetRoomGridX() <= gridX && roomsList[i].GetRoomGridX() + (int)(roomsList[i].GetRoomWidth() / RoomData.defaultRoomWidth) - 1 >= gridX 
                && roomsList[i].GetRoomGridY() <= gridY && roomsList[i].GetRoomGridY() + (int)(roomsList[i].GetRoomHeight() / RoomData.defaultRoomHeight) - 1 >= gridY)
            {
                SetCurrentRoom(roomsList[i]);
                return;
            }
        }
    }

    public int GetLevelWidth()
    {
        return levelWidth;
    }

    public int GetLevelHeight()
    {
        return levelHeight;
    }

    public RoomData GetRoom(int i)
    {
        return roomsList[i];
    }

    public RoomData GetRoom(int x, int y)
    {
        for (int i = 0; i < roomsList.Count; i++)
        {
            if (roomsList[i].GetRoomGridX() <= x && roomsList[i].GetRoomGridX() + (int)(roomsList[i].GetRoomWidth()/RoomData.defaultRoomWidth) - 1 >= x
                && roomsList[i].GetRoomGridY() <= y && roomsList[i].GetRoomGridY() + (int)(roomsList[i].GetRoomHeight()/RoomData.defaultRoomHeight) - 1 >= y)
            {
                RoomData rd = roomsList[i]; ;
                return roomsList[i];
            }
        }
        return null;
    }

    public bool CanAddRoom(int l, int t, int w, int h)
    {
        for(int x = l; x < l + w; x++)
        {
            for (int y = t; y < t + h; y++)
            {
                if(GetRoom(x,y) != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public RoomData AddRoom(int l, int t, int w, int h)
    {
        RoomData rd = new RoomData(l, t, w * RoomData.defaultRoomWidth, h * RoomData.defaultRoomHeight);
        Map.Instance.AddRoom(l, t);
        rd.InitDataLayers(SystemData.NR_OF_LAYERS);
        roomsList.Add(rd);
        return rd;
    }

    public void DeleteRoom(RoomData rd)
    {
        int x = rd.GetRoomGridX();
        int y = rd.GetRoomGridY();

        if(Map.Instance.DeleteRoom(x, y))
        {
            for (int i = 0; i < roomsList.Count; i++)
            {
                if (roomsList[i] == rd)
                {
                    roomsList.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public bool HasRoomNeighbourUp(RoomData rd)
    {
        return GetRoom(rd.GetRoomGridX(), rd.GetRoomGridY() + 1) != null;
    }

    public bool HasRoomNeighbourBottom(RoomData rd)
    {
        return GetRoom(rd.GetRoomGridX(), rd.GetRoomGridY() - 1) != null;
    }

    public bool HasRoomNeighbourLeft(RoomData rd)
    {
        return GetRoom(rd.GetRoomGridX() - 1, rd.GetRoomGridY()) != null;
    }

    public bool HasRoomNeighbourRight(RoomData rd)
    {
        return GetRoom(rd.GetRoomGridX() + 1, rd.GetRoomGridY()) != null;
    }

    public void EnableColliders(bool v)
    {
        for (int i = 0; i < roomsList.Count; i++)
        {
            roomsList[i].EnableColliders(v);
        }
    }

    public void UpdateEnvironment(EditorData ed)
    {
        for (int i = 0; i < roomsList.Count; i++)
        {
            roomsList[i].UpdateEnvironment(ed);
        }
    }

    public void CreateFromGenerator()
    {
        levelWidth = LevelGenerator.GetGridWidth();
        levelHeight = LevelGenerator.GetGridHeight();

        int nCount = LevelGenerator.CountRooms();
        roomsList = new List<RoomData>();

        RoomData defaultRoomData = null;

        int minXY = 10000;

        for (int i = 1; i <= nCount; i++)
        {
            int left, top, w, h;
            RoomInfo ri = LevelGenerator.FindRoomByIndex(i, out left, out top, out w, out h);
            if (ri != null)
            {
                RoomData rd = AddRoom(left, top, w, h);
                rd.leftEdgeDoor = ri.leftEdgeDoor;
                rd.rightEdgeDoor = ri.rightEdgeDoor;
                rd.topEdgeDoor = ri.topEdgeDoor;
                rd.bottomEdgeDoor = ri.bottomEdgeDoor;

                int xy = top * levelWidth + left;
                if(xy < minXY)
                {
                    minXY = xy;
                    defaultRoomData = rd;
                }
            }
            else
            {
                Debug.LogError("ERROR: (CreateFromGenerator) Unable find room index " + i);
            }
        }

        CreateBackGrounds();

        selectionList = new SelectionList();

        SetCurrentRoom(defaultRoomData);
    }

    public void CreateFromLevelInfo(LevelInfo li)
    {
        levelWidth = li.levelWidth;
        levelHeight = li.levelHeight;

        int nCount = li.roomsList.Count;
        roomsList = new List<RoomData>();

        RoomData defaultRoomData = null;

        int minXY = 10000;

        for (int i = 0; i < nCount; i++)
        {
            RoomInfo ri = li.roomsList[i];
            if (ri != null)
            {
                RoomData rd = AddRoom(ri.x, ri.y, ri.w, ri.h);
                rd.CreateFromRoomInfo(ri);

                rd.isStartRoom = li.GetStartRoom() == ri;
                rd.isEndRoom = li.GetEndRoom() == ri;

                int xy = ri.y * levelWidth + ri.x;
                if (xy < minXY)
                {
                    minXY = xy;
                    defaultRoomData = rd;
                }
            }
            else
            {
                Debug.LogError("ERROR: (CreateFromGenerator) Unable find room index " + i);
            }
        }

        CreateBackGrounds();

        selectionList = new SelectionList();

        SetCurrentRoom(defaultRoomData);
    }

    public void SetDefaultRoomData(int xx, int yy)
    {
        if(xx < 0 || yy < 0)
        {
            int minXY = 100000;
            RoomData rd = null;
            for(int i = 0; i < roomsList.Count; i++)
            {
                int xy = roomsList[i].GetRoomGridY() * levelWidth + roomsList[i].GetRoomGridX();
                if(xy < minXY)
                {
                    minXY = xy;
                    rd = roomsList[i];
                }
            }

            if(rd != null)
            {
                SetCurrentRoom(rd);
            }
        }
        else
        {
            SetCurrentRoom(GetRoom(xx, yy));
        }
    }

    public void Clear()
    {
        Map.Instance.ClearAll();
        levelWidth = 0;
        levelHeight = 0;
        roomsList = null;
        currentRoom = null;
    }

    public void CreateBackGrounds()
    {
        for (int i = 0; i < roomsList.Count; i++)
        {
            roomsList[i].CreateBackGroundObjectAtCenterOfLayer(SystemData.Instance.GetEditorData().backgroundObjectPrefab);
        }
    }


    public void SaveAsJson(string fileName)
    {
        JsonWriterReader.Save(fileName, this);
    }

    public bool LoadFromJson(string fileName)
    {
        if(!JsonWriterReader.Load(fileName))
        {
            return false;
        }

        Clear();

        levelWidth = JsonWriterReader.levelSerializer.levelWidth;
        levelHeight = JsonWriterReader.levelSerializer.levelHeight;

        levelMusicTrackIndex = JsonWriterReader.levelSerializer.levelMusicTrackIndex;

        int currentRoomX = JsonWriterReader.levelSerializer.currentRoomX;
        int currentRoomY = JsonWriterReader.levelSerializer.currentRoomY;

        roomsList = new List<RoomData>();

        int roomsCount = JsonWriterReader.levelSerializer.Rooms.Count;

        for (int i = 0; i < roomsCount; i++)
        {
            int nIndex = JsonWriterReader.levelSerializer.Rooms[i].roomIndex;
            int xx = nIndex % levelWidth;
            int yy = nIndex / levelWidth;

            RoomData rd = new RoomData(xx, yy);
            Map.Instance.AddRoom(xx, yy);
            rd.LoadFromJsonRoomSerializer(JsonWriterReader.levelSerializer.Rooms[i]);

            roomsList.Add(rd);
        }

        levelName = fileName;

        SetDefaultRoomData(currentRoomX, currentRoomY);

        selectionList = new SelectionList();

        return true;
    }

    public LevelInfo ConvertToLevelInfo()
    {
        LevelInfo li = new LevelInfo();

        li.levelWidth = GetLevelWidth();
        li.levelHeight = GetLevelHeight();

        int roomsCount = CountRooms();

        for (int i = 0; i < roomsCount; i++)
        {
            RoomData rd = GetRoom(i);
            RoomInfo ri = new RoomInfo();

            ri.index = i + 1;
            ri.x = rd.GetRoomGridX();
            ri.y = rd.GetRoomGridY();
            ri.w = (int)(rd.GetRoomWidth() / RoomData.defaultRoomWidth);
            ri.h = (int)(rd.GetRoomHeight() / RoomData.defaultRoomHeight);
            ri.leftEdgeDoor = rd.leftEdgeDoor;
            ri.rightEdgeDoor = rd.rightEdgeDoor;
            ri.topEdgeDoor = rd.topEdgeDoor;
            ri.bottomEdgeDoor = rd.bottomEdgeDoor;

            li.roomsList.Add(ri);
        }

        li.ConvertToSegment();

        return li;
    }

}