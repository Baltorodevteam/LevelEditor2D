using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;




public class LevelGeneratorAlgorithm2
{
    // for editor...
    public static int levelWidth = 12;
    public static int levelHeight = 12;

    int[,] levelGrid;       // gridWidth x gridHeight, 0 - empty
    int[,] levelTmpGrid;    // gridWidth x gridHeight, 0 - empty

    List<RoomInfo> roomsList = new List<RoomInfo>();

    List<LevelInfo> segmentsList = new List<LevelInfo>();

    public void Init()
    {
        levelGrid = new int[levelWidth, levelHeight];
        levelTmpGrid = new int[levelWidth, levelHeight];

        for (int i = 1; i <= 10; i++)
        {
            LevelInfo li = new LevelInfo();
            li.LoadBin("levelAsSegment_" + i + ".bin");
            li.CreateGrid();
            segmentsList.Add(li);
        }

        roomsList.Clear();
    }

    void CopyGridToTmp()
    {
        for (int y = 0; y < levelHeight; y++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                levelTmpGrid[x, y] = levelGrid[x, y];
            }
        }
    }

    bool CanAddSegmentAtPlace(LevelInfo li, int x, int y)
    {
        int maxLastRoomIndex = li.roomsList.Count;
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

                    if(xx > 0 && levelGrid[xx - 1, yy] > 0 && levelGrid[xx - 1, yy] <= maxLastRoomIndex)
                    {
                        nNewNeighbours++;
                        if(nNewNeighbours > 1)
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

    bool AddSegmentAtPlace(LevelInfo li, int x, int y)
    {
        int currentIndex = 1;
        if(roomsList.Count > 0)
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
                    if(levelGrid[xx, yy] != 0)
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


    public void Run()
    {
        int r = Random.Range(0, 10);
        AddSegmentAtPlace(segmentsList[r], levelWidth / 2 - 1, levelHeight / 2 - 1);

        for(int i = 0; i < 1000; i++)
        {
            TryNewSegment();
            if(roomsList.Count > 40)
            {
                break;
            }
        }

    }

    void TryNewSegment()
    {
        int r = Random.Range(0, segmentsList.Count);

        CopyGridToTmp();
        int nrOfLeftAndTopDoors = CheckAllEdges();
        int nRoomsCount = roomsList.Count;

        int minX = 10000;
        int minY = 10000;
        int maxX = -10000;
        int maxY = -10000;
        for (int y = 0; y < levelHeight; y++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                if (levelTmpGrid[x,y] > 0)
                {
                    if(x > maxX)
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

        for(int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                if(CanAddSegmentAtPlace(segmentsList[r], x, y))
                {
                    AddSegmentAtPlace(segmentsList[r], x, y);
                }
            }
        }

    }


    int CheckEdges(int roomIndex)
    {
        int counter = 0;

        roomsList[roomIndex].leftEdgeDoor = roomsList[roomIndex].rightEdgeDoor = roomsList[roomIndex].topEdgeDoor = roomsList[roomIndex].bottomEdgeDoor = 0;

        // left
        if (roomsList[roomIndex].x > 0)
        {
            for (int h = 0; h < roomsList[roomIndex].h; h++)
            {
                if (levelTmpGrid[roomsList[roomIndex].x - 1, roomsList[roomIndex].y + h] > 0)
                {
                    roomsList[roomIndex].leftEdgeDoor |= (int)Mathf.Pow(2, h);
                    counter++;
                }
            }

        }
        // right
        if (roomsList[roomIndex].x + roomsList[roomIndex].w < levelWidth)
        {
            for (int h = 0; h < roomsList[roomIndex].h; h++)
            {
                if (levelTmpGrid[roomsList[roomIndex].x + roomsList[roomIndex].w, roomsList[roomIndex].y + h] > 0)
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
                if (levelTmpGrid[roomsList[roomIndex].x + w, roomsList[roomIndex].y - 1] > 0)
                {
                    roomsList[roomIndex].topEdgeDoor |= (int)Mathf.Pow(2, w);
                    counter++;
                }
            }
        }
        // bottom
        if (roomsList[roomIndex].y + roomsList[roomIndex].h < levelHeight)
        {
            for (int w = 0; w < roomsList[roomIndex].w; w++)
            {
                if (levelTmpGrid[roomsList[roomIndex].x + w, roomsList[roomIndex].y + roomsList[roomIndex].h] > 0)
                {
                    roomsList[roomIndex].bottomEdgeDoor |= (int)Mathf.Pow(2, w);
                }
            }
        }
        
        return counter;
    }

    int CheckAllEdges()
    {
        int counter = 0;
        for (int i = 0; i < roomsList.Count; i++)
        {
            counter += CheckEdges(i);
        }
        return counter;
    }

}