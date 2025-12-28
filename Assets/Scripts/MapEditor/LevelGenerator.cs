using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;



public class LevelGenerator
{
    // for editor...
    public static int levelWidth = 12;
    public static int levelHeight = 12;

    static LevelGeneratorParams generatorParams = new LevelGeneratorParams();

    static int[,] levelGrid; // gridWidth x gridHeight, 0 - empty

    static List<RoomInfo> roomsList = new List<RoomInfo>();

    // Algorithm1
    static List<int> roomsQueue = new List<int>();

    // Algorithm2
    static List<LevelInfo> segmentsList = new List<LevelInfo>();


    static List<RoomInfo> endRooms = new List<RoomInfo>();

    static int startRoomIndex = -1;
    static int endRoomIndex = -1;


    static public LevelGeneratorParams GetParams()
    {
        return generatorParams;
    }

    static public LevelInfo ToLevelInfo()
    {
        LevelInfo li = new LevelInfo();

        li.Clear();
        li.levelWidth = generatorParams.levelWidth;
        li.levelHeight = generatorParams.levelHeight;
        li.startRoomIndex = startRoomIndex;
        li.endRoomIndex = endRoomIndex;
        li.CopyRoomsList(roomsList);

        float addDiff = (generatorParams.endDiff - generatorParams.startDiff) / roomsList.Count;

        for (int i = 0; i < li.roomsList.Count; i++)
        {
            float diff = generatorParams.startDiff + (float)i * addDiff;
            li.roomsList[i].difficulty = diff;
            AreaGenerator ag = new AreaGenerator();
            ag.Generate(li, li.roomsList[i]);
        }

        return li;
    }


    public static int GetLastRoomIndex()
    {
        return roomsList.Count;
    }

    public static int GetStartRoomIndex()
    {
        return startRoomIndex;
    }

    public static int GetEndRoomIndex()
    {
        return endRoomIndex;
    }

    static void CheckEdges(int roomIndex)
    {
        // left
        if(roomsList[roomIndex].x > 0)
        {
            for (int h = 0; h < roomsList[roomIndex].h; h++)
            {
                if(levelGrid[roomsList[roomIndex].x - 1, roomsList[roomIndex].y + h] > 0)
                {
                    roomsList[roomIndex].leftEdgeDoor |= (int)Mathf.Pow(2, h);
                }
            }

        }
        // right
        if (roomsList[roomIndex].x + roomsList[roomIndex].w < generatorParams.levelWidth)
        {
            for (int h = 0; h < roomsList[roomIndex].h; h++)
            {
                if (levelGrid[roomsList[roomIndex].x + roomsList[roomIndex].w, roomsList[roomIndex].y + h] > 0)
                {
                    roomsList[roomIndex].rightEdgeDoor |= (int)Mathf.Pow(2, h);
                }
            }
        }
        // top
        if (roomsList[roomIndex].y > 0)
        {
            for (int w = 0; w < roomsList[roomIndex].w; w++)
            {
                if (levelGrid[roomsList[roomIndex].x + w, roomsList[roomIndex].y - 1] > 0)
                {
                    roomsList[roomIndex].topEdgeDoor |= (int)Mathf.Pow(2, w);
                }
            }
        }
        // bottom
        if (roomsList[roomIndex].y + roomsList[roomIndex].h < generatorParams.levelHeight)
        {
            for (int w = 0; w < roomsList[roomIndex].w; w++)
            {
                if (levelGrid[roomsList[roomIndex].x + w, roomsList[roomIndex].y + roomsList[roomIndex].h] > 0)
                {
                    roomsList[roomIndex].bottomEdgeDoor |= (int)Mathf.Pow(2, w);
                }
            }
        }
    }

    static void CheckAllEdges()
    {
        for(int i = 0; i < roomsList.Count; i++)
        {
            CheckEdges(i);
        }
    }

    static void Clear()
    {
        levelGrid = new int[generatorParams.levelWidth, generatorParams.levelHeight];

        for (int x = 0; x < generatorParams.levelWidth; x++)
        {
            for (int y = 0; y < generatorParams.levelHeight; y++)
            {
                levelGrid[x, y] = 0;
            }
        }

        endRooms.Clear();
        roomsList.Clear();

        startRoomIndex = -1;
        endRoomIndex = -1;

        // Algorithm1
        roomsQueue.Clear();

        // Algorithm2
        segmentsList.Clear();
    }

