using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RoomScreenOverlay : BaseScreenOverlay
{
    RoomScreen roomScreen = null;

    protected void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        base.OnEnable();
    }

    override public void SetSrcBaseScreen(BaseScreen bs)
    {
        srcBaseScreen = bs;
        roomScreen = srcBaseScreen.GetComponent<RoomScreen>();
    }

    override protected void OnLeft()
    {
        if(bDownKeyX)
        {
            roomScreen.SelectPrevCategory();
            Map.Instance.OnCancel();
        }
        else if(bDownKeyB)
        {
            roomScreen.SelectPrevObject();
            Map.Instance.OnCancel();
        }
        else if (bDownKeyY)
        {
            roomScreen.MoveLevelMiniMapCursor(-1, 0);
            roomScreen.DrawLevelMiniMap(true);
        }
        else
        {
            int ret = Map.Instance.CursorLeft(SystemData.Instance.GetLevelData().GetCurrentRoom());
            if(ret > 0)
            {
                roomScreen.UpdateViewForCurrentRoom();
            }

            if (bDownKeyA || bDownKeyL)
            {
                CreateOrRemoveObject();
            }
        }
    }


    override protected void OnRight()
    {
        if (bDownKeyX)
        {
            roomScreen.SelectNextCategory();
            Map.Instance.OnCancel();
        }
        else if (bDownKeyB)
        {
            roomScreen.SelectNextObject();
            Map.Instance.OnCancel();
        }
        else if (bDownKeyY)
        {
            roomScreen.MoveLevelMiniMapCursor(1, 0);
            roomScreen.DrawLevelMiniMap(true);
        }
        else
        {
            int ret = Map.Instance.CursorRight(SystemData.Instance.GetLevelData().GetCurrentRoom());
            if (ret > 0)
            {
                roomScreen.UpdateViewForCurrentRoom();
            }

            if (bDownKeyA || bDownKeyL)
            {
                CreateOrRemoveObject();
            }
        }
    }

    override protected void OnUp()
    {
        if (bDownKeyY)
        {
            roomScreen.MoveLevelMiniMapCursor(0, 1);
            roomScreen.DrawLevelMiniMap(true);
        }
        else
        {
            //Map.Instance.CameraPositionVertical(5.0f * Time.deltaTime);
            //stickTimer = 0;
            int ret = Map.Instance.CursorUp(SystemData.Instance.GetLevelData().GetCurrentRoom());
            if (ret > 0)
            {
                roomScreen.UpdateViewForCurrentRoom();
            }

            if (bDownKeyA || bDownKeyL)
            {
                CreateOrRemoveObject();
            }
        }
    }

    override protected void OnDown()
    {
        if (bDownKeyY)
        {
            roomScreen.MoveLevelMiniMapCursor(0, -1);
            roomScreen.DrawLevelMiniMap(true);
        }
        else
        {
            int ret = Map.Instance.CursorDown(SystemData.Instance.GetLevelData().GetCurrentRoom());
            if (ret > 0)
            {
                roomScreen.UpdateViewForCurrentRoom();
            }

            if (bDownKeyA || bDownKeyL)
            {
                CreateOrRemoveObject();
            }
        }
    }

    override protected void OnRightStickRightContinuous()
    {
        Map.Instance.CameraPositionHorizontal(5.0f * Time.deltaTime);
    }

    override protected void OnRightStickLeftContinuous()
    {
        Map.Instance.CameraPositionHorizontal(-5.0f * Time.deltaTime);
    }

    override protected void OnRightStickUpContinuous()
    {
        Map.Instance.CameraPositionVertical(5.0f * Time.deltaTime);
    }

    override protected void OnRightStickDownContinuous()
    {
        Map.Instance.CameraPositionVertical(-5.0f * Time.deltaTime);
    }

    override public void Y_ActionDown()
    {
        roomScreen.DrawLevelMiniMap(true);
    }

    override public void Y_ActionUp()
    {
        roomScreen.UpdateCurrentRoom();
        roomScreen.DrawLevelMiniMap(false);
    }


    override public void B_ActionUp()
    {
        if (TemplateObject.Instance.GetChildCount() > 0)
        {
            Map.Instance.OnCancel();
        }
        else
        {
            roomScreen.CreateObjectTemplate(0);
        }
    }

    void CreateOrRemoveObject()
    {
        /*
        if (SystemData.Instance.GetEditorData().objectCategories[roomScreen.GetCurrentCategory()].categoryName.Equals("Rubber"))
        {
            //Map.Instance.RemoveObject();
        }
        else
        {
            TemplateObject.Instance.CreateOnLayer();
        }
        */
        if (bDownKeyL)
        {
            Map.Instance.RemoveObjectAtCursor();
        }
        else
        {
            if (bDownKeyA)
            {
                TemplateObject.Instance.CreateOnLayer();
                Map.Instance.UpdateTerrain3DShape();
            }
        }
    }

    // czy po puszczeniu L mamy przywrocic aktualny asset
    bool bReturnCurrentAsset = false;

    override public void L_ActionDown()
    {
        bReturnCurrentAsset = false;
        if (TemplateObject.Instance.GetChildCount() > 0)
        {
            Map.Instance.OnCancel();
            bReturnCurrentAsset = true;
        }
        Map.Instance.RemoveObjectAtCursor();
        Map.Instance.GetCursor().SetCursor_Rubber();
    }

    override public void L_ActionUp()
    {
        Map.Instance.GetCursor().SetCursor_Normal();
        if(bReturnCurrentAsset)
        {
            roomScreen.CreateObjectTemplate(0);
        }
        bReturnCurrentAsset = false;
    }

    override protected void A_ActionDown()
    {
        bDownKeyA = true;

        if (TemplateObject.Instance.GetChildCount() > 0)
        {
            UndoManager.AddToUndo();
            CreateOrRemoveObject();
        }
        else
        {
            BaseGameObject o = Map.Instance.GetObjectAtCursor();
            if(o != null)
            {
                /*
                EnemyObject eo = o.GetComponent<EnemyObject>();
                if(eo != null)
                {
                    roomScreen.ShowEnemyScreen(eo);
                }

                var so = o.GetComponent<EnemySpawner>();
                if (so != null) {
                    roomScreen.ShowSpawnerScreen(so);
                }

                var wo = o.GetComponent<EnemyWave>();
                if (wo != null) 
                {
                    roomScreen.ShowWaveScreen(wo);
                }
                */
            }
        }
    }
    
    override protected void A_ActionUp()
    {
        bDownKeyA = false;
    }

}