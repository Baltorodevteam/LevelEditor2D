using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class GenerateWorldScreen : BaseScreen
{
    [SerializeField]
    Text numOfLevelsValue;
    [SerializeField]
    Text levelsSizeValuel;
    [SerializeField]
    Text numOfRoomsAtFirstLevelValue;
    [SerializeField]
    Text numOfRoomsAtLastLevelValue;
    [SerializeField]
    Text numOfEnvironmentsValue;
    [SerializeField]
    Text lastGenerationTimeValue;

    int nrOfLevels = 100;
    int levelsSize = 100;
    int numRoomsAtFirstLevel = 120;
    int numRoomsAtLastLevel = 200;
    int numOfEnvironments = 8;

    WorldGeneratorParams wgp = new WorldGeneratorParams();


    private void OnEnable()
    {
        OnEnterScreen(null);

        WorldGenerator.LoadParams("world_generator_params.json");

        nrOfLevels = WorldGenerator.GetParams().nrOfLevels;
        levelsSize = WorldGenerator.GetParams().levelsSize;
        numRoomsAtFirstLevel = WorldGenerator.GetParams().nrOfRoomsAtFirstLevel;
        numRoomsAtLastLevel = WorldGenerator.GetParams().nrOfRoomsAtLastLevel;

        UpdateValues();
    }

    public void OnNrOfLevelsPrev()
    {
        if(nrOfLevels > 1)
        {
            nrOfLevels--;
        }
        UpdateValues();
    }

    public void OnNrOfLevelsNext()
    {
        nrOfLevels++;
        UpdateValues();
    }

    public void OnLevelsSizePrev()
    {
        if (levelsSize > 5)
        {
            levelsSize--;
        }
        UpdateValues();
    }

    public void OnLevelsSizeNext()
    {
        if (levelsSize < 1000)
        {
            levelsSize++;
        }
        UpdateValues();
    }

    public void OnFirstLevelNumRoomsPrev()
    {
        if (numRoomsAtFirstLevel > 1)
        {
            numRoomsAtFirstLevel--;
        }
        UpdateValues();
    }

    public void OnFirstLevelNumRoomsNext()
    {
        if (numRoomsAtFirstLevel < 5000)
        {
            numRoomsAtFirstLevel++;
        }
        UpdateValues();
    }

    public void OnLastLevelNumRoomsPrev()
    {
        if (numRoomsAtLastLevel > 1)
        {
            numRoomsAtLastLevel--;
        }
        UpdateValues();
    }

    public void OnLastLevelNumRoomsNext()
    {
        if (numRoomsAtLastLevel < 5000)
        {
            numRoomsAtLastLevel++;
        }
        UpdateValues();
    }

    public void OnNumOfEnvironmentsPrev()
    {
        if (numOfEnvironments > 1)
        {
            numOfEnvironments--;
        }
        UpdateValues();
    }

    public void OnNumOfEnvironmentsNext()
    {
        if (numOfEnvironments < 100)
        {
            numOfEnvironments++;
        }
        UpdateValues();
    }

    public void OnGenerateWorld()
    {
        WorldGenerator.saveWorld = false;

        WorldGeneratorParams wgp = new WorldGeneratorParams();
        wgp.nrOfLevels = nrOfLevels;
        wgp.levelsSize = levelsSize;
        wgp.nrOfRoomsAtFirstLevel = numRoomsAtFirstLevel;
        wgp.nrOfRoomsAtLastLevel = numRoomsAtLastLevel;
        float fT = WorldGenerator.Generate(wgp);
        //float fT = WorldGenerator.GenerateSegments();
        lastGenerationTimeValue.text = "" + fT + " s";
    }

    public void OnDone()
    {
    }

    void UpdateValues()
    {
        numOfLevelsValue.text = "" + nrOfLevels;
        levelsSizeValuel.text = "" + levelsSize;
        numOfRoomsAtFirstLevelValue.text = "" + numRoomsAtFirstLevel;
        numOfRoomsAtLastLevelValue.text = "" + numRoomsAtLastLevel;
        numOfEnvironmentsValue.text = "" + numOfEnvironments;
    }

    public void OnGenerateTest()
    {
        //StartCoroutine(_OnGenerateTest());
        //StartCoroutine(_OnGenerateFull());
    }
} 
