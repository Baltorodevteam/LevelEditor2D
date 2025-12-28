 using UnityEngine;
using UnityEngine.UI; 


public class LevelMiniMap : MonoBehaviour
{
    public const int eLevelWidth = 12;
    public const int eLevelHeight = 12;

    public const int ePiece_LRTB   = 0;
    public const int ePiece_LRT    = 1;
    public const int ePiece_LRB    = 2;
    public const int ePiece_LTB    = 3;
    public const int ePiece_RTB    = 4;
    public const int ePiece_LT     = 5;
    public const int ePiece_LB     = 6;
    public const int ePiece_RT     = 7;
    public const int ePiece_RB     = 8;
    public const int ePiece_T      = 9;
    public const int ePiece_B      = 10;
    public const int ePiece_L      = 11;
    public const int ePiece_R      = 12;
    public const int ePiece_M      = 13;
    public const int ePiece_LR     = 14;
    public const int ePiece_TB     = 15;


    [SerializeField]
    Texture2D miniMapTexture;

    [SerializeField]
    Texture2D[] roomPieceTextures;
    [SerializeField]
    Texture2D[] selectedRoomPieceTextures;
    [SerializeField]
    Texture2D[] correctRoomPieceTextures;
    [SerializeField]
    Texture2D texCursor;
    [SerializeField]
    Texture2D startRoomTexture;
    [SerializeField]
    Texture2D endRoomTexture;
    [SerializeField]
    Texture2D doorTexture;

    int cursorX = 0;
    int cursorY = 0;

    public delegate void DownEventExtAction(int x, int y);
    public delegate void UpEventExtAction(int x, int y);
    public delegate void OnClickEventExtAction();

    DownEventExtAction downEventAction;
    UpEventExtAction upEventAction;
    OnClickEventExtAction onClickEventAction;


    public void SetDownEventExtAction(DownEventExtAction f)
    {
        downEventAction = f;
    }

    public void SetUpEventExtAction(UpEventExtAction f)
    {
        upEventAction = f;
    }

    public void SetOnClickEventExtAction(OnClickEventExtAction f)
    {
        onClickEventAction = f;
    }

    public void SetCursorAtCurremtRoom(LevelData ld)
    {
        cursorX = ld.GetCurrentRoom().GetRoomGridX();
        cursorY = ld.GetCurrentRoom().GetRoomGridY();
    }

    public int GetCursorX()
    {
        return cursorX;
    }

    public int GetCursorY()
    {
        return cursorY;
    }

    public void SetCursorXY(int x, int y)
    {
        cursorX = x;
        cursorY = y;
    }

    public void MoveCursor(int dx, int dy)
    {
        cursorX += dx;
        if(cursorX < 0)
        {
            cursorX = 0;
        }
        else if(cursorX >= eLevelWidth)
        {
            cursorX = eLevelWidth - 1;
        }

        cursorY += dy;
        if (cursorY < 0)
        {
            cursorY = 0;
        }
        else if (cursorY >= eLevelHeight)
        {
            cursorY = eLevelHeight - 1;
        }
    }

    public RoomData GetRoomAtCursor(LevelData ld)
    {
        return GetRoomAtPosition(ld, cursorX, cursorY);
    }

    public RoomData GetRoomAtPosition(LevelData ld, int x, int y)
    {
        for(int i = 0; i < ld.CountRooms(); i++)
        {
            RoomData rd = ld.GetRoom(i);
            if(x >= rd.GetRoomGridX() && y >= rd.GetRoomGridY())
            {
                int w = (int)(rd.GetRoomWidth() / RoomData.defaultRoomWidth);
                int h = (int)(rd.GetRoomHeight() / RoomData.defaultRoomHeight);
                if(x <= rd.GetRoomGridX() + w - 1 && y <= rd.GetRoomGridY() + h - 1)
                {
                    return rd;
                }
            }
        }
        return null;
    }

