using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class LevelInfo
{
    public int levelWidth;
    public int levelHeight;
    public List<RoomInfo> roomsList = new List<RoomInfo>();
    public int startRoomIndex;
    public int endRoomIndex;

    public int[,] levelGrid;

    public void Clear()
    {
        levelWidth = 0;
        levelHeight = 0;
        roomsList.Clear();
        startRoomIndex = -1;
        endRoomIndex = -1;
        levelGrid = null;
    }

    public void CreateGrid()
    {
        levelGrid = new int[levelWidth, levelHeight];
        for (int i = 0; i < roomsList.Count; i++)
        {
            for (int w = 0; w < roomsList[i].w; w++)
            {
                for (int h = 0; h < roomsList[i].h; h++)
                {
                    int xx = roomsList[i].x + w;
                    int yy = roomsList[i].y + h;
                    levelGrid[xx, yy] = roomsList[i].index;
                }
            }
        }
    }

    public void CopyRoomsList(List<RoomInfo> srcList)
    {
        for (int i = 0; i < srcList.Count; i++)
        {
            RoomInfo ri = new RoomInfo();
            ri.index = srcList[i].index;
            ri.x = srcList[i].x;
            ri.y = srcList[i].y;
            ri.w = srcList[i].w;
            ri.h = srcList[i].h;
            ri.leftEdgeDoor = srcList[i].leftEdgeDoor;
            ri.topEdgeDoor = srcList[i].topEdgeDoor;
            ri.rightEdgeDoor = srcList[i].rightEdgeDoor;
            ri.bottomEdgeDoor = srcList[i].bottomEdgeDoor;

            ri.PrepareLayers();

            roomsList.Add(ri);
        }
    }

    public RoomInfo GetRoom(int x, int y)
    {
        for (int i = 0; i < roomsList.Count; i++)
        {
            if (roomsList[i].x <= x && roomsList[i].x + roomsList[i].w - 1 >= x
                && roomsList[i].y <= y && roomsList[i].y + roomsList[i].h - 1 >= y)
            {
                return roomsList[i];
            }
        }
        return null;
    }

    public RoomInfo GetStartRoom()
    {
        for (int i = 0; i < roomsList.Count; i++)
        {
            if(roomsList[i].index == startRoomIndex)
            {
                return roomsList[i];
            }
        }
        return null;
    }

    public RoomInfo GetEndRoom()
    {
        for (int i = 0; i < roomsList.Count; i++)
        {
            if (roomsList[i].index == endRoomIndex)
            {
                return roomsList[i];
            }
        }
        return null;
    }


    public int GetLevelValueAt(int x, int y) // x: 0 -> 15 * 12, y: 0 -> 9 * 12
    {
        int val = -1;

        int levelGridX = x / (int)RoomData.defaultRoomWidth;
        int levelGridY = y / (int)RoomData.defaultRoomHeight;

        RoomInfo ri = GetRoom(levelGridX, levelGridY);
        if (ri != null)
        {
            int roomGridX = x - ri.x * (int)RoomData.defaultRoomWidth;
            int roomGridY = y - ri.y * (int)RoomData.defaultRoomHeight;

            val = ri.layers[0].layerGrid[roomGridX, roomGridY] * 1000 + ri.layers[1].layerGrid[roomGridX, roomGridY] * 100;
        }

        return val;
    }

    public bool SaveFullLevelAsBitmap(string fileName)
    {
        string filepath = Path.Combine(Application.streamingAssetsPath, fileName);

        Color c = Color.red;
        Texture2D tex = LevelGenerator.CreatefillTexture2D(Color.black, levelWidth * (int)RoomData.defaultRoomWidth, levelHeight * (int)RoomData.defaultRoomHeight);

        for (int x = 0; x < (int)RoomData.defaultRoomWidth * levelWidth; x++)
        {
            for (int y = 0; y < (int)RoomData.defaultRoomHeight * levelHeight; y++)
            {
                int val = GetLevelValueAt(x, y);
                if (val < 0)
                {
                    tex.SetPixel(x, y, Color.black);
                }
                else if (val == 0) // water
                {
                    tex.SetPixel(x, y, Color.blue);
                }
                else if (val == 1000) // terrain
                {
                    tex.SetPixel(x, y, Color.red);
                }
                else if (val == 1200) // bush
                {
                    tex.SetPixel(x, y, Color.green);
                }
                else if (val == 1300) // tree
                {
                    tex.SetPixel(x, y, Color.yellow);
                }
            }
        }

        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(filepath, bytes);

        return true;
    }

    static public int CompareLevels(LevelInfo l1, LevelInfo l2)
    {
        int sameCounter = 0;
        int allCounter = 0;
        for (int x = 0; x < (int)RoomData.defaultRoomWidth * l1.levelWidth; x++)
        {
            for (int y = 0; y < (int)RoomData.defaultRoomHeight * l1.levelHeight; y++)
            {
                int val1 = l1.GetLevelValueAt(x, y);
                int val2 = l2.GetLevelValueAt(x, y);

                if (val1 >= 0 && val2 >= 0)
                {
                    allCounter++;
                    if (val1 == val2)
                    {
                        sameCounter++;
                    }
                }
            }
        }

        return 100 * sameCounter / allCounter;
    }


    // closing the level in the smallest possible segment (top-left)
    public void ConvertToSegment()
    {
        int minX = 1000;
        int minY = 1000;
        int maxX = -1000;
        int maxY = -1000;

        for (int i = 0; i < roomsList.Count; i++)
        {
            if (roomsList[i].x < minX)
            {
                minX = roomsList[i].x;
            }
            if (roomsList[i].y < minY)
            {
                minY = roomsList[i].y;
            }
            if (roomsList[i].x + roomsList[i].w > maxX)
            {
                maxX = roomsList[i].x + roomsList[i].w;
            }
            if (roomsList[i].y + roomsList[i].h > maxY)
            {
                maxY = roomsList[i].y + roomsList[i].h;
            }
        }

        levelWidth = maxX - minX;
        levelHeight = maxY - minY;

        for (int i = 0; i < roomsList.Count; i++)
        {
            roomsList[i].x -= minX;
            roomsList[i].y -= minY;
        }
    }

    public bool SaveBitmap(string fileName)
    {
        string filepath = Path.Combine(Application.streamingAssetsPath, fileName);

        Color c = Color.red;
        Texture2D tex = LevelGenerator.CreatefillTexture2D(Color.black, levelWidth, levelHeight);

        for (int i = 0; i < roomsList.Count; i++)
        {
            for (int w = 0; w < roomsList[i].w; w++)
            {
                for (int h = 0; h < roomsList[i].h; h++)
                {
                    ///tex.SetPixel(roomsList[i].x + w, roomsList[i].y + h, colors[i % colors.Length]);
                    c.r = c.g = c.b = (20 + i * 3) / 255.0f;
                    c.r = c.g = c.b = 1 - c.r;
                    tex.SetPixel(roomsList[i].x + w, roomsList[i].y + h, c);
                }
            }
        }
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(filepath, bytes);

        return true;
    }

    public bool SaveTxt(string fileName)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR

        int[,] levelGrid = new int[levelWidth, levelHeight];
        for (int i = 0; i < roomsList.Count; i++)
        {
            for (int w = 0; w < roomsList[i].w; w++)
            {
                for (int h = 0; h < roomsList[i].h; h++)
                {
                    levelGrid[roomsList[i].x + w, roomsList[i].y + h] = roomsList[i].index;
                }
            }
        }

        string str = levelWidth + "x" + levelHeight;
        str += "\n";
        for (int y = levelHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                str += levelGrid[x, y];
                if (x + 1 < levelWidth)
                {
                    str += ",";
                }
            }
            str += "\n";
        }
        str += startRoomIndex;
        str += "\n";
        str += endRoomIndex;

        string filepath = Path.Combine(Application.streamingAssetsPath, fileName);
        File.WriteAllText(filepath, str);

        return true;
