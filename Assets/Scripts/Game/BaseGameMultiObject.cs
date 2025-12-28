using UnityEngine;
using UnityEngine.UI;


// The object consists of several sub-objects, each of which can have a different sortingOrder
public class BaseGameMultiObject : BaseGameObject
{
    [SerializeField]
    SpriteRenderer[] nextSpriteRenderers;
    [SerializeField]
    float[] nextSpritePivotOffsets;


    override public void UpdateSortingOrder()
    {
        if (myRenderer)
        {
            myRenderer.sortingOrder = GetSortingOrder(transform, pivotOffset);
        }
        for (int i = 0; i < nextSpriteRenderers.Length; i++)
        {
            nextSpriteRenderers[i].sortingLayerName = "GameLayer_" + (GetDefaultLayer() + 1);
            nextSpriteRenderers[i].sortingOrder = GetSortingOrder(nextSpriteRenderers[i].transform, nextSpritePivotOffsets[i]);
        }
    }

    override public void SetColorA(float a)
    {
        Color c = myRenderer.color;
        c.a = a;
        myRenderer.color = c;

        for (int i = 0; i < nextSpriteRenderers.Length; i++)
        {
            c = nextSpriteRenderers[i].color;
            c.a = a;
            nextSpriteRenderers[i].color = c;
        }
    }
}