    public void OnDown()
    {
        if (downEventAction != null)
        {
            downEventAction(cursorX, cursorY);
        }
        else
        {

        }
        Debug.Log("MiniMap DownEvent X = " + cursorX + "  Y = " + cursorY);
    }

    public void OnUp()
    {
        if(upEventAction != null)
        {
            upEventAction(cursorX, cursorY);
        }
        else
        {
            OnClick();
        }
        Debug.Log("MiniMap UpEvent X = " + cursorX + "  Y = " + cursorY);
    }


    public void OnClick()
    {
        RectTransform rt = GetComponent<RectTransform>();

        float mx = Input.mousePosition.x - rt.position.x;
        mx += (rt.lossyScale.x * rt.rect.width / 2);

        float my = rt.position.y - Input.mousePosition.y;
        my += (rt.lossyScale.y * rt.rect.height / 2);
        my = (rt.lossyScale.y * rt.rect.height) - my;

        mx /= (rt.lossyScale.x * rt.rect.width);
        my /= (rt.lossyScale.y * rt.rect.height);

        SystemData.Instance.GetLevelData().SetCurrentRoom(mx, my);
        if(SystemData.Instance.GetLevelData().GetCurrentRoom() != null)
        {
            SetCursorAtCurremtRoom(SystemData.Instance.GetLevelData());
        }
        DrawLevelFromLevelDataToTexture(SystemData.Instance.GetLevelData(), false);
        if(onClickEventAction != null)
        {
            onClickEventAction();
        }
    }


    public void DrawLevelFromGeneratorToTexture()
    {
        FillTexture2D(miniMapTexture, Color.black);

        for (int i = 1; i <= LevelGenerator.GetLastRoomIndex(); i++)
        {
            int l, t, w, h;
            RoomInfo ri = LevelGenerator.FindRoomByIndex(i, out l, out t, out w, out h);
            if (ri != null)
            {
                DrawRoom(l, t, w, h, roomPieceTextures);
            }
        }

        for (int i = 1; i <= LevelGenerator.GetLastRoomIndex(); i++)
        {
            int l, t, w, h;
            RoomInfo ri = LevelGenerator.FindRoomByIndex(i, out l, out t, out w, out h);
            if (ri != null)
            {
                if (LevelGenerator.GetStartRoomIndex() == i)
                {
                    DrawRoomMarker(startRoomTexture, l, t, w, h);
                }
                if (LevelGenerator.GetEndRoomIndex() == i)
                {
                    DrawRoomMarker(endRoomTexture, l, t, w, h);
                }
                DrawDoors(l, t, w, h, ri.leftEdgeDoor, ri.topEdgeDoor);
            }
        }

        miniMapTexture.Apply();
    }

    public void DrawLevelFromLevelInfoToTexture(LevelInfo li)
    {
        FillTexture2D(miniMapTexture, Color.black);

        for (int i = 0; i < li.roomsList.Count; i++)
        {
            if (li.roomsList[i] != null)
            {
                DrawRoom(li.roomsList[i].x, li.roomsList[i].y, li.roomsList[i].w, li.roomsList[i].h, roomPieceTextures);
            }
        }

        for (int i = 0; i < li.roomsList.Count; i++)
        {
            if (li.roomsList[i] != null)
            {
                if (li.startRoomIndex == li.roomsList[i].index)
                {
                    DrawRoomMarker(startRoomTexture, li.roomsList[i].x, li.roomsList[i].y, li.roomsList[i].w, li.roomsList[i].h);
                }
                if (li.endRoomIndex == li.roomsList[i].index)
                {
                    DrawRoomMarker(endRoomTexture, li.roomsList[i].x, li.roomsList[i].y, li.roomsList[i].w, li.roomsList[i].h);
                }
                DrawDoors(li.roomsList[i].x, li.roomsList[i].y, li.roomsList[i].w, li.roomsList[i].h, li.roomsList[i].leftEdgeDoor, li.roomsList[i].topEdgeDoor);
            }
        }

        miniMapTexture.Apply();
    }

