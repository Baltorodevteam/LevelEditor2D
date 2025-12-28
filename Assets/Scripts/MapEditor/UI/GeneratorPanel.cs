using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;


public class GeneratorPanel : MonoBehaviour
{
    [SerializeField]
    Text[] texts;

    int randomStartPercentValue = 20;
    int tresholdIfEmpty = 4;
    int tresholdIfTaken = 5;
    int finalExpectedPercentValue = 30;
    int maxIterations = 5;


    private void OnEnable()
    {
        //OnEnterScreen(null);
        //OnGenerate();
        UpdateValues();
    }

    public void OnRandomStartPercentValuePrev()
    {
        if(randomStartPercentValue > 1)
        {
            randomStartPercentValue--;
        }
        UpdateValues();
    }

    public void OnRandomStartPercentValueNext()
    {
        if (randomStartPercentValue < 100)
        {
            randomStartPercentValue++;
        }
        UpdateValues();
    }

    public void OnTresholdIfEmptyPrev()
    {
        if (tresholdIfEmpty > 1)
        {
            tresholdIfEmpty--;
        }
        UpdateValues();
    }

    public void OnTresholdIfEmptyNext()
    {
        if (tresholdIfEmpty < 8)
        {
            tresholdIfEmpty++;
        }
        UpdateValues();
    }

    public void OnTresholdIfTakenPrev()
    {
        if (tresholdIfTaken > 1)
        {
            tresholdIfTaken--;
        }
        UpdateValues();
    }

    public void OnTresholdIfTakenNext()
    {
        if (tresholdIfTaken < 8)
        {
            tresholdIfTaken++;
        }
        UpdateValues();
    }

    public void OnFinalExpectedPercentValuePrev()
    {
        if (finalExpectedPercentValue > 1)
        {
            finalExpectedPercentValue--;
        }
        UpdateValues();
    }

    public void OnFinalExpectedPercentValueNext()
    {
        if (finalExpectedPercentValue < 100)
        {
            finalExpectedPercentValue++;
        }
        UpdateValues();
    }

    public void OnMaxIterationsPrev()
    {
        if (maxIterations > 0)
        {
            maxIterations--;
        }
        UpdateValues();
    }

    public void OnMaxIterationsNext()
    {
        if (maxIterations < 1000)
        {
            maxIterations++;
        }
        UpdateValues();
    }

    public void OnGenerate()
    {
        GameObject to = SystemData.Instance.GetEditorData().objectCategories[1].objects[1];
        BaseGameTerrainObject bgto = to.GetComponent<BaseGameTerrainObject>();

        AreaGenerator.width = Grid.gridWidth;
        AreaGenerator.height = Grid.gridHeight;
        AreaGenerator ag = new AreaGenerator();
        ag.GenerateDefault();

        Terrain3DShapeGenerator.CreateTerrainFromAreaGenerator(ag, bgto, SystemData.Instance.GetLevelData().GetCurrentRoom(), 3);
        for (int i = 0; i < SystemData.NR_OF_LAYERS; i++)
        {
            Terrain3DShapeGenerator.CreateFromAreaGenerator(ag, SystemData.Instance.GetLevelData().GetCurrentRoom(), i);
        }
    }

    public void OnAutoGenerate()
    {
        for (int l = 1; l < SystemData.NR_OF_LAYERS; l++)
        {
            SystemData.Instance.GetLevelData().GetCurrentRoom().GetLayerByIndex(l).ClearList();
        }

        GameObject to = SystemData.Instance.GetEditorData().objectCategories[1].objects[1];
        BaseGameTerrainObject bgto = to.GetComponent<BaseGameTerrainObject>();

        AreaGenerator ag = new AreaGenerator();
        ag.AutoRun(SystemData.Instance.GetLevelData(), SystemData.Instance.GetLevelData().GetCurrentRoom());

        Terrain3DShapeGenerator.CreateTerrainFromAreaGenerator(ag, bgto, SystemData.Instance.GetLevelData().GetCurrentRoom(), 3);
        for (int i = 0; i < SystemData.NR_OF_LAYERS; i++)
        {
            Terrain3DShapeGenerator.CreateFromAreaGenerator(ag, SystemData.Instance.GetLevelData().GetCurrentRoom(), i);
        }
    }

    void UpdateValues()
    {
        texts[0].text = "" + randomStartPercentValue;
        texts[1].text = "" + tresholdIfEmpty;
        texts[2].text = "" + tresholdIfTaken;
        texts[3].text = "" + finalExpectedPercentValue;
        texts[4].text = "" + maxIterations;
    }
} 