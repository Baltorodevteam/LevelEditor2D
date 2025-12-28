using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyData", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    static float DPS_FACTOR = 1.0f;
    static float SURVIVABILITY_FACTOR = 0.7f;
    static float SPEED_FACTOR = 0.5f;
    static float REACH_FACTOR = 0.3f;

    public int enemyID;
    [HideInInspector]
    public int waveID;
    [HideInInspector]
    public int timeout;

    public int type; // 0 - trash, 1 - elite, 2 - boss

    public int layerIndex;
    public int layerMask; // 1 - onGround, 2 - offGround, 0 - everywhere

    public float baseHp;
    public float baseDamage;
    public float armor;
    public float fireRate;
    public float range;
    public float moveSpeed;

    [HideInInspector]
    public float threatScore;

    [HideInInspector]
    public int areaGridX;
    [HideInInspector]
    public int areaGridY;

    public int areaGridW;
    public int areaGridH;

    public void RecalculateThreat()
    {
        float dps = baseDamage * fireRate;
        float survivability = baseHp * (1f + armor / 100f);
        threatScore = dps * DPS_FACTOR + survivability * SURVIVABILITY_FACTOR + moveSpeed * SPEED_FACTOR + range * SPEED_FACTOR;
    }

    public JsonEnemyDataSerializer ToJson()
    {
        JsonEnemyDataSerializer edj = new JsonEnemyDataSerializer();
        edj.enemyID = enemyID;
        edj.type = type;
        edj.layerIndex = layerIndex;
        edj.layerMask = layerMask;
        edj.baseHp = baseHp;
        edj.baseDamage = baseDamage;
        edj.armor = armor;
        edj.fireRate = fireRate;
        edj.range = range;
        edj.moveSpeed = moveSpeed;
        edj.areaGridW = areaGridW;
        edj.areaGridH = areaGridH;
        
        return edj;
    }

    public void FromJson(JsonEnemyDataSerializer edj)
    {
        enemyID = edj.enemyID;
        type = edj.type;
        layerIndex = edj.layerIndex;
        layerMask = edj.layerMask;
        baseHp = edj.baseHp;
        baseDamage = edj.baseDamage;
        armor = edj.armor;
        fireRate = edj.fireRate;
        range = edj.range;
        moveSpeed = edj.moveSpeed;
        areaGridW = edj.areaGridW;
        areaGridH = edj.areaGridH;
    }

}

public static class EnemyScaler
{
    public static void ApplyScaling(EnemyData stats, DiffCurves curves, float difficulty01, out float hp, out float damage, out float attackSpeed, out float moveSpeed)
    {
        hp = stats.baseHp * curves.EvaluateHP(difficulty01);
        damage = stats.baseDamage * curves.EvaluateDamage(difficulty01);
        attackSpeed = stats.fireRate * curves.EvaluateAttackSpeed(difficulty01);
        moveSpeed = stats.moveSpeed * curves.EvaluateMoveSpeed(difficulty01);
    }
}

