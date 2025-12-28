using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class SerializableKeyframe
{
    public float time;
    public float value;
    public float inTangent;
    public float outTangent;
    public float inWeight;
    public float outWeight;
    public WeightedMode weightedMode;

    public SerializableKeyframe() { }

    public SerializableKeyframe(Keyframe k)
    {
        time = k.time;
        value = k.value;
        inTangent = k.inTangent;
        outTangent = k.outTangent;
        inWeight = k.inWeight;
        outWeight = k.outWeight;
        weightedMode = k.weightedMode;
    }

    public Keyframe ToKeyframe()
    {
        Keyframe k = new Keyframe(time, value, inTangent, outTangent)
        {
            inWeight = inWeight,
            outWeight = outWeight,
            weightedMode = weightedMode
        };
        return k;
    }
}

[System.Serializable]
public class SerializableAnimationCurve
{
    public SerializableKeyframe[] keys;

    public SerializableAnimationCurve() { }

    public SerializableAnimationCurve(AnimationCurve curve)
    {
        keys = new SerializableKeyframe[curve.keys.Length];
        for (int i = 0; i < curve.keys.Length; i++)
        {
            keys[i] = new SerializableKeyframe(curve.keys[i]);
        }
    }

    public AnimationCurve ToAnimationCurve()
    {
        Keyframe[] unityKeys = new Keyframe[keys.Length];
        for (int i = 0; i < keys.Length; i++)
        {
            unityKeys[i] = keys[i].ToKeyframe();
        }

        return new AnimationCurve(unityKeys);
    }
}

[System.Serializable]
public class SerializableCurve
{
    public SerializableAnimationCurve curve;
}



[CreateAssetMenu(fileName = "newCurveData", menuName = "CurveData")]
public class CurveData : ScriptableObject
{
    [SerializeField]
    AnimationCurve[] presetCurves;

    AnimationCurve curve;

    public void SetCurve(int presetIndex)
    {
        if(presetIndex >= 0 && presetIndex < presetCurves.Length)
        {
            curve = presetCurves[presetIndex];
        }
    }

    [ContextMenu("Save All Presets")]
    public void SaveAllPresets()
    {
        for(int i = 0; i < presetCurves.Length; i++)
        {
            SetCurve(i);
            SaveCurve("preset_" + (i + 1) + ".crv");
        }
    }

    public void LoadCurve(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string jsonContent = File.ReadAllText(path);

        SerializableCurve loaded = JsonUtility.FromJson<SerializableCurve>(jsonContent);
        if(loaded != null)
        {
            AnimationCurve ac = loaded.curve.ToAnimationCurve();
            curve = ac;
        }
    }

    public void SaveCurve(string fileName)
    {
        AnimationCurve newCurve = curve;// AnimationCurve.EaseInOut(0, 0, 1, 1);
        SerializableCurve sc = new SerializableCurve
        {
            curve = new SerializableAnimationCurve(newCurve)
        };

        string jsonContent = JsonUtility.ToJson(sc, true);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, jsonContent);
    }

    float Evaluate(float t)
    {
        return curve.Evaluate(t);
    }
}
