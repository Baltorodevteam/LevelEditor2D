using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Objects with a shader that can be switched between displaying a texture and a solid color (for underwater terrain objects)
public class BaseGameBlendingColorTerrainObject : BaseGameObject
{
    private MaterialPropertyBlock m_PropertyBlock;

    private Color32 blendingColor = new Color32();
    private float blendingValue = 1;

    Vector2 initColliderOffset;
    Coroutine updateColliderSize;

    override public void UpdateEnvironment(EditorData ed)
    {
        myRenderer.sprite = ed.environments[ed.currentEnvironment].terrainUnderWaterSprites[currentSpriteIndex];

        blendingColor = ed.environments[ed.currentEnvironment].underWaterBlendColor;

        SetBlendingColor(blendingColor.r, blendingColor.g, blendingColor.b, blendingColor.a, true);
    }

    public void SetBlendingColor(byte r, byte g, byte b, byte a, bool updateNow)
    {
        blendingColor.r = r;
        blendingColor.g = g;
        blendingColor.b = b;
        blendingColor.a = a;

        if (updateNow)
        {
            m_PropertyBlock.SetColor("_SpriteColor", blendingColor);
            myRenderer.SetPropertyBlock(m_PropertyBlock);
        }
    }

    public void SetBlendingValue(float v, bool updateNow)
    {
        blendingValue = v;

        if (updateNow)
        {
            m_PropertyBlock.SetFloat("_BlendingValue", blendingValue);
            myRenderer.SetPropertyBlock(m_PropertyBlock);
        }
    }

    override public void SetSpriteIndex(int nIndex)
    {
        if (myRenderer)
        {
            currentSpriteIndex = nIndex;
            myRenderer.sprite = SystemData.Instance.GetEditorData().GetCurrentEnvironmentData().terrainUnderWaterSprites[nIndex];
        }
    }

  //  public override void EnableCollider(bool b) {
  //      if (myCollider == null)
  //          return;

  //      if (updateColliderSize != null)
  //          StopCoroutine(updateColliderSize);

  //      updateColliderSize = StartCoroutine(UpdateColliderOffset(b));
  //  }

  //  IEnumerator UpdateColliderOffset(bool enable) {
		//myCollider.enabled = true;

  //      var current = myCollider.offset;
  //      var target = initColliderOffset + (enable ? Vector2.zero : Vector2.up);

  //      while (true) {
  //          current = Vector2.MoveTowards(current, target, (Time.deltaTime / PlayerController.CHANGE_LEVEL_TIME));
  //          myCollider.offset = current;

  //          if (current == target)
  //              break;

  //          yield return null;
  //      }

  //      myCollider.offset = initColliderOffset;
  //      //myCollider.enabled = enable;
  //  }


    protected override void Awake()
    {
        base.Awake();

        if (editorObject != null)
        {
            editorObjectRenderer = editorObject.GetComponent<SpriteRenderer>();
        }

        m_PropertyBlock = new MaterialPropertyBlock();

        SetBlendingColor(0x75, 0x88, 0xC8, 0xff, false);
    }

    protected override void Start() {
        base.Start();

        if (myCollider != null)
            initColliderOffset = myCollider.offset;

        m_PropertyBlock.SetTexture("_MainTex", myRenderer.sprite.texture);
        m_PropertyBlock.SetColor("_SpriteColor", blendingColor);
        m_PropertyBlock.SetFloat("_BlendingValue", 1.0f);
        myRenderer.SetPropertyBlock(m_PropertyBlock);
    }

    public void UpdateProperty()
    {
        m_PropertyBlock.SetTexture("_MainTex", myRenderer.sprite.texture);
        m_PropertyBlock.SetColor("_SpriteColor", blendingColor);
        m_PropertyBlock.SetFloat("_BlendingValue", blendingValue);
        myRenderer.SetPropertyBlock(m_PropertyBlock);
    }
}