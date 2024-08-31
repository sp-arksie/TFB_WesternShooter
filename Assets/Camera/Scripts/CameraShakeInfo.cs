using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeInfo
{
    public float strength;
    public Vector2 direction;
    public float speed;
    public float recoveryTime;
    public float recoveryStartDelay;
    public float recoveryPercent;

    public CameraShakeInfo(float strength, Vector3 direction, float speed, float recoveryTime, float recoveryStartDelay, float recoveryPercent)
    {
        this.strength = strength;
        this.direction = direction;
        this.speed = speed;
        this.recoveryTime = recoveryTime;
        this.recoveryStartDelay = recoveryStartDelay;
        this.recoveryPercent = recoveryPercent;
    }
}
