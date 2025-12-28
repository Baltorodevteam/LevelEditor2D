using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newDifficultyScalingCurves", menuName = "Difficulty Scaling Curves")]
public class DiffCurves : ScriptableObject
{
    [Header("INPUT")]
    [Tooltip("X = difficulty (0–1), Y = multiplier")]
    public AnimationCurve hpCurve = AnimationCurve.EaseInOut(0, 1, 1, 3);
    public AnimationCurve damageCurve = AnimationCurve.EaseInOut(0, 1, 1, 2);
    public AnimationCurve attackSpeedCurve = AnimationCurve.Linear(0, 1, 1, 1.5f);
    public AnimationCurve moveSpeedCurve = AnimationCurve.Linear(0, 1, 1, 1.2f);

    public AnimationCurve spawnCurve;

    public float EvaluateHP(float difficulty01) =>
        hpCurve.Evaluate(difficulty01);

    public float EvaluateDamage(float difficulty01) =>
        damageCurve.Evaluate(difficulty01);

    public float EvaluateAttackSpeed(float difficulty01) =>
        attackSpeedCurve.Evaluate(difficulty01);

    public float EvaluateMoveSpeed(float difficulty01) =>
        moveSpeedCurve.Evaluate(difficulty01);

    public float EvaluateSpawn(float difficulty01) =>
        spawnCurve.Evaluate(difficulty01);
}
