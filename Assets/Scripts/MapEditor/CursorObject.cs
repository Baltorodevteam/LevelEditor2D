 using UnityEngine;
 
public class CursorObject : MonoBehaviour
{
    const int eCursor_Normal = 0;
    const int eCursor_Rubber = 1;

    [SerializeField]
    Sprite[] sprites;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void SetCursor_Normal()
    {
        spriteRenderer.sprite = sprites[eCursor_Normal];
    }

    public void SetCursor_Rubber()
    {
        spriteRenderer.sprite = sprites[eCursor_Rubber];
    }

}