#endif
        return false;
    }

    public bool SaveBin(string fileName)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR

        string filepath = Path.Combine(Application.streamingAssetsPath, fileName);
        System.IO.BinaryWriter writer = new System.IO.BinaryWriter(File.Open(filepath, FileMode.Create));
        if (writer != null)
        {
            writer.Write(levelWidth);
            writer.Write(levelHeight);
            writer.Write(roomsList.Count);
            for (int i = 0; i < roomsList.Count; i++)
            {
                writer.Write(roomsList[i].index);
                writer.Write(roomsList[i].x);
                writer.Write(roomsList[i].y);
                writer.Write(roomsList[i].w);
                writer.Write(roomsList[i].h);
                writer.Write(roomsList[i].leftEdgeDoor);
                writer.Write(roomsList[i].topEdgeDoor);
                writer.Write(roomsList[i].rightEdgeDoor);
                writer.Write(roomsList[i].bottomEdgeDoor);
            }

            writer.Write(startRoomIndex);
            writer.Write(endRoomIndex);

            writer.Close();

            return true;
        }
#endif
        return false;
    }

    public bool LoadBin(string fileName)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR

        string filepath = Path.Combine(Application.streamingAssetsPath, fileName);
        System.IO.BinaryReader reader = new System.IO.BinaryReader(File.Open(filepath, FileMode.Open));
        if (reader != null)
        {
            roomsList = new List<RoomInfo>();

            levelWidth = reader.ReadInt32();
            levelHeight = reader.ReadInt32();

            int size = reader.ReadInt32();

            for (int i = 0; i < size; i++)
            {
                RoomInfo ri = new RoomInfo();
                ri.index = reader.ReadInt32();
                ri.x = reader.ReadInt32();
                ri.y = reader.ReadInt32();
                ri.w = reader.ReadInt32();
                ri.h = reader.ReadInt32();
                ri.leftEdgeDoor = reader.ReadInt32();
                ri.topEdgeDoor = reader.ReadInt32();
                ri.rightEdgeDoor = reader.ReadInt32();
                ri.bottomEdgeDoor = reader.ReadInt32();
                roomsList.Add(ri);
            }

            startRoomIndex = reader.ReadInt32();
            endRoomIndex = reader.ReadInt32();

            reader.Close();

            return true;
        }
#endif
        return false;
    }

    public bool LoadFromJson(string fileName)
    {
        if (!JsonWriterReader.Load(fileName))
        {
            return false;
        }

        Clear();

        levelWidth = JsonWriterReader.levelSerializer.levelWidth;
        levelHeight = JsonWriterReader.levelSerializer.levelHeight;

        startRoomIndex = JsonWriterReader.levelSerializer.startRoom;
        endRoomIndex = JsonWriterReader.levelSerializer.endRoom;

        roomsList = new List<RoomInfo>();

        int roomsCount = JsonWriterReader.levelSerializer.Rooms.Count;

        for (int i = 0; i < roomsCount; i++)
        {
            RoomInfo rd = new RoomInfo();
            rd.LoadFromJsonRoomSerializer(JsonWriterReader.levelSerializer.Rooms[i]);
            roomsList.Add(rd);
        }

        return true;
    }
}
