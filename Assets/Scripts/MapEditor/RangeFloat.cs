using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct RangeFloat
{
    public float min;
    public float max;

    public RangeFloat(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float Random() => UnityEngine.Random.Range(min, max);

    public float ClampValue(float value) => Mathf.Clamp(value, min, max);

    public void Clamp()
    {
        if (min > max)
        {
            (min, max) = (max, min);
        }
    }
}
