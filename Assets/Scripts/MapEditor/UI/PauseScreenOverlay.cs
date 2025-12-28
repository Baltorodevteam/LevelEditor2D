using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PauseScreenOverlay : BaseScreenOverlay
{
    [SerializeField]
    BaseButtonOverlay enviroPrevButton;
    [SerializeField]
    BaseButtonOverlay enviroNextButton;
    [SerializeField]
    BaseButtonOverlay musicPrevButton;
    [SerializeField]
    BaseButtonOverlay musicNextButton;

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
        else if (currentButtonIndex == 1) // music
        {
            musicPrevButton.OnClick();
        }
    }

    override protected void OnRight()
    {
        currentButtonIndex = GetCurrentButtonIndex();
        if (currentButtonIndex == 0) // environment
        {
            enviroNextButton.OnClick();
        }
        else if (currentButtonIndex == 1) // music
        {
            musicNextButton.OnClick();
        }
    }
} 