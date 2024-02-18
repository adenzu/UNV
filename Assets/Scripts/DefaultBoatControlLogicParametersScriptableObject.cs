using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default DefaultBoatControlLogic Parameters", menuName = "UNV/Default DefaultBoatControlLogic Parameters", order = 1)]
public class DefaultBoatControlLogicParametersScriptableObject : BoatControlLogicParametersScriptableObject
{
    public float steerCoefficient = 1f;
    public DefaultBoatControlLogic.TargetOvershootOptions onTargetOvershoot = DefaultBoatControlLogic.TargetOvershootOptions.DoNothing;
    public bool useBezierPath = false;
    public int bezierPathSampleCount = 10;
    public float colregDetectionRadius;
    public float colregAvoidanceRadius;
    public float colregCooldown = 0.1f;
    public bool showColregDetectorGizmos = true;
}
