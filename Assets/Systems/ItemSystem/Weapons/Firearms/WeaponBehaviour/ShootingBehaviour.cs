using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingBehaviour : MonoBehaviour
{
    [SerializeField] float secondsBetweenShot = 1f;
    [SerializeField] float muzzlevelocity = 60f;
    [SerializeField] int bulletsPerShot = 1;
    [SerializeField] bool applySpread = false;
    [SerializeField] Vector2 maxSpread = Vector2.zero;

    public float SecondsBetweenShot { get => secondsBetweenShot; set =>  secondsBetweenShot = value; }
    public float MuzzleVelocity { get => muzzlevelocity; }
    public int BulletsPerShot { get => bulletsPerShot; }
    public bool ApplySpread { get => applySpread; }
    public Vector2 MaxSpread { get => maxSpread; }

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
}
