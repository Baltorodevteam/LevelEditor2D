using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemData : MonoBehaviour
{
    static public SystemData Instance;

    public static int NR_OF_LAYERS = 10;

    [SerializeField]
    EditorData editorData;

    [SerializeField]
    EnemySpawnerSystem enemySpawnerSystem;

    // editor or gameplay...
    static public bool editorMode = true;

    static public bool startCurrentRoom = true;
    static public bool backFromTest = false;


    LevelData levelData = null;
    LevelInfo levelInfo = null;

    public LevelData GetLevelData()
    {
        return levelData;
    }

    public void CreateLevelInfoFromLevelGenerator()
    {
        levelInfo = LevelGenerator.ToLevelInfo();
    }


    public LevelInfo GetLevelInfo()
    {
        return levelInfo;
    }

    public EditorData GetEditorData()
    {
        return editorData;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }


    public void NewLevelData()
    {
        if (levelData != null)
        {
            levelData.Clear();
        }

        levelData = new LevelData();
    }
}