using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;


public class GenerateScreen : BaseScreen
{
    [SerializeField]
    LevelMiniMap levelMiniMap;
    [SerializeField]
    Text environmentValue;
    [SerializeField]
    Text numOfRoomsValue;
    [SerializeField]
    Text minRoomSizeValue;
    [SerializeField]
    Text maxRoomSizeValue;
    [SerializeField]
    Text algorithmNrValue;

    static bool firstRun = true;

    int nrOfRooms = 5;
    int minSizeRoom = 1;
    int maxSizeRoom = 1;
    int algorithmNr = 1;

    LevelGeneratorParams lgp = new LevelGeneratorParams();


    private void OnEnable()
    {
        OnEnterScreen(null);

        if(firstRun)
        {
            LevelGeneratorParams.Load("level_generator_params.json", ref lgp);
            nrOfRooms = lgp.minRooms;
            minSizeRoom = lgp.minRoomSize + 1;
            maxSizeRoom = lgp.maxRoomSize + 1;
            algorithmNr = lgp.algorithmNr;

            OnGenerate();
            UpdateValues();

            firstRun = false;
        }
        else
        {
            levelMiniMap.DrawLevelFromLevelInfoToTexture(SystemData.Instance.GetLevelInfo());
        }
    }

    public void OnEnvironmentPrev()
    {
        SystemData.Instance.GetEditorData().currentEnvironment--;
        if(SystemData.Instance.GetEditorData().currentEnvironment < 0)
        {
            SystemData.Instance.GetEditorData().currentEnvironment = SystemData.Instance.GetEditorData().environments.Length - 1;
        }
        UpdateValues();
    }

    public void OnEnvironmentNext()
    {
        SystemData.Instance.GetEditorData().currentEnvironment++;
        SystemData.Instance.GetEditorData().currentEnvironment %= SystemData.Instance.GetEditorData().environments.Length;
        UpdateValues();
    }

    public void OnRoomPrev()
    {
        if (nrOfRooms > 1)
        {
            nrOfRooms--;
        }
        UpdateValues();
    }

    public void OnRoomNext()
    {
        nrOfRooms++;
        UpdateValues();
    }

    public void OnMinSizeRoomPrev()
    {
        if (minSizeRoom > 1)
        {
            minSizeRoom--;
        }
        UpdateValues();
    }

    public void OnMinSizeRoomNext()
    {
        if (minSizeRoom < LevelGenerator.levelWidth / 3 && minSizeRoom >= maxSizeRoom)
            OnMaxSizeRoomNext();

        if(minSizeRoom < LevelGenerator.levelWidth / 3 && minSizeRoom < maxSizeRoom)
        {
            minSizeRoom++;
        }
        UpdateValues();
    }

    public void OnMaxSizeRoomPrev()
    {
        if (maxSizeRoom <= minSizeRoom)
            OnMinSizeRoomPrev();

        if (maxSizeRoom > 1 && maxSizeRoom > minSizeRoom)
        {
            maxSizeRoom--;
        }
        UpdateValues();
    }

    public void OnMaxSizeRoomNext()
    {
        if (maxSizeRoom < 1 + LevelGenerator.levelWidth / 3)
        {
            maxSizeRoom++;
        }
        UpdateValues();
    }

    public void OnAlgorithmPrev()
    {
        if (algorithmNr > 1)
        {
            algorithmNr--;
        }
        UpdateValues();
    }

    public void OnAlgorithmNext()
    {
        if (algorithmNr < 2)
        {
            algorithmNr++;
        }
        UpdateValues();
    }

    public void OnGenerate()
    {
        lgp.levelWidth = LevelGenerator.levelWidth;
        lgp.levelHeight = LevelGenerator.levelHeight;
        lgp.minRooms = nrOfRooms;
        lgp.maxRooms = nrOfRooms;
        lgp.minRoomSize = minSizeRoom - 1;
        lgp.maxRoomSize = maxSizeRoom - 1;
        lgp.algorithmNr = algorithmNr;

        //LevelGeneratorParams.Save("level_generator_params.json", lgp);

        float t = LevelGenerator.Generate(lgp);
        SystemData.Instance.CreateLevelInfoFromLevelGenerator();

        levelMiniMap.DrawLevelFromLevelInfoToTexture(SystemData.Instance.GetLevelInfo());
    }

    public void OnGenerateWorld()
    {
        WorldGenerator.saveWorld = false;
        float fT = WorldGenerator.GenerateSegments();
    }

    public void OnLoad(BaseScreen nextScreen)
    {
        nextScreen.PreviousScreen = this;
        LoadScreen.fileExtension = "LVL";
        gameObject.SetActive(false);
        nextScreen.gameObject.SetActive(true);
    }

    public void OnSave(BaseScreen nextScreen)
    {
        nextScreen.PreviousScreen = this;
        gameObject.SetActive(false);
        nextScreen.gameObject.SetActive(true);
    }

    public void OnDone(BaseScreen nextScreen)
    {
        StartCoroutine(CreateLevel(nextScreen));
    }

    IEnumerator CreateLevel(BaseScreen nextScreen)
    {
        SystemData.Instance.NewLevelData();
        yield return new WaitForEndOfFrame();
        SystemData.Instance.GetLevelData().CreateFromLevelInfo(SystemData.Instance.GetLevelInfo());
        yield return new WaitForEndOfFrame();

        nextScreen.PreviousScreen = this;
        gameObject.SetActive(false);
        nextScreen.gameObject.SetActive(true);
    }

    void UpdateValues()
    {
        environmentValue.text = "" + (SystemData.Instance.GetEditorData().currentEnvironment + 1);
        numOfRoomsValue.text = "" + nrOfRooms;
        minRoomSizeValue.text = "" + minSizeRoom;
        maxRoomSizeValue.text = "" + maxSizeRoom;
        algorithmNrValue.text = "" + algorithmNr;
    }
}

