using UnityEngine;
using UnityEngine.UI;
using System.Collections;


// Object representing the terrain (consists of 16 (shape) * 3 (visual variants) sprites). PerspectiveOffset specifies the high offset relative to the grid (0.25, 0.5, 0.75)
public class BaseGameTerrainObject : BaseGameObject
{
    [SerializeField]
    float perspectiveOffset = 0;

    Vector2 initColliderOffset;
    Coroutine updateColliderSize;

    [HideInInspector] public int underWaterIndex;
    [HideInInspector] public float underWaterOffset;

    protected override void Start() {
        base.Start();

        if (myCollider != null)
            initColliderOffset = myCollider.offset;
    }

	override public void SetSpriteIndex(int nIndex)
    {
        currentSpriteIndex = nIndex;
    }

    public float GetPerspectiveOffset()
    {
        return perspectiveOffset;
    }

	//public override void EnableCollider(bool b) {
 //       if (myCollider == null)
 //           return;

 //       if (updateColliderSize != null)
 //           StopCoroutine(updateColliderSize);

 //       updateColliderSize = StartCoroutine(UpdateColliderOffset(b));
	//}

 //   IEnumerator UpdateColliderOffset(bool enable) {
	//	myCollider.enabled = true;

	//	var current = myCollider.offset;
	//	var target = initColliderOffset + (enable ? Vector2.zero : Vector2.down);

	//	while (true) {
	//		current = Vector2.MoveTowards(current, target, (Time.deltaTime / PlayerController.CHANGE_LEVEL_TIME));
 //           myCollider.offset = current;

	//		if (current == target)
	//			break;

	//		yield return null;
	//	}

 //       myCollider.offset = initColliderOffset;
 //       //myCollider.enabled = enable;
 //   }

	override public void UpdateEnvironment(EditorData ed)
    {
        if(IsTerrain() == 1)
        {
            myRenderer.sprite = ed.environments[ed.currentEnvironment].terrainLowSprites[currentSpriteIndex];
        }
        else if (IsTerrain() == 2)
        {
            myRenderer.sprite = ed.environments[ed.currentEnvironment].terrainMidSprites[currentSpriteIndex];
        }
        else if (IsTerrain() == 3)
        {
            myRenderer.sprite = ed.environments[ed.currentEnvironment].terrainHiSprites[currentSpriteIndex];
        }
    }
}