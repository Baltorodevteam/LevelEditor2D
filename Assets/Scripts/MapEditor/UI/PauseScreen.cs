using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PauseScreen : BaseScreen
{
    [SerializeField]
    Text environmentValue;
    [SerializeField]
    Text musicValue;


    private void OnEnable()
    {
        OnEnterScreen(null);
        UpdateValues();
        JsonWriterReader.Save("save1.sv", SystemData.Instance.GetLevelData());
    }

    public void OnEnvironmentPrev()
    {
        SystemData.Instance.GetEditorData().currentEnvironment--;
        if (SystemData.Instance.GetEditorData().currentEnvironment < 0)
        {
            SystemData.Instance.GetEditorData().currentEnvironment = SystemData.Instance.GetEditorData().environments.Length - 1;
        }
        UpdateValues();
        SystemData.Instance.GetLevelData().UpdateEnvironment(SystemData.Instance.GetEditorData());
    }

    public void OnEnvironmentNext()
    {
        SystemData.Instance.GetEditorData().currentEnvironment++;
        SystemData.Instance.GetEditorData().currentEnvironment %= SystemData.Instance.GetEditorData().environments.Length;
        UpdateValues();
        SystemData.Instance.GetLevelData().UpdateEnvironment(SystemData.Instance.GetEditorData());
    }

    public void OnMusicPrev()
    {
        if (SystemData.Instance.GetEditorData().currentMusic > 0)
        {
            SystemData.Instance.GetEditorData().currentMusic--;
        }
        UpdateValues();
    }

    public void OnMusicNext()
    {
        SystemData.Instance.GetEditorData().currentMusic++;
        UpdateValues();
    }

    void UpdateValues()
    {
        environmentValue.text = "" + (SystemData.Instance.GetEditorData().currentEnvironment + 1);
        musicValue.text = "" + (SystemData.Instance.GetEditorData().currentMusic + 1);
    }

    public void SaveLevelAsSegment()
    {
        LevelInfo li = SystemData.Instance.GetLevelData().ConvertToLevelInfo();
        li.SaveBin("levelAsSegment.bin");
        li.SaveTxt("levelAsSegment.txt");
    }
} 