    public void DrawLevelFromLevelDataToTexture(LevelData ld, bool withCursor)
    {
        FillTexture2D(miniMapTexture, Color.black);

        for (int i = 0; i < ld.CountRooms(); i++)
        {
            RoomData rd = ld.GetRoom(i);

            if(rd == ld.GetCurrentRoom())
            {
                DrawRoom((int)rd.GetRoomGridX(), (int)rd.GetRoomGridY(), (int)(rd.GetRoomWidth() / RoomData.defaultRoomWidth), (int)(rd.GetRoomHeight() / RoomData.defaultRoomHeight), selectedRoomPieceTextures);
            }
            else
            {
                DrawRoom((int)rd.GetRoomGridX(), (int)rd.GetRoomGridY(), (int)(rd.GetRoomWidth() / RoomData.defaultRoomWidth), (int)(rd.GetRoomHeight() / RoomData.defaultRoomHeight), roomPieceTextures);
            }

            //DrawRoom((int)rd.GetRoomGridX(), (int)rd.GetRoomGridY(), (int)(rd.GetRoomWidth() / RoomData.defaultRoomWidth), (int)(rd.GetRoomHeight() / RoomData.defaultRoomHeight), roomPieceTextures);
        }

        for (int i = 0; i < ld.CountRooms(); i++)
        {
            RoomData rd = ld.GetRoom(i);
            if (rd != null)
            {
                if (LevelGenerator.GetStartRoomIndex() == i)
                {
                    DrawRoomMarker(startRoomTexture, (int)rd.GetRoomGridX(), (int)rd.GetRoomGridY(), (int)(rd.GetRoomWidth() / RoomData.defaultRoomWidth), (int)(rd.GetRoomHeight() / RoomData.defaultRoomHeight));
                }
                if (LevelGenerator.GetEndRoomIndex() == i)
                {
                    DrawRoomMarker(endRoomTexture, (int)rd.GetRoomGridX(), (int)rd.GetRoomGridY(), (int)(rd.GetRoomWidth() / RoomData.defaultRoomWidth), (int)(rd.GetRoomHeight() / RoomData.defaultRoomHeight));
                }
                DrawDoors((int)rd.GetRoomGridX(), (int)rd.GetRoomGridY(), (int)(rd.GetRoomWidth() / RoomData.defaultRoomWidth), (int)(rd.GetRoomHeight() / RoomData.defaultRoomHeight), rd.leftEdgeDoor, rd.topEdgeDoor);
            }
        }

        if (withCursor)
        {
            DrawCursor();
        }

        miniMapTexture.Apply();
    }

    public void DrawCursor()
    {
        int posX = cursorX * roomPieceTextures[ePiece_LRTB].width + roomPieceTextures[ePiece_LRTB].width / 2 - texCursor.width / 2;
        int posY = cursorY * roomPieceTextures[ePiece_LRTB].height + roomPieceTextures[ePiece_LRTB].height / 2 - texCursor.height / 2;
        CopyPixels(texCursor, 0, 0, texCursor.width, texCursor.height, miniMapTexture, posX, posY);
        miniMapTexture.Apply();
    }

    public void DrawRoomMarker(Texture2D tex, int l, int t, int w, int h)
    {
        int posX = l * roomPieceTextures[ePiece_LRTB].width + roomPieceTextures[ePiece_LRTB].width / 2 - tex.width / 2 + (w-1) * roomPieceTextures[ePiece_LRTB].width/2;
        int posY = t * roomPieceTextures[ePiece_LRTB].height + roomPieceTextures[ePiece_LRTB].height / 2 - tex.height / 2 + (h-1) * roomPieceTextures[ePiece_LRTB].height/2;
        CopyPixels(tex, 0, 0, tex.width, tex.height, miniMapTexture, posX, posY);
        miniMapTexture.Apply();
    }

