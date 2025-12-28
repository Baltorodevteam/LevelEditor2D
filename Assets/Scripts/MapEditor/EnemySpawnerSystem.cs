using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class EnemySpawnerSystem : MonoBehaviour
{
    public static EnemySpawnerSystem Instance = null;

    public PlayerData playerData;
    public List<EnemyData> enemyPool;
    public DiffCurves difficultyCurves;
    public DifficultyRangesData ranges;

    [Range(0f, 1f)]
    public float difficulty01;

    public float difficultyMultiplier = 1.3f;


    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        LoadEnemiesData();
    }


    // type: 0 - trash, 1 - elite, 2 - boss
    public EnemyData GenerateEnemy(int id, int type)
    {
        EnemyData ed =  new EnemyData
        {
            enemyID = id,
            baseDamage = ranges.enemyDamage.Random(),
            fireRate = ranges.enemyAttackSpeed.Random(),
            moveSpeed = ranges.enemyMoveSpeed.Random(),
            range = ranges.enemyAttackRange.Random(),
        };

        if(type == 0)
        {
            ed.baseHp = ranges.enemyHP_Trash.Random();
        }
        else if(type == 1)
        {
            ed.baseHp = ranges.enemyHP_Elite.Random();
        }
        else
        {
            ed.baseHp = ranges.enemyHP_Boss.Random();
        }

        return ed;
    }

    [ContextMenu("Generate and Save Enemies Data")]
    public void GenerateAndSaveEnemiesData()
    {
        enemyPool = new List<EnemyData>();

        for (int id = 2000; id <= 2010; id++)
        {
            EnemyData ed = GenerateEnemy(id, 0);
            enemyPool.Add(ed);
        }
        for (int id = 2011; id <= 2020; id++)
        {
            EnemyData ed = GenerateEnemy(id, 1);
            enemyPool.Add(ed);
        }
        for (int id = 2021; id <= 2025; id++)
        {
            EnemyData ed = GenerateEnemy(id, 2);
            enemyPool.Add(ed);
        }

        for(int i = 0; i < enemyPool.Count; i++)
        {
            JsonEnemyDataSerializer edj = enemyPool[i].ToJson();
            string jsonContent = JsonUtility.ToJson(edj, true);
            string path = Path.Combine(Application.persistentDataPath, "enemy_" + enemyPool[i].enemyID + ".enm");
            
            File.WriteAllText(path, jsonContent);
        }
    }

    public void LoadEnemiesData()
    {
        enemyPool = new List<EnemyData>();

        string path = Application.persistentDataPath;
        string[] jsonFiles = Directory.GetFiles(path, "*.enm");

        foreach (var file in jsonFiles)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, file);
            string jsonContent = File.ReadAllText(fullPath);

            JsonEnemyDataSerializer edj = JsonUtility.FromJson<JsonEnemyDataSerializer>(jsonContent);
            if (edj != null)
            {
                EnemyData ed = new EnemyData();
                ed.FromJson(edj);
                enemyPool.Add(ed);
            }
        }
    }

    public void PreparePlayerData(float diff01)
    {
        playerData = new PlayerData();
        playerData.hp = Mathf.Lerp(ranges.playerHP.min, ranges.playerHP.max, diff01);
        playerData.weaponDamage = Mathf.Lerp(ranges.playerDamage.min, ranges.playerDamage.max, diff01);
        playerData.fireRate = Mathf.Lerp(ranges.playerAttackSpeed.min, ranges.playerAttackSpeed.max, diff01);
        playerData.moveSpeed = Mathf.Lerp(ranges.playerMoveSpeed.min, ranges.playerMoveSpeed.max, diff01);
        playerData.armor = Mathf.Lerp(0, ranges.maxArmor, diff01);
    }

    public void PrepareEnemies()
    {
        for (int i = 0; i < enemyPool.Count; i++)
        {
            enemyPool[i].RecalculateThreat();
        }
    }

    public List<EnemyData> GenerateWave(PlayerData player, bool[] availableTypes)
    {
        float playerPower = player.Calculate();
        //difficultyMultiplier *= Mathf.Lerp(0.9f, 1.1f, playerDeathsRatio);
        float levelBudget = playerPower * difficultyMultiplier;

        if(availableTypes[2]) // needed boss
        {
            levelBudget *= 2;
        }
        
        levelBudget *= difficultyCurves.spawnCurve.Evaluate(difficulty01);

        List<EnemyData> spawned = new List<EnemyData>();
        List<EnemyData> candidates = new List<EnemyData>();

        while (levelBudget > 0)
        {
            candidates.Clear();
            foreach (var e in enemyPool)
            {
                if(e.threatScore <= levelBudget /*&& e.threatScore >= levelBudget / 20*/)
                {
                    if(availableTypes[e.type])
                    {
                        candidates.Add(e);
                    }
                }
            }

            if (candidates.Count == 0)
                break;

            EnemyData chosen = candidates[Random.Range(0, candidates.Count)];
            spawned.Add(chosen);
            levelBudget -= chosen.threatScore;

            if(availableTypes[2])
            {
                break;
            }
        }

        Debug.Log("Spawned " + spawned.Count + "  enemies...");

        return spawned;
    }
}
