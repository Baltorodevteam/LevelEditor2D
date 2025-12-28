using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScreenOverlay : MonoBehaviour
{
    //public static BaseScreenOverlay previousScreenOverlay = null;
    //public static BaseScreenOverlay currentScreenOverlay = null;


    protected float defaultStickTimerLR = 0.2f;
    protected float defaultStickTimerUD = 0.2f;


    public BaseButtonOverlay[] list;

    public BaseButtonOverlay startButton;
    public BaseButtonOverlay backButton;
    public BaseButtonOverlay acceptButton;
    public BaseButtonOverlay xButton;
    public BaseButtonOverlay yButton;
    public BaseButtonOverlay plusButton;
    public BaseButtonOverlay lButton;
    public BaseButtonOverlay rButton;
    public BaseButtonOverlay zlButton;
    public BaseButtonOverlay zrButton;
    public BaseButtonOverlay l3Button;
    public BaseButtonOverlay r3Button;


    protected BaseButtonOverlay currentButton;
    protected int currentButtonIndex = 0;

    protected float stickTimer;

    protected int joystickNr = 1;

    protected bool bDownKeyA = false;
    protected bool bDownKeyB = false;
    protected bool bDownKeyX = false;
    protected bool bDownKeyY = false;
    protected bool bDownKeyPlus = false;
    protected bool bDownKeyL = false;
    protected bool bDownKeyR = false;
    protected bool bDownKeyZL = false;
    protected bool bDownKeyZR = false;
    protected bool bDownKeyL3 = false;
    protected bool bDownKeyR3 = false;


    protected BaseScreen srcBaseScreen = null;


    virtual public void SetSrcBaseScreen(BaseScreen bs)
    {
        srcBaseScreen = bs;
    }


    public BaseScreen GetScreen()
    {
        return gameObject.GetComponent<BaseScreen>();
    }

    // Ukrywa selectory wszystkich baseButtonow z listy
    virtual public void DisableListSelectors()
    {
        if (list != null && list.Length > 0)
        {
            foreach (BaseButtonOverlay b in list)
            {
                b.OnUp();
                b.OnExit();
                b.SelectorEnable(false);
            }
        }
    }

    // wolane przy zmianie selecta
    public virtual void OnChangeSelectionEvent()
    {
    }

    public void SetCurrentButton(BaseButtonOverlay b)
    {
        DisableListSelectors();
        currentButton = b;
        currentButton.SelectorEnable(true);
        currentButtonIndex = GetCurrentButtonIndex();

        OnChangeSelectionEvent();
    }

    public int GetCurrentButtonIndex()
    {
        if (list != null)
        {
            for(int i=0; i < list.Length; i++)
            {
                if(currentButton==list[i])
                {
                    return i;
                }
            }
        }
        return -1;
    }

    virtual public void OnButtonEvent(int nID)
    {
        if (nID == 100000) // select
        {
            //DownA();
            //UpA();
        }
    }

    // Use this for initialization
    protected void Start ()
    {
        DisableListSelectors();

        if (startButton != null)
        {
            currentButton = startButton;
            currentButton.OnEnter();
            //currentButton.OnDown();
            currentButtonIndex = GetCurrentButtonIndex();
        }
    }

    protected void OnEnable()
    {
        DisableListSelectors();

        if (currentButton)
        {
            currentButton.OnEnter();
            currentButtonIndex = GetCurrentButtonIndex();

            OnChangeSelectionEvent();
        }

        bDownKeyA = bDownKeyB = bDownKeyX = bDownKeyY = bDownKeyPlus = false;
    }

    virtual protected void UpdateScreen()
    {

    }

    // Update is called once per frame
    virtual protected void Update ()
    {
        if(BaseScreen.CurrentScreen == null || BaseScreen.CurrentScreen.GetScreenOverlay() != this)
        {
            return;
        }

        UpdateScreen();

        UpdateArrows();

        UpdateStickL();
        UpdateStickR();

        A_Action_Down();
        A_Action_Up();

        B_Action_Down();
        B_Action_Up();

        X_Action_Down();
        X_Action_Up();

        Y_Action_Down();
        Y_Action_Up();

        Plus_Action_Down();
        Plus_Action_Up();

        L_Action_Down();
        L_Action_Up();

        R_Action_Down();
        R_Action_Up();

        ZL_Action_Down();
        ZR_Action_Down();

        L3_Action_Down();
        L3_Action_Up();

        R3_Action_Down();
        R3_Action_Up();
    }

    virtual protected void A_Action_Down()
    {
        if(BGInput.GetButtonDown(BGGamepadButton.ButtonEast))
        {
            A_ActionDown();
            bDownKeyA = true;
        }
    }

    virtual protected void A_ActionDown()
    {
        if (acceptButton)
        {
            if (acceptButton.targetObject.activeInHierarchy)
            {
                bDownKeyA = true;
                acceptButton.OnEnter();
                acceptButton.OnDown();
            }
        }
        else if (currentButton != null)
        {
            bDownKeyA = true;
            currentButton.OnDown();
        }
    }

    virtual protected void A_Action_Up()
    {
        if (BGInput.GetButtonUp(BGGamepadButton.ButtonEast))
        {
            bDownKeyA = false;
            A_ActionUp();
        }
    }

    virtual protected void A_ActionUp()
    {
        if (acceptButton)
        {
            if (acceptButton.targetObject.activeInHierarchy)
            {
                acceptButton.OnClick();
                //acceptButton.OnUp();
                acceptButton.OnExit();
            }
        }
        else if (currentButton != null)
        {
            currentButton.OnClick();
            currentButton.OnUp();
            if (currentButton.exitAfterClick)
            {
                currentButton.OnExit();
                currentButton.SelectorEnable(true);
            }
        }
        bDownKeyA = false;
    }

    virtual protected void OnLeft()
    {
        if(currentButton == null)
        {
            return;
        }

        BaseButtonOverlay item = currentButton.leftNeighbour;
        if (item != null)
        {
            currentButton.OnUp(); // added
            currentButton.OnExit();
            item.OnEnter();
            //item.OnDown(); // added
            currentButton = item;
            currentButtonIndex = GetCurrentButtonIndex();

            OnChangeSelectionEvent();
        }
    }

    virtual protected void OnRight()
    {
        if (currentButton == null)
        {
            return;
        }

        BaseButtonOverlay item = currentButton.rightNeighbour;
        if (item != null)
        {
            currentButton.OnUp(); // added
            currentButton.OnExit();
            item.OnEnter();
            //item.OnDown(); // added
            currentButton = item;
            currentButtonIndex = GetCurrentButtonIndex();

            OnChangeSelectionEvent();
        }
    }

    virtual protected void OnUp()
    {
        if (currentButton == null)
        {
            return;
        }

        BaseButtonOverlay item = currentButton.upNeighbour;
        if (item != null)
        {
            currentButton.OnUp(); // added
            currentButton.OnExit();
            item.OnEnter();
            //item.OnDown(); // added
            currentButton = item;
            currentButtonIndex = GetCurrentButtonIndex();

            OnChangeSelectionEvent();
        }
    }

    virtual protected void OnDown()
    {
        if (currentButton == null)
        {
            return;
        }

        BaseButtonOverlay item = currentButton.downNeighbour;
        if (item != null)
        {
            currentButton.OnUp(); // added
            currentButton.OnExit();
            item.OnEnter();
            //item.OnDown(); // added
            currentButton = item;
            currentButtonIndex = GetCurrentButtonIndex();

            OnChangeSelectionEvent();
        }
    }

    virtual protected void OnDownContinuous()
    {

    }

    virtual protected void OnUpContinuous()
    {

    }

    virtual protected void OnLeftContinuous()
    {

    }

    virtual protected void OnRightContinuous()
    {

    }

    virtual protected void UpdateStickTimer()
    {
        stickTimer -= Time.unscaledDeltaTime;
    }

    virtual protected void UpdateStickR()
    {
        /*
        UpdateStickTimer();
        if (stickTimer > 0)
        {
            return;
        }
        */

        Vector2 vec = BGInput.GetJoystickRight();

        if (vec.x > 0.7f)
        {
            //stickTimer = defaultStickTimerLR;
            OnRightStickRightContinuous();
        }
        else if (vec.x < -0.7f)
        {
            //stickTimer = defaultStickTimerLR;
            OnRightStickLeftContinuous();
        }

        if (vec.y < -0.7f)
        {
            //stickTimer = defaultStickTimerUD;
            OnRightStickDownContinuous();
        }
        else if (vec.y > 0.7f)
        {
            //stickTimer = defaultStickTimerUD;
            OnRightStickUpContinuous();
        }

    }

    virtual protected void OnRightStickRightContinuous()
    {

    }

    virtual protected void OnRightStickLeftContinuous()
    {

    }

    virtual protected void OnRightStickUpContinuous()
    {

    }

    virtual protected void OnRightStickDownContinuous()
    {

    }

    virtual protected void UpdateStickL()
    {
        UpdateStickTimer();
        if (stickTimer > 0)
        {
            return;
        }

        Vector2 vec = BGInput.GetJoystickLeft();

        if(vec.x > 0.9f)
        {
            stickTimer = defaultStickTimerLR;
            OnRight();
            OnRightContinuous();
        }
        else if (vec.x < -0.9f)
        {
            stickTimer = defaultStickTimerLR;
            OnLeft();
            OnLeftContinuous();
        }
        else if (vec.y < -0.9f)
        {
            stickTimer = defaultStickTimerUD;
            OnDown();
            OnDownContinuous();
        }
        else if(vec.y > 0.9f)
        {
            stickTimer = defaultStickTimerUD;
            OnUp();
            OnUpContinuous();
        }
    }

    virtual protected void UpdateArrows()
    {
        if (BGInput.GetButtonDown(BGGamepadButton.UpArrow))
        {
            OnUp();
        }
        else if (BGInput.GetButtonDown(BGGamepadButton.DownArrow))
        {
            OnDown();
        }
        if (BGInput.GetButtonDown(BGGamepadButton.LeftArrow))
        {
            OnLeft();
        }
        else if (BGInput.GetButtonDown(BGGamepadButton.RightArrow))
        {
            OnRight();
        }

        if (BGInput.GetButton(BGGamepadButton.UpArrow))
        {
            OnUpContinuous();
        }
        else if (BGInput.GetButton(BGGamepadButton.DownArrow))
        {
            OnDownContinuous();
        }

        if (BGInput.GetButton(BGGamepadButton.LeftArrow))
        {
            OnLeftContinuous();
        }
        else if (BGInput.GetButton(BGGamepadButton.RightArrow))
        {
            OnRightContinuous();
        }
    }


    virtual protected void B_Action_Down()
    {
        if (BGInput.GetButtonDown(BGGamepadButton.ButtonSouth))
        {
            if (backButton != null && backButton.targetObject && backButton.targetObject.activeInHierarchy)
            {
                backButton.OnEnter();
                backButton.OnDown();
            }
            else
            {
                B_ActionDown();
            }
            bDownKeyB = true;
        }
    }

    virtual protected void B_Action_Up()
    {
        if (BGInput.GetButtonUp(BGGamepadButton.ButtonSouth))
        {
            if (backButton != null && backButton.targetObject && backButton.targetObject.activeInHierarchy)
            {
                backButton.OnClick();
                //backButton.OnUp();
                backButton.OnExit();
            }
            else
            {
                B_ActionUp();
            }
            bDownKeyB = false;
        }
    }

    virtual public void B_ActionDown()
    {

    }
    virtual public void B_ActionUp()
    {

    }

    virtual protected void X_Action_Down()
    {
        if (BGInput.GetButtonDown(BGGamepadButton.ButtonNorth))
        {
            if (xButton != null && xButton.targetObject && xButton.targetObject.activeInHierarchy)
            {
                xButton.OnEnter();
                xButton.OnDown();
            }
            else
            {
                X_ActionDown();
            }

            bDownKeyX = true;
        }
    }

    virtual protected void X_Action_Up()
    {
        if (BGInput.GetButtonUp(BGGamepadButton.ButtonNorth))
        {
            if (xButton != null && xButton.targetObject && xButton.targetObject.activeInHierarchy)
            {
                xButton.OnClick();
                //xButton.OnUp();
                xButton.OnExit();
            }
            else
            {
                X_ActionUp();
            }

            bDownKeyX = false;
        }
    }
    virtual public void X_ActionDown()
    {

    }
    virtual public void X_ActionUp()
    {

    }

    virtual protected void Y_Action_Down()
    {
        if (BGInput.GetButtonDown(BGGamepadButton.ButtonWest))
        {
            if (yButton != null && yButton.targetObject && yButton.targetObject.activeInHierarchy)
            {
                yButton.OnEnter();
                yButton.OnDown();
            }
            else
            {
                Y_ActionDown();
            }

            bDownKeyY = true;
        }
    }

    virtual protected void Y_Action_Up()
    {
        if (BGInput.GetButtonUp(BGGamepadButton.ButtonWest))
        {
            if (yButton != null && yButton.targetObject && yButton.targetObject.activeInHierarchy)
            {
                yButton.OnClick();
                //yButton.OnUp();
                yButton.OnExit();
            }
            else
            {
                Y_ActionUp();
            }

            bDownKeyY = false;
        }
    }

    virtual public void Y_ActionDown()
    {

    }
    virtual public void Y_ActionUp()
    {

    }

    virtual protected void Plus_Action_Down()
    {
        if (BGInput.GetButtonDown(BGGamepadButton.StartButton))
        {
            if (plusButton != null && plusButton.targetObject && plusButton.targetObject.activeInHierarchy)
            {
                plusButton.OnEnter();
                plusButton.OnDown();
            }
            else
            {
                Plus_ActionDown();
            }
            bDownKeyPlus = true;
        }
    }

    virtual protected void Plus_Action_Up()
    {
        if (BGInput.GetButtonUp(BGGamepadButton.StartButton))
        {
            if (plusButton != null && plusButton.targetObject && plusButton.targetObject.activeInHierarchy)
            {
                plusButton.OnClick();
                plusButton.OnExit();
            }
            else
            {
                Plus_ActionUp();
            }

            bDownKeyPlus = false;
        }
    }

    virtual public void Plus_ActionDown()
    {

    }
    virtual public void Plus_ActionUp()
    {

    }

    virtual protected void L_Action_Down()
    {
        if (BGInput.GetButtonDown(BGGamepadButton.LeftBumper))
        {
            bDownKeyL = true;

            if (lButton != null && lButton.targetObject && lButton.targetObject.activeInHierarchy)
            {
                //lButton.OnClick();
                //lButton.OnExit();
                lButton.OnEnter();
                lButton.OnDown();
            }
            else
            {
                L_ActionDown();
            }
        }
    }

    virtual public void L_ActionDown()
    {

    }

    virtual protected void L_Action_Up()
    {
        if (BGInput.GetButtonUp(BGGamepadButton.LeftBumper))
        {
            bDownKeyL = false;
            
            if (lButton != null && lButton.targetObject && lButton.targetObject.activeInHierarchy)
            {
                lButton.OnClick();
                lButton.OnExit();
            }
            else
            {
                L_ActionUp();
            }
        }
    }

    virtual public void L_ActionUp()
    {

    }

    virtual protected void R_Action_Down()
    {
        if (BGInput.GetButtonDown(BGGamepadButton.RightBumper))
        {
            if (rButton != null && rButton.targetObject && rButton.targetObject.activeInHierarchy)
            {
                rButton.OnEnter();
                rButton.OnDown();
            }
            else
            {
                R_ActionDown();
            }

            bDownKeyR = true;
        }
    }

    virtual public void R_ActionDown()
    {

    }

    virtual protected void R_Action_Up()
    {
        if (BGInput.GetButtonUp(BGGamepadButton.RightBumper))
        {
            if (rButton != null && rButton.targetObject && rButton.targetObject.activeInHierarchy)
            {
                rButton.OnClick();
                rButton.OnExit();
            }
            else
            {
                R_ActionUp();
            }

            bDownKeyR = false;
        }
    }

    virtual public void R_ActionUp()
    {

    }

    virtual protected void ZL_Action_Down()
    {
        if (BGInput.GetButtonDown(BGGamepadButton.LeftTrigger))
        {
            if (zlButton != null && zlButton.targetObject && zlButton.targetObject.activeInHierarchy)
            {
                zlButton.OnClick();
                zlButton.OnExit();
            }
            else
            {
                ZL_ActionDown();
            }

            //bDownKeyZL = true;
        }
    }

    virtual public void ZL_ActionDown()
    {

    }

    virtual protected void ZR_Action_Down()
    {
        if (BGInput.GetButtonDown(BGGamepadButton.RightTrigger))
        {
            if (zrButton != null && zrButton.targetObject && zrButton.targetObject.activeInHierarchy)
            {
                zrButton.OnClick();
                zrButton.OnExit();
            }
            else
            {
                ZR_ActionDown();
            }

            //bDownKeyZR = true;
        }
    }

    virtual public void ZR_ActionDown()
    {

    }

    virtual protected void L3_Action_Down()
    {
        if (BGInput.GetButtonDown(BGGamepadButton.LeftStickButton))
        {
            if (l3Button != null && l3Button.targetObject && l3Button.targetObject.activeInHierarchy)
            {
                l3Button.OnEnter();
                l3Button.OnDown();
            }
            else
            {
                L3_ActionDown();
            }

            bDownKeyL3 = true;
        }
    }

    virtual protected void L3_Action_Up()
    {
        if (BGInput.GetButtonUp(BGGamepadButton.LeftStickButton))
        {
            if (l3Button != null && l3Button.targetObject && l3Button.targetObject.activeInHierarchy)
            {
                l3Button.OnClick();
                l3Button.OnUp();
                l3Button.OnExit();
            }
            else
            {
                L3_ActionUp();
            }

            bDownKeyL3 = false;
        }
    }

    virtual public void L3_ActionDown()
    {

    }

    virtual public void L3_ActionUp()
    {

    }

    virtual protected void R3_Action_Down()
    {
        if (BGInput.GetButtonDown(BGGamepadButton.RightStickButton))
        {
            if (r3Button != null && r3Button.targetObject && r3Button.targetObject.activeInHierarchy)
            {
                r3Button.OnEnter();
                r3Button.OnDown();
            }
            else
            {
                R3_ActionDown();
            }

            bDownKeyR3 = true;
        }
    }

    virtual protected void R3_Action_Up()
    {
        if (BGInput.GetButtonUp(BGGamepadButton.RightStickButton))
        {
            if (r3Button != null && r3Button.targetObject && r3Button.targetObject.activeInHierarchy)
            {
                r3Button.OnClick();
                r3Button.OnUp();
                r3Button.OnExit();
            }
            else
            {
                R3_ActionUp();
            }

            bDownKeyR3 = false;
        }
    }

    virtual public void R3_ActionDown()
    {

    }

    virtual public void R3_ActionUp()
    {

    }
}