    public void DrawDoors(int l, int t, int w, int h, int leftEdgeDoor, int topEdgeDoor)
    {
        int posX = l * roomPieceTextures[ePiece_LRTB].width;
        int posY = t * roomPieceTextures[ePiece_LRTB].height;

        if (leftEdgeDoor > 0)
        {
            int xx = posX - doorTexture.width / 2;
            int yy = posY + roomPieceTextures[ePiece_LRTB].height / 2 - doorTexture.height / 2;

            for(int i = 0; i < h; i++)
            {
                int v = (int)Mathf.Pow(2, i);
                if((v & leftEdgeDoor) > 0)
                {
                    CopyPixels(doorTexture, 0, 0, doorTexture.width, doorTexture.height, miniMapTexture, xx, yy + i * roomPieceTextures[ePiece_LRTB].height);
                }
            }
        }

        if(topEdgeDoor > 0)
        {
            int xx = posX + roomPieceTextures[ePiece_LRTB].width / 2 - doorTexture.width / 2;
            int yy = posY - doorTexture.height / 2;

            for (int i = 0; i < w; i++)
            {
                int v = (int)Mathf.Pow(2, i);
                if ((v & topEdgeDoor) > 0)
                {
                    CopyPixels(doorTexture, 0, 0, doorTexture.width, doorTexture.height, miniMapTexture, xx + i * roomPieceTextures[ePiece_LRTB].width, yy);
                }
            }
        }
    }

    public void DrawSelectedRect(int x1, int y1, int x2, int y2)
    {
        if(x2 < x1)
        {
            int xx = x1;
            x1 = x2;
            x2 = xx;
        }

        if (y2 < y1)
        {
            int yy = y1;
            y1 = y2;
            y2 = yy;
        }

        if(SystemData.Instance.GetLevelData().CanAddRoom(x1, y1, x2 - x1 + 1, y2 - y1 + 1))
        {
            DrawRoom(x1, y1, x2 - x1 + 1, y2 - y1 + 1, correctRoomPieceTextures);
        }
        else
        {
            DrawRoom(x1, y1, x2 - x1 + 1, y2 - y1 + 1, selectedRoomPieceTextures);
        }
        miniMapTexture.Apply();
    }

