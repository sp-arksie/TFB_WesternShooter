using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingBehaviour : MonoBehaviour
{
    [SerializeField] float secondsBetweenShot = 1f;
    [SerializeField] float muzzlevelocity = 60f;

    float timeSinceLastShot = -100;

    public bool GetCanShoot()
    {
        bool canShoot = false;

        if( (Time.time - timeSinceLastShot) > secondsBetweenShot)
        {
            canShoot = true;
            timeSinceLastShot = Time.time;
        }

        return canShoot;
    }

    public float GetMuzzleVelocity() { return muzzlevelocity; }
}
