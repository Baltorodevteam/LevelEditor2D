using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class WorldGenerator
{
    public static bool saveWorld = false;

    static WorldGeneratorParams generatorParams = new WorldGeneratorParams();


    public static void LoadParams(string fileName)
    {
        WorldGeneratorParams.Load(fileName, ref generatorParams);
    }

    public static WorldGeneratorParams GetParams()
    {
        return generatorParams;
    }

    public static float Generate(WorldGeneratorParams wgp)
    {
        if(wgp == null)
        {
            wgp = generatorParams;
        }

        float startT = Time.realtimeSinceStartup;

        LevelGeneratorParams lgp = new LevelGeneratorParams();
        lgp.levelWidth = wgp.levelsSize;
        lgp.levelHeight = wgp.levelsSize;
        lgp.minRoomSize = LevelGenerator.GetParams().minRoomSize;
        lgp.maxRoomSize = LevelGenerator.GetParams().maxRoomSize;
        lgp.roomSizePercentTab = LevelGenerator.GetParams().roomSizePercentTab;

        float fWsp = (float)(wgp.nrOfRoomsAtLastLevel - wgp.nrOfRoomsAtFirstLevel) / (float)wgp.nrOfLevels;
        float fNrOfRooms = wgp.nrOfRoomsAtFirstLevel;

        float startDiff = wgp.startDiff;
        float endDiff = wgp.endDiff;
        float addDiff = (endDiff - startDiff) / (float)(wgp.nrOfLevels + 1);

        for (int i = 0; i < wgp.nrOfLevels; i++)
        {
            lgp.minRooms = (int)fNrOfRooms;
            lgp.maxRooms = lgp.minRooms;

            lgp.startDiff = startDiff + (float)i * addDiff;
            lgp.endDiff = startDiff + (float)(i + 1) * addDiff;

            LevelGenerator.Generate(lgp);
            LevelInfo li = LevelGenerator.ToLevelInfo();
            string path = Path.Combine(Application.persistentDataPath, "LEVEL_" + (i + 1) + ".LVL");
            JsonWriterReader.Save(path, li);

            fNrOfRooms += fWsp;

            if(saveWorld)
            {
                LevelGenerator.SaveTxt("level_" + (i + 1) + ".txt");
                LevelGenerator.SaveBitmap("level_" + (i + 1) + ".png");
            }
        }

        Debug.Log("World generation time: " + (Time.realtimeSinceStartup - startT));
        return Time.realtimeSinceStartup - startT;
    }

    public static float GenerateSegments()
    {
        float startT = Time.realtimeSinceStartup;

        generatorParams.nrOfLevels = 30;
        generatorParams.nrOfRoomsAtFirstLevel = 4;
        generatorParams.nrOfRoomsAtLastLevel = 7;

        LevelGeneratorParams lgp = new LevelGeneratorParams();
        lgp.levelWidth = 12;
        lgp.levelHeight = 12;
        lgp.minRoomSize = 0;
        lgp.maxRoomSize = 2;
        lgp.roomSizePercentTab = null;

        float fWsp = (float)(generatorParams.nrOfRoomsAtLastLevel - generatorParams.nrOfRoomsAtFirstLevel) / (float)generatorParams.nrOfLevels;
        float fNrOfRooms = generatorParams.nrOfRoomsAtFirstLevel;

        for (int i = 0; i < generatorParams.nrOfLevels; i++)
        {
            lgp.minRooms = (int)fNrOfRooms;
            lgp.maxRooms = lgp.minRooms;

            LevelGenerator.Generate(lgp);

            fNrOfRooms += fWsp;

            if (saveWorld)
            {
                LevelInfo li = LevelGenerator.ToLevelInfo();
                li.ConvertToSegment();
                li.SaveBitmap("segment_" + (i + 1) + ".png");
                li.SaveBin("segment_" + (i + 1) + ".bin");
                li.SaveTxt("segment_" + (i + 1) + ".txt");
            }
        }

        Debug.Log("World generation time: " + (Time.realtimeSinceStartup - startT));
        return Time.realtimeSinceStartup - startT;
    }
}