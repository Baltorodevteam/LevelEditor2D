using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class LoadScreenOverlay : BaseScreenOverlay
{
    [SerializeField]
    BaseButtonOverlay upButton;
    [SerializeField]
    BaseButtonOverlay downButton;
    [SerializeField]
    LoadScreen loadScreen;


    void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        base.OnEnable();
    }
    /*
    override protected void OnUpContinuous()
    {
        upButton.OnClick();
    }

    override protected void OnDownContinuous()
    {
        downButton.OnClick();
    }
    */
    override protected void OnUp()
    {
        upButton.OnClick();
    }

    override protected void OnDown()
    {
        downButton.OnClick();
    }

    override protected void A_ActionDown()
    {
        loadScreen.OnLoadCurrent();
    }
} 