    void DrawRoom(int l, int t, int w, int h, Texture2D[] tex)
    {
        if(l < 0 || t < 0 || l + w > eLevelWidth || t + h > eLevelHeight)
        {
            return;
        }

        if(w == 1)
        {
            if (h == 1)
            {
                CopyPixels(tex[ePiece_LRTB], 0, 0, tex[ePiece_LRTB].width, tex[ePiece_LRTB].height, miniMapTexture, l * tex[ePiece_LRTB].width, t * tex[ePiece_LRTB].height);
            }
            else
            {
                CopyPixels(tex[ePiece_LRT], 0, 0, tex[ePiece_LRT].width, tex[ePiece_LRT].height, miniMapTexture, l * tex[ePiece_LRT].width, (t + h - 1) * tex[ePiece_LRT].height);
                CopyPixels(tex[ePiece_LRB], 0, 0, tex[ePiece_LRB].width, tex[ePiece_LRB].height, miniMapTexture, l * tex[ePiece_LRB].width, t * tex[ePiece_LRB].height);

                for(int y = t + 1; y < t + h - 1; y++)
                {
                    CopyPixels(tex[ePiece_LR], 0, 0, tex[ePiece_LR].width, tex[ePiece_LR].height, miniMapTexture, l * tex[ePiece_LR].width, y * tex[ePiece_LR].height);
                }
            }
        }
        else if(h == 1)
        {
            CopyPixels(tex[ePiece_LTB], 0, 0, tex[ePiece_LTB].width, tex[ePiece_LTB].height, miniMapTexture, l * tex[ePiece_LTB].width, t* tex[ePiece_LTB].height);
            CopyPixels(tex[ePiece_RTB], 0, 0, tex[ePiece_RTB].width, tex[ePiece_RTB].height, miniMapTexture, (l + w - 1) * tex[ePiece_RTB].width, t * tex[ePiece_RTB].height);

            for (int x = l + 1; x < l + w - 1; x++)
            {
                CopyPixels(tex[ePiece_TB], 0, 0, tex[ePiece_TB].width, tex[ePiece_TB].height, miniMapTexture, x * tex[ePiece_TB].width, t * tex[ePiece_TB].height);
            }
        }
        else
        {
            // bottom...
            CopyPixels(tex[ePiece_LB], 0, 0, tex[ePiece_LB].width, tex[ePiece_LB].height, miniMapTexture, l * tex[ePiece_LB].width, t * tex[ePiece_LB].height);
            CopyPixels(tex[ePiece_RB], 0, 0, tex[ePiece_RB].width, tex[ePiece_RB].height, miniMapTexture, (l + w - 1) * tex[ePiece_RB].width, t * tex[ePiece_RB].height);

            for (int x = l + 1; x < l + w - 1; x++)
            {
                CopyPixels(tex[ePiece_B], 0, 0, tex[ePiece_B].width, tex[ePiece_B].height, miniMapTexture, x * tex[ePiece_B].width, t * tex[ePiece_B].height);
            }

            // top...
            CopyPixels(tex[ePiece_LT], 0, 0, tex[ePiece_LT].width, tex[ePiece_LT].height, miniMapTexture, l * tex[ePiece_LT].width, (t + h - 1) * tex[ePiece_LT].height);
            CopyPixels(tex[ePiece_RT], 0, 0, tex[ePiece_RT].width, tex[ePiece_RT].height, miniMapTexture, (l + w - 1) * tex[ePiece_RT].width, (t + h - 1) * tex[ePiece_RT].height);

            for (int x = l + 1; x < l + w - 1; x++)
            {
                CopyPixels(tex[ePiece_T], 0, 0, tex[ePiece_T].width, tex[ePiece_T].height, miniMapTexture, x * tex[ePiece_T].width, (t + h - 1) * tex[ePiece_T].height);
            }

            // left
            for (int y = t + 1; y < t + h - 1; y++)
            {
                CopyPixels(tex[ePiece_L], 0, 0, tex[ePiece_L].width, tex[ePiece_L].height, miniMapTexture, l * tex[ePiece_L].width, y * tex[ePiece_L].height);
            }

            // right
            for (int y = t + 1; y < t + h - 1; y++)
            {
                CopyPixels(tex[ePiece_R], 0, 0, tex[ePiece_R].width, tex[ePiece_R].height, miniMapTexture, (l + w -1) * tex[ePiece_R].width, y * tex[ePiece_R].height);
            }

            // interior
            for (int x = l + 1; x < l + w - 1; x++)
            {
                for (int y = t + 1; y < t + h - 1; y++)
                {
                    CopyPixels(tex[ePiece_M], 0, 0, tex[ePiece_M].width, tex[ePiece_M].height, miniMapTexture, x * tex[ePiece_M].width, y * tex[ePiece_M].height);
                }
            }
        }
    }


    Texture2D CreateFillTexture2D(Color color, int textureWidth, int textureHeight)
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        texture.anisoLevel = 1;
        //texture.alphaIsTransparency = true;
        texture.wrapMode = TextureWrapMode.Clamp;

        Color c = new Color(0, 0, 0, 1);

        int numOfPixels = textureWidth * textureHeight;
        Color[] colors = new Color[numOfPixels];
        for (int x = 0; x < numOfPixels; x++)
        {
            colors[x] = c;
        }

        texture.SetPixels(colors);

        return texture;
    }

    void FillTexture2D(Texture2D tex, Color c)
    {
        int numOfPixels = tex.width * tex.height;
        Color[] colors = new Color[numOfPixels];
        for (int x = 0; x < numOfPixels; x++)
        {
            colors[x] = c;
        }

        tex.SetPixels(colors);
        tex.Apply();
    }

    void CopyPixels(Texture2D src, int srcX, int srcY, int Width, int Height, Texture2D dst, int dstX, int dstY)
    {
        Color[] pixels = src.GetPixels(srcX, srcY, Width, Height);
        dst.SetPixels(dstX, dstY, Width, Height, pixels);
    }
}