    static void AddRoom(int index, int x, int y, int w, int h)
    {
        RoomInfo ri = new RoomInfo();
        ri.index = index;
        ri.x = x;
        ri.y = y;
        ri.w = w;
        ri.h = h;
        ri.PrepareLayers();
        roomsList.Add(ri);
    }

    static public bool SaveTxt(string fileName)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        string str = generatorParams.levelWidth + "x" + generatorParams.levelHeight;
        str += "\n";
        for(int y = generatorParams.levelHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < generatorParams.levelWidth; x++)
            {
                str += levelGrid[x, y];
                if(x + 1 < generatorParams.levelWidth)
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
    }

    static public bool SaveBin(string fileName)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR

        string filepath = Path.Combine(Application.streamingAssetsPath, fileName);
        System.IO.BinaryWriter writer = new System.IO.BinaryWriter(File.Open(filepath, FileMode.Create));
        if (writer != null)
        {
            writer.Write(generatorParams.levelWidth);
            writer.Write(generatorParams.levelHeight);
            for (int y = 0; y < generatorParams.levelHeight; y++)
            {
                for (int x = 0; x < generatorParams.levelWidth; x++)
                {
                    writer.Write(levelGrid[x, y]);
                }
            }

            writer.Write(startRoomIndex);
            writer.Write(endRoomIndex);

            writer.Close();

            return true;
        }
#endif
        return false;
    }

    static public bool LoadBin(string fileName)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR

        string filepath = Path.Combine(Application.streamingAssetsPath, fileName);
        System.IO.BinaryReader reader = new System.IO.BinaryReader(File.Open(filepath, FileMode.Open));
        if (reader != null)
        {
            generatorParams.levelWidth = reader.ReadInt32();
            generatorParams.levelHeight = reader.ReadInt32();

            levelGrid = new int[generatorParams.levelWidth, generatorParams.levelHeight];
            for (int y = 0; y < generatorParams.levelHeight; y++)
            {
                for (int x = 0; x < generatorParams.levelWidth; x++)
                {
                    levelGrid[x, y] = reader.ReadInt32();
                }
            }

            startRoomIndex = reader.ReadInt32();
            endRoomIndex = reader.ReadInt32();

            reader.Close();

            return true;
        }
