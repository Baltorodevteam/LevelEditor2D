using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;


public class BaseGameObject : MonoBehaviour
{
    public const float SORTING_ORDER_BASE = 50000;

    [SerializeField]
    public int objectID = 0;
    [SerializeField]
    public int width;
    [SerializeField]
    public int height;
    [SerializeField]
    bool withCollider = false;

    [SerializeField]
    bool randomSprite = false;
    [SerializeField]
    int terrainSprite = 0;
    [SerializeField]
    int destructibleSprite = 0;

    [SerializeField]
    protected Sprite[] sprites;
    protected int currentSpriteIndex = 0;

    [SerializeField]
    protected GameObject editorObject;
    [SerializeField]
    protected GameObject gameplayObject;


    [SerializeField]
    int defaultLayer = -1;

    [SerializeField]
    protected float pivotOffset = 0.0f;

    float offsetPosY = 0;

    //protected Renderer myRenderer;
    protected SpriteRenderer myRenderer;
    protected SortingGroup sortingGroup;

    protected SpriteRenderer editorObjectRenderer;

    protected Collider2D myCollider;

    virtual public void UpdateEnvironment(EditorData ed)
    {

    }

    virtual public void UpdateSortingOrder()
    {
        int sortingOrder = GetSortingOrder(transform, pivotOffset);

        if (myRenderer)
            myRenderer.sortingOrder = sortingOrder;

        if (sortingGroup)
            sortingGroup.sortingOrder = sortingOrder;

    }

    public static int GetSortingOrder(Transform t, float pivotOffset)
    {
        return (int)(SORTING_ORDER_BASE - 10.0f * (t.position.y + pivotOffset));
    }

    public void SetOffsetPosY(float y)
    {
        offsetPosY = y;
    }

    public float GetOffsetPosY()
    {
        return offsetPosY;
    }

    public void SetPivotOffset(float offset) {
        pivotOffset = offset;
    }

    public float GetPivotOffset() {
        return pivotOffset;
    }

    public void SetColliderOffsetY(float y)
    {
        if (myCollider != null)
        {
            myCollider.offset = new Vector2(0, y);
        }
    }

    public float GetColliderOffsetY()
    {
        if (myCollider != null)
        {
            return myCollider.offset.y;
        }
        return 0;
    }


    public bool IsColliderableObject()
    {
        return withCollider;
    }

    public int IsTerrain()
    {
        return terrainSprite;
    }

    public void SetTerrain(int i)
    {
        terrainSprite = i;
    }

    public int IsDestructible()
    {
        return destructibleSprite;
    }

    public void SetDestructible(int i)
    {
        destructibleSprite = i;
    }

    public int GetDefaultLayer()
    {
        return defaultLayer;
    }

    virtual public void SetSize(float w, float h)
    {
        myRenderer.size = new Vector2(w, h);
    }

    public void SetSprite(Sprite s)
    {
        myRenderer.sprite = s;
    }

    public Sprite GetSprite()
    {
        /*
        if(sprites != null && sprites.Length > currentSpriteIndex)
        {
            return sprites[currentSpriteIndex];
        }
        */
        return myRenderer.sprite;
    }

