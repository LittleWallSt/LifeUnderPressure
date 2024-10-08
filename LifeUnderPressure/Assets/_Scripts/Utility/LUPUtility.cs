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

    public MovementVector (float forward, float side, float backward, float upward)
    {
        this.forward = forward;
        this.side = side;
        this.backward = backward;
        this.upward = upward;
    }

}
[Serializable]
public struct ScannerStruct
{
    public float scanTimer;
    public float scanAnimationSpeed;
    public float depletingSpeed;
}
