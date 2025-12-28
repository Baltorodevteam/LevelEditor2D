using UnityEngine;
using UnityEngine.UI;


// An object representing the lowest layer - the bottom. It consists of two objects, one for when we are above the water, the other for when we are underwater
public class BaseGameBackGroundObject : BaseGameObject
{
    [SerializeField]
    SpriteRenderer secondSpriteRenderer;


    Color col = new Color(1, 1, 1, 1);

    override public void SetSize(float w, float h)
    {
        myRenderer.size = new Vector2(w, h);
        secondSpriteRenderer.size = new Vector2(w, h);
    }

    public void SetBlendValue(float v)
    {
        col.a = 1.0f - v;
        secondSpriteRenderer.color = col;
    }

    override public void UpdateEnvironment(EditorData ed)
    {
        myRenderer.sprite = SystemData.Instance.GetEditorData().GetCurrentEnvironmentData().backgroundSprite;
        secondSpriteRenderer.sprite = SystemData.Instance.GetEditorData().GetCurrentEnvironmentData().backgroundUnderWaterSprite;
    }

}