    virtual public void SetSpriteIndex(int nIndex)
    {
        if (myRenderer && sprites != null && sprites.Length > 0 && nIndex >= 0 && nIndex < sprites.Length)
        {
            currentSpriteIndex = nIndex;
            myRenderer.sprite = sprites[nIndex];
        }
    }

    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(x, y, 0);
    }

    public void SetLocalPosition(float x, float y)
    {
        transform.localPosition = new Vector3(x, y, 0);
    }

    public virtual void SetColorA(float a)
    {
        Color c = myRenderer.color;
        c.a = a;
        myRenderer.color = c;
    }

    public virtual void SetColorRGBA(float r, float g, float b, float a)
    {
        Color c = myRenderer.color;
        c.r = r; c.g = g; c.b = b; c.a = a;
        myRenderer.color = c;
    }


    protected virtual void Awake()
    {
        //myRenderer = GetComponent<Renderer>();
        myRenderer = GetComponent<SpriteRenderer>();
        sortingGroup = GetComponent<SortingGroup>();

        myCollider = GetComponent<Collider2D>();

        if (editorObject != null)
        {
            editorObjectRenderer = editorObject.GetComponent<SpriteRenderer>();
        }

        if (myRenderer && sprites != null && sprites.Length > 0)
        {
            if(randomSprite)
            {
                currentSpriteIndex = Random.Range(0, sprites.Length);
                myRenderer.sprite = sprites[currentSpriteIndex];
            }
        }
    }

    protected virtual void Start() {
	}

    public int GetSpriteIndex()
    {
        return currentSpriteIndex;
    }

    public Renderer GetRenderer()
    {
        return myRenderer;
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return myRenderer;
    }


    public virtual void EnableCollider(bool b)
    {
        if (myCollider != null)
        {
            myCollider.enabled = b;
        }
    }

    public void UpdateCollider()
    {
        EnableCollider(withCollider);
    }

    bool bSelected = false;

    public bool GetSelect()
    {
        return bSelected;
    }

    public void SelectMe(bool b)
    {
        if (b)
        {
            if (myRenderer != null)
            {
                myRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            }
            if (editorObjectRenderer != null)
            {
                editorObjectRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            }

            bSelected = true;
        }
        else
        {
            if (myRenderer != null)
            {
                myRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            if (editorObjectRenderer != null)
            {
                editorObjectRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }

            bSelected = false;
        }
    }

    public void SelectBlockedMe(bool b)
    {
        if (b)
        {
            if (myRenderer != null)
            {
                myRenderer.material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            }
            if (editorObjectRenderer != null)
            {
                editorObjectRenderer.material.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            }

            bSelected = true;
        }
        else
        {
            if (myRenderer != null)
            {
                myRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            if (editorObjectRenderer != null)
            {
                editorObjectRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }

            bSelected = false;
        }
    }

    public void SelectBlockedMe2(bool b)
    {
        if (b)
        {
            if (myRenderer != null)
            {
                myRenderer.material.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
            }
            if (editorObjectRenderer != null)
            {
                editorObjectRenderer.material.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
            }
        }
        else
        {
            if (myRenderer != null)
            {
                myRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            if (editorObjectRenderer != null)
            {
                editorObjectRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }

    virtual public void SetSortingLayerID(int nID)
    {
        if (myRenderer != null)
        {
            myRenderer.sortingLayerName = "GameLayer_" + (nID + 1);
        }

        if(sortingGroup != null) 
        {
            sortingGroup.sortingLayerName = "GameLayer_" + (nID + 1);
        }

        if (editorObjectRenderer != null)
        {
            editorObjectRenderer.sortingLayerName = "GameLayer_" + (nID + 1);
        }
    }

    virtual public void SetSortingLayerString(string str)
    {
        if (myRenderer != null)
        {
            myRenderer.sortingLayerName = str;
        }

        if (sortingGroup != null) 
        {
            sortingGroup.sortingLayerName = str;
        }

        if (editorObjectRenderer != null)
        {
            editorObjectRenderer.sortingLayerName = str;
        }
    }

    public void SetTopSortingLayer()
    {
        if (myRenderer != null)
        {
            myRenderer.sortingLayerName = "EditorLayer_1";
        }

        if (sortingGroup != null) 
        {
            sortingGroup.sortingLayerName = "EditorLayer_1";
        }

        if (editorObjectRenderer != null)
        {
            editorObjectRenderer.sortingLayerName = "EditorLayer_1";
        }
    }

    public virtual void SetSortingOrder(int sortingOrder) {
        if (myRenderer)
            myRenderer.sortingOrder = sortingOrder;

        if (sortingGroup)
            sortingGroup.sortingOrder = sortingOrder;
    }

    public void ShowEditorObject(bool b)
    {
        if(editorObject != null)
        {
            editorObject.SetActive(b);
        }
    }

    public void ShowGameplayObject(bool b)
    {
        if (gameplayObject != null)
        {
            gameplayObject.SetActive(b);
        }
    }

    // Update the Y position on the grid, as some objects may be moved up (mainly terrain)
    public void UpdatePositionY(bool bTakeOffset)
    {
        int gridY = (int)(transform.position.y + 0.05f);

        if(bTakeOffset)
        {
            transform.position = new Vector3(transform.position.x, gridY + offsetPosY, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, gridY, transform.position.z);
        }

        UpdateSortingOrder();
    }

    public virtual string GetData() {
        return null;
	}

    public virtual void SetData(string data) {

	}
}
