using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnvironment", menuName = "EnvironmentData")]
public class EnvironmentData : ScriptableObject
{
    [Header("Background")]
    public Sprite backgroundSprite;
    public Sprite backgroundUnderWaterSprite;

    [Header("Water")]
    public Color waterSurfaceColor;
    public Color waterSurfaceSpecialColor;
    public Sprite[] foamSprites;

    [Header("Terrain")]
    public Sprite[] terrainLowSprites;
    public Sprite[] terrainMidSprites;
    public Sprite[] terrainHiSprites;

    [Header("Underwater terrain")]
    public Sprite[] terrainUnderWaterSprites;
    public Color underWaterBlendColor;

}
