using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GenerateScreenOverlay : BaseScreenOverlay
{
    [SerializeField]
    BaseButtonOverlay enviroPrevButton;
    [SerializeField]
    BaseButtonOverlay enviroNextButton;
    [SerializeField]
    BaseButtonOverlay nrOfRoomsPrevButton;
    [SerializeField]
    BaseButtonOverlay nrOfRoomsNextButton;
    [SerializeField]
    BaseButtonOverlay minSizeRoomPrevButton;
    [SerializeField]
    BaseButtonOverlay minSizeRoomNextButton;
    [SerializeField]
    BaseButtonOverlay maxSizeRoomPrevButton;
    [SerializeField]
    BaseButtonOverlay maxSizeRoomNextButton;
    [SerializeField]
    BaseButtonOverlay algorithmPrevButton;
    [SerializeField]
    BaseButtonOverlay algorithmNextButton;


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
        currentButtonIndex = GetCurrentButtonIndex();
        if (currentButtonIndex == 0) // environment
        {
            enviroPrevButton.OnClick();
        }
        else if (currentButtonIndex == 1) // num of rooms
        {
            nrOfRoomsPrevButton.OnClick();
        }
        else if (currentButtonIndex == 2) // min size room
        {
            minSizeRoomPrevButton.OnClick();
        }
        else if (currentButtonIndex == 3) // max size room
        {
            maxSizeRoomPrevButton.OnClick();
        }
        else if (currentButtonIndex == 4) // algorithm nr
        {
            algorithmPrevButton.OnClick();
        }
        else
        {
            base.OnLeft();
        }
    }

    override protected void OnRight()
    {
        currentButtonIndex = GetCurrentButtonIndex();
        if (currentButtonIndex == 0) // environment
        {
            enviroNextButton.OnClick();
        }
        else if (currentButtonIndex == 1) // num of rooms
        {
            nrOfRoomsNextButton.OnClick();
        }
        else if (currentButtonIndex == 2) // min size room
        {
            minSizeRoomNextButton.OnClick();
        }
        else if (currentButtonIndex == 3) // max size room
        {
            maxSizeRoomNextButton.OnClick();
        }
        else if (currentButtonIndex == 4) // algorithm nr
        {
            algorithmNextButton.OnClick();
        }
        else
        {
            base.OnRight();
        }
    }
} 