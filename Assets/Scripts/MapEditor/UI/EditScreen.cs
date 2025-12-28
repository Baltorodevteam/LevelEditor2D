using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class EditScreen : BaseScreen
{
    [SerializeField]
    Text environmentValueText;
    [SerializeField]
    LevelMiniMap levelMiniMap;
    [SerializeField]
    RoomScreen roomScreen;

    int startCursorX = -1;
    int startCursorY = -1;


    public void SetStartCursorXY()
    {
        startCursorX = levelMiniMap.GetCursorX();
        startCursorY = levelMiniMap.GetCursorY();
        DrawLevelMiniMap(true);
    }

    public void ResetStartCursorXY()
    {
        startCursorX = -1;
        startCursorY = -1;
        DrawLevelMiniMap(true);
    }

    public void SetStopCursorXY()
    {
        int x = levelMiniMap.GetCursorX();
        int y = levelMiniMap.GetCursorY();

        if (x < startCursorX)
        {
            int xx = x;
            x = startCursorX;
            startCursorX = xx;
        }

        if (y < startCursorY)
        {
            int yy = y;
            y = startCursorY;
            startCursorY = yy;
        }

        if(SystemData.Instance.GetLevelData().CanAddRoom(startCursorX, startCursorY, x - startCursorX + 1, y - startCursorY + 1))
        {
            SystemData.Instance.GetLevelData().AddRoom(startCursorX, startCursorY, x - startCursorX + 1, y - startCursorY + 1);
        }

        startCursorX = -1;
        startCursorY = -1;

        levelMiniMap.DrawLevelFromLevelDataToTexture(SystemData.Instance.GetLevelData(), true);
    }

    public void DeleteRoomAtCursorPosition()
    {
        RoomData rd = SystemData.Instance.GetLevelData().GetRoom(levelMiniMap.GetCursorX(), levelMiniMap.GetCursorY());
        if(rd != null)
        {
            bool needNewCurrentRoom = rd == SystemData.Instance.GetLevelData().GetCurrentRoom();
            SystemData.Instance.GetLevelData().DeleteRoom(rd);

            if(needNewCurrentRoom)
            {
                SystemData.Instance.GetLevelData().SetDefaultRoomData(-1, -1);
            }

            DrawLevelMiniMap(true);
        }
    }


    private void OnEnable()
    {
        OnEnterScreen(null);

        environmentValueText.text = "" + (SystemData.Instance.GetEditorData().currentEnvironment + 1);

        levelMiniMap.SetCursorXY(LevelGenerator.levelWidth / 2, LevelGenerator.levelHeight / 2);

        levelMiniMap.SetDownEventExtAction(OnLevelMiniMapDownAction);
        levelMiniMap.SetUpEventExtAction(OnLevelMiniMapUpAction);

        DrawLevelMiniMap(true);
    }

    public void MoveLevelMiniMapCursor(int dx, int dy)
    {
        levelMiniMap.MoveCursor(dx, dy);
    }

    public void DrawLevelMiniMap(bool withCursor)
    {
        levelMiniMap.DrawLevelFromLevelDataToTexture(SystemData.Instance.GetLevelData(), withCursor);
        if(startCursorX >= 0 && startCursorY >= 0)
        {
            levelMiniMap.DrawSelectedRect(startCursorX, startCursorY, levelMiniMap.GetCursorX(), levelMiniMap.GetCursorY());
            levelMiniMap.DrawCursor();
        }
    }

    public void OnEnvironmentPrev()
    {
        SystemData.Instance.GetEditorData().currentEnvironment--;
        if (SystemData.Instance.GetEditorData().currentEnvironment < 0)
        {
            SystemData.Instance.GetEditorData().currentEnvironment = SystemData.Instance.GetEditorData().environments.Length - 1;
        }

        environmentValueText.text = "" + (SystemData.Instance.GetEditorData().currentEnvironment + 1);
    }

    public void OnEnvironmentNext()
    {
        SystemData.Instance.GetEditorData().currentEnvironment++;
        SystemData.Instance.GetEditorData().currentEnvironment %= SystemData.Instance.GetEditorData().environments.Length;

        environmentValueText.text = "" + (SystemData.Instance.GetEditorData().currentEnvironment + 1);
    }

    public void OnDone()
    {
        if(SystemData.Instance.GetLevelData().CountRooms() > 0)
        {
            SystemData.Instance.GetLevelData().SetDefaultRoomData(-1, -1);
            SystemData.Instance.GetLevelData().CreateBackGrounds();

            gameObject.SetActive(false);
            roomScreen.gameObject.SetActive(true);
        }
    }

    public void OnLevelMiniMapDownAction(int x, int y)
    {

    }

    public void OnLevelMiniMapUpAction(int x, int y)
    {

    }
}
