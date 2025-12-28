using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DifficultyRanges", menuName = "Difficulty Ranges")]
public class DifficultyRangesData : ScriptableObject
{
    [Header("PLAYER — CORE")]
    public RangeFloat playerHP = new RangeFloat(100, 1200);
    public RangeFloat playerDamage = new RangeFloat(10, 250);
    public RangeFloat playerAttackSpeed = new RangeFloat(1.0f, 6f);
    public RangeFloat playerMoveSpeed = new RangeFloat(3.5f, 9f);

    [Header("PLAYER — DEFENSE")]
    [Range(0, 1)] public float maxArmor = 0.7f;
    [Range(0, 1)] public float maxDodge = 0.35f;
    [Range(0, 1)] public float maxCritChance = 0.4f;
    public RangeFloat critMultiplier = new RangeFloat(1.5f, 3f);

    [Header("ENEMY — CORE")]
    public RangeFloat enemyHP_Trash = new RangeFloat(10, 40);
    public RangeFloat enemyHP_Elite = new RangeFloat(80, 300);
    public RangeFloat enemyHP_Boss = new RangeFloat(1000, 20000);

    public RangeFloat enemyDamage = new RangeFloat(5, 200);
    public RangeFloat enemyAttackSpeed = new RangeFloat(0.3f, 3f);
    public RangeFloat enemyMoveSpeed = new RangeFloat(2.5f, 6.5f);
    public RangeFloat enemyAttackRange = new RangeFloat(5.0f, 15.0f);

    [Header("THREAT SCORE")]
    public RangeFloat threatTrash = new RangeFloat(20, 80);
    public RangeFloat threatElite = new RangeFloat(120, 400);
    public RangeFloat threatBoss = new RangeFloat(3000, 15000);

    private void OnValidate()
    {
        playerHP.Clamp();
        playerDamage.Clamp();
        playerAttackSpeed.Clamp();
        playerMoveSpeed.Clamp();

        critMultiplier.Clamp();

        enemyHP_Trash.Clamp();
        enemyHP_Elite.Clamp();
        enemyHP_Boss.Clamp();

        enemyDamage.Clamp();
        enemyAttackSpeed.Clamp();
        enemyMoveSpeed.Clamp();
        enemyAttackRange.Clamp();

        threatTrash.Clamp();
        threatElite.Clamp();
        threatBoss.Clamp();

        maxArmor = Mathf.Clamp01(maxArmor);
        maxDodge = Mathf.Clamp01(maxDodge);
        maxCritChance = Mathf.Clamp01(maxCritChance);
    }
}
