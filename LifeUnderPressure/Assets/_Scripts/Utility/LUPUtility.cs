using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LUPUtility
{
    
}
[Serializable]
public struct MovementVector
{
    public float forward;
    public float side;
    public float backward;
    public float upward;
}
[Serializable]
public struct ScannerStruct
{
    public float scanTimer;
    public float scanAnimationSpeed;
    public float depletingSpeed;
}
