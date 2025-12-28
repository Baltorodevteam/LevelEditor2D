using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class BaseButtonOverlay : MonoBehaviour
{
    public GameObject targetObject;
    public GameObject selectorObject;

    public BaseButtonOverlay leftNeighbour;
    public BaseButtonOverlay rightNeighbour;
    public BaseButtonOverlay upNeighbour;
    public BaseButtonOverlay downNeighbour;

    public BaseScreenOverlay parentScreen;

    public bool exitAfterClick = true;
    public int buttonID = 0;

    void Awake()
    {
        if(selectorObject != null)
        {
            selectorObject.SetActive(false);
        }

        if(transform.parent)
        {
            parentScreen = transform.parent.gameObject.GetComponent<BaseScreenOverlay>();
        }

        if(targetObject)
        {
            Button b = targetObject.GetComponent<Button>();
            if(b)
            {
                b.onClick.AddListener(OnClickFunctionExt);
            }
            else
            {
                Toggle t = targetObject.GetComponent<Toggle>();
                if (t)
                {
                    t.onValueChanged.AddListener(OnChangeFunctionExt);
                }
            }
        }
    }

    virtual public void OnClickFunctionExt()
    {
        if (parentScreen)
        {
            if(selectorObject)
            {
                parentScreen.DisableListSelectors();
                parentScreen.SetCurrentButton(this);
                SelectorEnable(true);
            }
            parentScreen.OnButtonEvent(buttonID);
        }
    }

    virtual protected void OnChangeFunctionExt(bool b)
    {
        if (parentScreen)
        {
            if (selectorObject)
            {
                parentScreen.DisableListSelectors();
                parentScreen.SetCurrentButton(this);
                SelectorEnable(true);
            }
            parentScreen.OnButtonEvent(buttonID);
        }
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    virtual public void OnEnter()
    {
        if(selectorObject != null)
        {
            selectorObject.SetActive(true);
        }
        
        ExecuteEvents.Execute(targetObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
    }

    virtual public void OnExit()
    {
        if (selectorObject != null)
        {
            selectorObject.SetActive(false);
        }

        ExecuteEvents.Execute(targetObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
    }

    virtual public void OnClick()
    {
        ExecuteEvents.Execute(targetObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
    }

    virtual public void OnDown()
    {
        ExecuteEvents.Execute(targetObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
    }

    virtual public void OnUp()
    {
        ExecuteEvents.Execute(targetObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
    }

    virtual public void SelectorEnable(bool b)
    {
        if (selectorObject != null)
        {
            selectorObject.SetActive(b);
        }
    }
}
