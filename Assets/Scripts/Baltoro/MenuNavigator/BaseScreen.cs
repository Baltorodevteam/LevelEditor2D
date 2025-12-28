using UnityEngine;


public class BaseScreen : MonoBehaviour
{
    public static BaseScreen CurrentScreen { get; private set; }

    public BaseScreen PreviousScreen { get; set; }
    public BaseScreen ParentScreen { get; set; }

    public int screenIdx = 0;
    public BaseScreenOverlay screenOverlay;


    public void SetPreviousScreen(BaseScreen bs)
    {
        PreviousScreen = bs;
    }

    public void BackToPreviousScreen()
    {
        PreviousScreen.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public BaseScreenOverlay GetScreenOverlay()
    {
        return screenOverlay;/* gameObject.GetComponent<BaseScreenOverlay>()*/;
    }

    virtual public void OnEnterScreen(BaseScreen previousScreen)
    {
        PreviousScreen = previousScreen;
        CurrentScreen = this;
        if(screenOverlay != null)
        {
            screenOverlay.SetSrcBaseScreen(this);
        }
    }

    virtual public void OnLeaveScreen(BaseScreen nextScreen)
    {
        nextScreen.PreviousScreen = this;
        CurrentScreen = nextScreen;
    }
}