#endif
        return false;
    }

    static public bool SaveBitmap(string fileName)
    {
        string filepath = Path.Combine(Application.persistentDataPath, fileName);

        Color c = Color.red;

        ///Color[] colors = { Color.blue, Color.green, Color.red, Color.white, Color.yellow, Color.grey };

        Texture2D tex = CreatefillTexture2D(Color.black, generatorParams.levelWidth, generatorParams.levelHeight);

        for(int i = 0; i < roomsList.Count; i++)
        {
            for(int w = 0; w < roomsList[i].w; w++)
            {
                for (int h = 0; h < roomsList[i].h; h++)
                {
                    ///tex.SetPixel(roomsList[i].x + w, roomsList[i].y + h, colors[i % colors.Length]);
                    c.r = c.g = c.b = (50 + i * 3) / 255.0f;
                    tex.SetPixel(roomsList[i].x + w, roomsList[i].y + h, c);
                }
            }
        }
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(filepath, bytes);

        return true;
    }

    static public Texture2D CreatefillTexture2D(Color color, int textureWidth, int textureHeight)
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false, true);
        texture.filterMode = FilterMode.Point;
        texture.anisoLevel = 0;
        //texture.alphaIsTransparency = true;
        texture.wrapMode = TextureWrapMode.Clamp;

        Color c = new Color();
        c = color;
        c.a = 1.0f;

        int numOfPixels = textureWidth * textureHeight;
        Color[] colors = new Color[numOfPixels];
        for (int x = 0; x < numOfPixels; x++)
        {
            colors[x] = c;
        }

        texture.SetPixels(colors);

        return texture;
    }


    static int CountNeighbours(int x, int y)
    {
        int ret = 0;

        if (x > 0)
            ret += (levelGrid[x - 1, y] > 0 ? 1 : 0);
        if (y > 0)
            ret += (levelGrid[x, y - 1] > 0 ? 1 : 0);
        if (x < generatorParams.levelWidth - 1)
            ret += (levelGrid[x + 1, y] > 0 ? 1 : 0);
        if (y < generatorParams.levelHeight - 1)
            ret += (levelGrid[x, y + 1] > 0 ? 1 : 0);

        return ret;
    }

    static int CountNeighbours(int[,] level, int currentRoomIndex, int x, int y, int w, int h)
    {
        int ret = 0;

        int startX = x - w - 1;
        if (startX < 0) startX = 0;
        int stopX = x + w + 1;
        if (stopX > generatorParams.levelWidth) stopX = generatorParams.levelWidth;

        int startY = y - h - 1;
        if (startY < 0) startY = 0;
        int stopY = y + h + 1;
        if (stopY > generatorParams.levelHeight) stopY = generatorParams.levelHeight;

        for (int xx = startX; xx < stopX; xx++)
        {
            for (int yy = startY; yy < stopY; yy++)
            {
                if(level[xx,yy] == currentRoomIndex)
                {
                    if (xx > 0)
                        ret += ((level[xx - 1, yy] > 0 && level[xx - 1, yy] != currentRoomIndex) ? 1 : 0);
                    if (yy > 0)
                        ret += ((level[xx, yy - 1] > 0 && level[xx, yy - 1] != currentRoomIndex) ? 1 : 0);
                    if (xx < generatorParams.levelWidth - 1)
                        ret += ((level[xx + 1, yy] > 0 && level[xx + 1, yy] != currentRoomIndex) ? 1 : 0);
                    if (yy < generatorParams.levelHeight - 1)
                        ret += ((level[xx, yy + 1] > 0 && level[xx, yy + 1] != currentRoomIndex) ? 1 : 0);
                }
            }
        }
        return ret;
    }

    static public int CountRooms()
    {
        return roomsList.Count;
    }

    static void PrepareSizePercentTab()
    {
        generatorParams.roomSizePercentTab = new int[generatorParams.maxRoomSize + 1];
        for(int i = 0; i <= generatorParams.minRoomSize; i++)
        {
            generatorParams.roomSizePercentTab[i] = 0;
        }

        float diff = generatorParams.maxRoomSize - generatorParams.minRoomSize;

        if (diff > 0)
        {
            diff = 70.0f / diff;
        }


        float counter = 0;
        for (int i = generatorParams.minRoomSize; i <= generatorParams.maxRoomSize; i++)
        {
            generatorParams.roomSizePercentTab[i] = 100 - (int)(diff * counter);
            counter++;
        }
    }

    static int GetRandomPercentTab(int[] _roomSizePercentTab)
    {
        int nMax = 0;
        for(int i = 0; i < _roomSizePercentTab.Length; i++)
        {
            nMax += _roomSizePercentTab[i];
        }

        int[] nrTab = new int[nMax];

        int nCurrentIndex = 0;
        for (int i = 0; i < _roomSizePercentTab.Length; i++)
        {
            for(int j = 0; j < _roomSizePercentTab[i]; j++)
            {
                nrTab[nCurrentIndex] = i;
                nCurrentIndex++;
            }
        }

        int nR = Random.Range(0, nMax);
        return nrTab[nR];
    }

    // Algorithm1
    static bool Visit(int x, int y, bool rememberOnlyLastRoom = false)
    {
        //if (CountRooms() >= generatorParams.maxRooms)
        //    return false;

        //if (Random.Range(0, 1.0f) < 0.35f && !(x == generatorParams.levelWidth / 2 && y == generatorParams.levelHeight / 2)) // 0.35 - parametr!
        //    return false;
        if (Random.Range(0, 1.0f) < 0.35f && roomsList.Count > 0) // 0.35 - parametr!
            return false;

        //if (Random.Range(0, 1.0f) < 0.75f && roomsList.Count > 0) // 0.35 - parametr!
        //    return false;

        int roomW = 1;
        int roomH = 1;

        int rand = GetRandomPercentTab(generatorParams.roomSizePercentTab);
        rand += 1;

        roomW = rand;
        roomH = Random.Range(1, roomW + 1);

        if (Random.Range(0, 100) > 50)
        {
            int t = roomW;
            roomW = roomH;
            roomH = t;
        }

        if (roomW == 1 && roomH == 1)
        {
            if (levelGrid[x, y] > 0)
                return false;

            int neighbours = CountNeighbours(x, y);

            if (neighbours > 1) // 1 - parametr!
                return false;

            if(rememberOnlyLastRoom)
            {
                roomsQueue.Clear();
            }
            roomsQueue.Add(y * generatorParams.levelWidth + x);

            int lastRoomIndex = GetLastRoomIndex() + 1;
            levelGrid[x, y] = lastRoomIndex;
            AddRoom(lastRoomIndex, x, y, 1, 1);

            return true;
        }
        else
        {
            int dir = -1;
            for(int j = 0; j < 4; j++)
            {
                if(IsGoodPlaceForRoom(x, y, roomW, roomH, j))
                {
                    dir = j;

                    int lastRoomIndex = GetLastRoomIndex();

                    int startX = x - roomW - 1;
                    if (startX < 0) startX = 0;
                    int stopX = x + roomW + 1;
                    if (stopX > generatorParams.levelWidth) stopX = generatorParams.levelWidth;
                    
                    int startY = y - roomH - 1;
                    if (startY < 0) startY = 0;
                    int stopY = y + roomH + 1;
                    if (stopY > generatorParams.levelHeight) stopY = generatorParams.levelHeight;

                    if (rememberOnlyLastRoom)
                    {
                        roomsQueue.Clear();
                    }

                    for (int xx = startX; xx < stopX; xx++)
                    {
                        for (int yy = startY; yy < stopY; yy++)
                        {
                            if(levelGrid[xx, yy] == lastRoomIndex)
                            {
                                roomsQueue.Add(yy * generatorParams.levelWidth + xx);
                            }
                        }
                    }

                    return true;
                }
            }
        }

        return false;
    }

    // Algorithm1
    static void Loop()
    {
        if (roomsQueue.Count > 0)
        {
            int index = roomsQueue[0];
            roomsQueue.RemoveAt(0);

            int x = index % generatorParams.levelWidth;
            int y = index / generatorParams.levelWidth;

            bool created = false;
            if (x > 0) created = created | Visit(x - 1, y);
            if (y > 0) created = created | Visit(x, y - 1);
            if (x < generatorParams.levelWidth - 1) created = created | Visit(x + 1, y);
            if (y < generatorParams.levelHeight - 1) created = created | Visit(x, y + 1);

            if(!created)
            {
                roomsQueue.Add(index);
            }
        }
    }

    // Algorithm4
    static void Loop(int destX, int destY)
    {
        if (roomsList.Count > 0 && roomsQueue.Count > 0)
        {
            int index = roomsQueue[0];
            roomsQueue.RemoveAt(0);

            int x = index % generatorParams.levelWidth;
            int y = index / generatorParams.levelWidth;

            //int x = roomsList[roomsList.Count - 1].x;
            //int y = roomsList[roomsList.Count - 1].y;

            bool created = false;
            if (x > 0 && destX < x) created = created | Visit(x - 1, y, true);
            if (!created && y > 0 && destY < y) created = created | Visit(x, y - 1, true);
            if (!created && x < generatorParams.levelWidth - 1 && destX > x) created = created | Visit(x + 1, y, true);
            if (!created && y < generatorParams.levelHeight - 1 && destY > y) created = created | Visit(x, y + 1, true);

            /*
            if (!created && x > 0) created = created | Visit(x - 1, y);
            if (!created && y > 0) created = created | Visit(x, y - 1);
            if (!created && x < generatorParams.levelWidth - 1) created = created | Visit(x + 1, y);
            if (!created && y < generatorParams.levelHeight - 1) created = created | Visit(x, y + 1);
            */

            if (!created)
            {
                roomsQueue.Add(index);
            }
        }
    }

    public static float Generate(LevelGeneratorParams lgp)
    {
        float startT = Time.realtimeSinceStartup;

        generatorParams.levelWidth = lgp.levelWidth;
        generatorParams.levelHeight = lgp.levelHeight;

        generatorParams.minRooms = lgp.minRooms;
        generatorParams.maxRooms = lgp.maxRooms;

        generatorParams.minRoomSize = lgp.minRoomSize;
        generatorParams.maxRoomSize = lgp.maxRoomSize;

        generatorParams.algorithmNr = lgp.algorithmNr;

        if (generatorParams.minRoomSize > generatorParams.levelWidth / 3)
        {
            generatorParams.minRoomSize = generatorParams.levelWidth / 3;
        }
        if (generatorParams.maxRoomSize > 1 + generatorParams.levelWidth / 3)
        {
            generatorParams.maxRoomSize = 1 + generatorParams.levelWidth / 3;
        }

        if(lgp.roomSizePercentTab != null)
        {
            generatorParams.roomSizePercentTab = lgp.roomSizePercentTab;
        }
        else
        {
            PrepareSizePercentTab();
        }

        tmpLevelGrid = new int[generatorParams.levelWidth, generatorParams.levelHeight];

        Clear();

        if (generatorParams.algorithmNr == 1)
        {
            Algorithm1();
        }
        else if (generatorParams.algorithmNr == 2)
        {
            Algorithm1();
        }
        else if (generatorParams.algorithmNr == 3)
        {
            Algorithm2();
        }

        CheckAllEdges();
        GetEndRooms();

        return Time.realtimeSinceStartup - startT;
    }

    static void Algorithm1()
    {
        Visit(generatorParams.levelWidth / 2, generatorParams.levelHeight / 2);

        for (int loopCounter = 0; loopCounter < 1000; loopCounter++)
        {
            Loop();
            if (CountRooms() >= generatorParams.minRooms)
            {
                break;
            }
        }
    }

    static void Algorithm2()
    {
        for (int i = 1; i <= 10; i++)
        {
            LevelInfo li = new LevelInfo();
            li.LoadBin("levelAsSegment_" + i + ".bin");
            li.CreateGrid();
            segmentsList.Add(li);
        }

        int r = Random.Range(0, 10);
        AddSegmentAtPlace(segmentsList[r], levelWidth / 2 - 1, levelHeight / 2 - 1);

        for (int i = 0; i < 1000; i++)
        {
            TryNewSegment();
            if (roomsList.Count > 20)
            {
                break;
            }
        }
    }

    static void AddNaviPoint(ref List<Vector2> list)
    {
        for (; ; )
        {
            int r2x = 2 + Random.Range(0, generatorParams.levelWidth - 4);
            int r2y = 2 + Random.Range(0, generatorParams.levelHeight - 4);

            bool bOK = true;

            for(int i = 0; i < list.Count; i++)
            {
                if ((list[i].x - r2x) * (list[i].x - r2x) + (list[i].y - r2y) * (list[i].y - r2y) < 9)
                {
                    bOK = false;
                    break;
                }
            }

            if(bOK)
            {
                list.Add(new Vector2(r2x, r2y));
                return;
            }
        }
    }

    static void Algorithm4()
    {
        List<Vector2> naviPoints = new List<Vector2>();

        int r1x = 2 + Random.Range(0, generatorParams.levelWidth - 4);
        int r1y = 2 + Random.Range(0, generatorParams.levelHeight - 4);

        naviPoints.Add(new Vector2(r1x, r1y));
        AddNaviPoint(ref naviPoints);
        AddNaviPoint(ref naviPoints);
        AddNaviPoint(ref naviPoints);

        Navigator2D nav = new Navigator2D(generatorParams.levelWidth, generatorParams.levelHeight);

        for(int i = 0; i < naviPoints.Count - 1; i++)
        {
            nav.SetCollision((int)naviPoints[i].x, (int)naviPoints[i].y, false);

            List<Vector2> list = nav.GetPath(naviPoints[i], naviPoints[i+1]);
            if(list != null)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    int lastRoomIndex = GetLastRoomIndex() + 1;
                    levelGrid[(int)list[j].x, (int)list[j].y] = lastRoomIndex;
                    AddRoom(lastRoomIndex, (int)list[j].x, (int)list[j].y, 1, 1);
                }

                nav.Bake();
                for (int j = 0; j < roomsList.Count - 1; j++)
                {
                    nav.SetCollisionN((int)roomsList[j].x, (int)roomsList[j].y);
                }
            }
            else
            {
                Debug.Log("Algorithm4 ERROR!!!");
                Clear();
                return;
            }
        }

        // pozostale pomieszczenia....
        roomsQueue.Clear();
        for (int i = 0; i < roomsList.Count; i++)
        {
            roomsQueue.Add(roomsList[i].y * generatorParams.levelWidth + roomsList[i].x);
        }

        int nrOfRooms = roomsList.Count;

        int addRooms = generatorParams.minRooms - nrOfRooms;
        if(addRooms < 0)
        {
            addRooms = nrOfRooms / 3;
        }

        for (int loopCounter = 0; loopCounter < 1000; loopCounter++)
        {
            Loop();
            if(roomsList.Count - nrOfRooms > addRooms)
            {
                break;
            }
        }
    }

    // Algorithm2
    static void TryNewSegment()
    {
        int r = Random.Range(0, segmentsList.Count);

        int minX = 10000;
        int minY = 10000;
        int maxX = -10000;
        int maxY = -10000;
        for (int y = 0; y < levelHeight; y++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                if (levelGrid[x, y] > 0)
                {
                    if (x > maxX)
                    {
                        maxX = x;
                    }
                    if (x < minX)
                    {
                        minX = x;
                    }
                    if (y > maxY)
                    {
                        maxY = y;
                    }
                    if (y < minY)
                    {
                        minY = y;
                    }
                }
            }
        }
        minX = Mathf.Max(minX - 3, 0);
        minY = Mathf.Max(minY - 3, 0);
        maxX = Mathf.Min(maxX, levelWidth - 4);
        maxY = Mathf.Min(maxY, levelHeight - 4);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                if (CanAddSegmentAtPlace(segmentsList[r], x, y))
                {
                    AddSegmentAtPlace(segmentsList[r], x, y);
                    return;
                }
            }
        }

    }

    // Algorithm2
    static bool CanAddSegmentAtPlace(LevelInfo li, int x, int y)
    {
        int maxLastRoomIndex = roomsList[roomsList.Count - 1].index;
        int nNewNeighbours = 0;

        for (int i = 0; i < li.roomsList.Count; i++)
        {
            for (int w = 0; w < li.roomsList[i].w; w++)
            {
                for (int h = 0; h < li.roomsList[i].h; h++)
                {
                    int xx = x + li.roomsList[i].x + w;
                    int yy = y + li.roomsList[i].y + h;
                    if (levelGrid[xx, yy] != 0)
                    {
                        return false;
                    }

                    if (xx > 0 && levelGrid[xx - 1, yy] > 0 && levelGrid[xx - 1, yy] <= maxLastRoomIndex)
                    {
                        nNewNeighbours++;
                        if (nNewNeighbours > 1)
                        {
                            return false;
                        }
                    }

                    if (yy > 0 && levelGrid[xx, yy - 1] > 0 && levelGrid[xx, yy - 1] <= maxLastRoomIndex)
                    {
                        nNewNeighbours++;
                        if (nNewNeighbours > 1)
                        {
                            return false;
                        }
                    }

                    if (xx < levelWidth - 1 && levelGrid[xx + 1, yy] > 0 && levelGrid[xx + 1, yy] <= maxLastRoomIndex)
                    {
                        nNewNeighbours++;
                        if (nNewNeighbours > 1)
                        {
                            return false;
                        }
                    }

                    if (yy < levelHeight - 1 && levelGrid[xx, yy + 1] > 0 && levelGrid[xx, yy + 1] <= maxLastRoomIndex)
                    {
                        nNewNeighbours++;
                        if (nNewNeighbours > 1)
                        {
                            return false;
                        }
                    }

                }
            }
        }

        return nNewNeighbours == 1;
    }

    // Algorithm2
    static bool AddSegmentAtPlace(LevelInfo li, int x, int y)
    {
        int currentIndex = 1;
        if (roomsList.Count > 0)
        {
            currentIndex = roomsList[roomsList.Count - 1].index + 1;
        }

        for (int i = 0; i < li.roomsList.Count; i++)
        {

            for (int w = 0; w < li.roomsList[i].w; w++)
            {
                for (int h = 0; h < li.roomsList[i].h; h++)
                {
                    int xx = x + li.roomsList[i].x + w;
                    int yy = y + li.roomsList[i].y + h;
                    if (levelGrid[xx, yy] != 0)
                    {
                        return false;
                    }
                    levelGrid[xx, yy] = currentIndex;
                }
            }

            RoomInfo ri = new RoomInfo();
            ri.x = x + li.roomsList[i].x;
            ri.y = y + li.roomsList[i].y;
            ri.w = li.roomsList[i].w;
            ri.h = li.roomsList[i].h;
            ri.index = currentIndex;
            roomsList.Add(ri);

            currentIndex++;
        }

        return true;
    }


    static void GetEndRooms()
    {
        int nS = roomsList.Count;
 
        int startPositionIndex = -1;
        int startPositionMinDistance = 1000000;

        int stopPositionIndex = -1;
        int stopPositionMinDistance = 1000000;


        for (int i = 0; i < nS; i++)
        {
            int neighbours = CountNeighbours(levelGrid, roomsList[i].index, roomsList[i].x, roomsList[i].y, roomsList[i].w, roomsList[i].h);
            if(neighbours == 1)
            {
                endRooms.Add(roomsList[i]);

                //int startMinDist = roomsList[i].x + roomsList[i].y;
                //int stopMinDist = (generatorParams.levelWidth - (roomsList[i].x + roomsList[i].w)) + (generatorParams.levelHeight - (roomsList[i].y + roomsList[i].h));
              
                int startMinDist = roomsList[i].x * roomsList[i].x + roomsList[i].y * roomsList[i].y;
                int stopMinDist = (generatorParams.levelWidth - (roomsList[i].x + roomsList[i].w)) * (generatorParams.levelWidth - (roomsList[i].x + roomsList[i].w)) 
                    + (generatorParams.levelHeight - (roomsList[i].y + roomsList[i].h)) * (generatorParams.levelHeight - (roomsList[i].y + roomsList[i].h));

                if (startMinDist < startPositionMinDistance)
                {
                    startPositionMinDistance = startMinDist;
                    startPositionIndex = roomsList[i].index;
                }

                if (stopMinDist < stopPositionMinDistance)
                {
                    stopPositionMinDistance = stopMinDist;
                    stopPositionIndex = roomsList[i].index;
                }
            }
        }

        startRoomIndex = startPositionIndex;
        endRoomIndex = stopPositionIndex;
    }


    static int[,] tmpLevelGrid = new int[generatorParams.levelWidth, generatorParams.levelHeight];

    static bool IsGoodPlaceForRoom(int x, int y, int w, int h, int dir) // dir - [0,3]
    {
        int startX = x - w - 2;
        if (startX < 0) startX = 0;
        int stopX = x + w + 2;
        if (stopX > generatorParams.levelWidth) stopX = generatorParams.levelWidth;

        int startY = y - h - 2;
        if (startY < 0) startY = 0;
        int stopY = y + h + 2;
        if (stopY > generatorParams.levelHeight) stopY = generatorParams.levelHeight;


        for (int k = startX; k < stopX; k++)
        {
            for (int j = startY; j < stopY; j++)
            {
                tmpLevelGrid[k, j] = levelGrid[k, j];
            }
        }


        bool fromLeft = true;
        bool fromTop = true;

        if(dir == 1)
        {
            fromTop = false;
            fromLeft = true;
        }
        else if(dir == 2)
        {
            fromTop = true;
            fromLeft = false;
        }
        else if (dir == 3)
        {
            fromTop = false;
            fromLeft = false;
        }

        int currentRoomIndex = GetLastRoomIndex() + 1;

        if (fromTop && fromLeft)
        {
            for(int xx = 0; xx < w; xx++)
            {
                for (int yy = 0; yy < h; yy++)
                {
                    int xxx = x + xx;
                    int yyy = y + yy;

                    if (xxx < 0 || xxx >= generatorParams.levelWidth || yyy < 0 || yyy >= generatorParams.levelHeight)
                    {
                        return false;
                    }

                    if(tmpLevelGrid[xxx, yyy] > 0)
                    {
                        return false;
                    }
                    else
                    {
                        tmpLevelGrid[xxx, yyy] = currentRoomIndex;
                    }
                }
            }

            AddRoom(currentRoomIndex, x, y, w, h);
        }
        else if (!fromTop && fromLeft)
        {
            for (int xx = 0; xx < w; xx++)
            {
                for (int yy = 0; yy < h; yy++)
                {
                    int xxx = x + xx;
                    int yyy = y - yy;

                    if (xxx < 0 || xxx >= generatorParams.levelWidth || yyy < 0 || yyy >= generatorParams.levelHeight)
                    {
                        return false;
                    }

                    if (tmpLevelGrid[xxx, yyy] > 0)
                    {
                        return false;
                    }
                    else
                    {
                        tmpLevelGrid[xxx, yyy] = currentRoomIndex;
                    }
                }
            }
            AddRoom(currentRoomIndex, x, y - h + 1, w, h);
        }
        else if (fromTop && !fromLeft)
        {
            for (int xx = 0; xx < w; xx++)
            {
                for (int yy = 0; yy < h; yy++)
                {
                    int xxx = x - xx;
                    int yyy = y + yy;

                    if (xxx < 0 || xxx >= generatorParams.levelWidth || yyy < 0 || yyy >= generatorParams.levelHeight)
                    {
                        return false;
                    }

                    if (tmpLevelGrid[xxx, yyy] > 0)
                    {
                        return false;
                    }
                    else
                    {
                        tmpLevelGrid[xxx, yyy] = currentRoomIndex;
                    }
                }
            }
            AddRoom(currentRoomIndex, x - w + 1, y, w, h);
        }
        else if (!fromTop && !fromLeft)
        {
            for (int xx = 0; xx < w; xx++)
            {
                for (int yy = 0; yy < h; yy++)
                {
                    int xxx = x - xx;
                    int yyy = y - yy;

                    if (xxx < 0 || xxx >= generatorParams.levelWidth || yyy < 0 || yyy >= generatorParams.levelHeight)
                    {
                        return false;
                    }

                    if (tmpLevelGrid[xxx, yyy] > 0)
                    {
                        return false;
                    }
                    else
                    {
                        tmpLevelGrid[xxx, yyy] = currentRoomIndex;
                    }
                }
            }
            AddRoom(currentRoomIndex, x - w + 1, y - h + 1, w, h);
        }

        int neighbours = CountNeighbours(tmpLevelGrid, currentRoomIndex, x, y, w, h);

        if(neighbours > 1)
        {
            roomsList.RemoveAt(roomsList.Count - 1);
            return false;
        }

        for (int k = startX; k < stopX; k++)
        {
            for (int j = startY; j < stopY; j++)
            {
                levelGrid[k, j] = tmpLevelGrid[k, j];
            }
        }

        return true;
    }

    public static int GetGridXY(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < generatorParams.levelWidth && y < generatorParams.levelHeight)
        {
            return levelGrid[x, y];
        }
        return 0;
    }

    public static int GetGridWidth()
    {
        return generatorParams.levelWidth;
    }

    public static int GetGridHeight()
    {
        return generatorParams.levelHeight;
    }

    static public RoomInfo FindRoomByIndex(int roomIndex, out int left, out int top, out int w, out int h)
    {
        for(int i = 0; i < roomsList.Count; i++)
        {
            if(roomsList[i].index == roomIndex)
            {
                left = roomsList[i].x;
                top = roomsList[i].y;
                w = roomsList[i].w;
                h = roomsList[i].h;
                return roomsList[i];
            }
        }

        left = -1;
        top = -1;
        w = -1;
        h = -1;

        return null;
    }

}