using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class EditScreenOverlay : BaseScreenOverlay
{
    [SerializeField]
    EditScreen editScreen;


    void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        base.OnEnable();
    }

    override protected void OnLeft()
    {
        editScreen.MoveLevelMiniMapCursor(-1, 0);
        editScreen.DrawLevelMiniMap(true);

        if (bDownKeyY)
        {

        }
    }

    override protected void OnRight()
    {
        editScreen.MoveLevelMiniMapCursor(1, 0);
        editScreen.DrawLevelMiniMap(true);
        
        if (bDownKeyY)
        {

        }
    }

    override protected void OnUp()
    {
        editScreen.MoveLevelMiniMapCursor(0, 1);
        editScreen.DrawLevelMiniMap(true);

        if (bDownKeyY)
        {

        }
    }

    override protected void OnDown()
    {
        editScreen.MoveLevelMiniMapCursor(0, -1);
        editScreen.DrawLevelMiniMap(true);

        if (bDownKeyY)
        {

        }
    }

    override protected void A_ActionDown()
    {
        editScreen.SetStartCursorXY();
    }

    override protected void A_ActionUp()
    {
        editScreen.SetStopCursorXY();
    }

    override public void X_ActionDown()
    {
        editScreen.ResetStartCursorXY();
        editScreen.DeleteRoomAtCursorPosition();
    }
} 