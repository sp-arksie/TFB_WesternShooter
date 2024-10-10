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
    [Tooltip("At what point in SecondsBetweenShot should the weapon cocking start")]
    [SerializeField][Range(0.1f, 0.9f)] float timeToCockStartPercent = 0.5f;

    public float SecondsBetweenShot { get => secondsBetweenShot; set =>  secondsBetweenShot = value; }
    public float MuzzleVelocity { get => muzzlevelocity; }
    public int BulletsPerShot { get => bulletsPerShot; }
    public bool ApplySpread { get => applySpread; }
    public Vector2 MaxSpread { get => maxSpread; }
    public float TimeToCockStartPercent { get => timeToCockStartPercent; }

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
