using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Video;

public class ProjectileWeapon : WeaponBase
{
    [Header("Ammo Stats")]
    [SerializeField] int magazineSize;
    [SerializeField] WeaponCalliber weaponCalliber;

    [Header("Firing Stats")]
    [SerializeField] float secondsBetweenShot = 1f;
    [SerializeField] float muzzleVelocity = 30f;

    [Header("VFX")]
    [SerializeField] Transform muzzleFlashParent;
    [SerializeField] Transform smokeEmissionPointsParent;
    [SerializeField] GameObject muzzleFlashPrefab;
    [SerializeField] GameObject gunSmokePrefab;

    [Header("Animation")]
    [SerializeField] Transform adsPositionTransform;
    [SerializeField] Transform hipPositionTransform;
    [SerializeField] Transform shootingRotationTransform;
    [SerializeField] Transform baseRotationTransform;
    public Transform AdsPositionTransform { get { return adsPositionTransform; } private set { adsPositionTransform = value; } }
    public Transform HipPositionTransform { get { return hipPositionTransform; } private set { hipPositionTransform = value; } }
    public Transform ShootingRotationTransform { get { return shootingRotationTransform; } private set { shootingRotationTransform = value; } }
    public Transform BaseRotationTransform { get { return baseRotationTransform; } private set { baseRotationTransform = value; } }

    public event Action ShootEvent;
    public event Action SelectedEvent;


    int currentBulletsInMagazine = 1;
    float timeOfLastShot = -10f;
    
    Barrel barrel;
    Animator animator;
    int isAiming;
    int isShooting;

    #region DEBUG

    [Header("Debug")]
    [SerializeField] bool debugShoot = false;

    private void OnValidate()
    {
        if (debugShoot)
        {
            debugShoot = false;
            NotifyAttack();
        }
    }

    #endregion

    private void Awake()
    {
        barrel = GetComponentInChildren<Barrel>();
    }

    private void Start()
    {
        isAiming = Animator.StringToHash("IsAiming");
        isShooting = Animator.StringToHash("IsShooting");
    }

    internal override void NotifyAttack()
    {
        Shoot();
    }

    internal override void NotifySelected()
    {
        SelectedEvent?.Invoke();
    }

    internal override void NotifyUnselected()
    {
        // NOOP
    }

    private void Shoot()
    {
        barrel.Shoot(muzzleVelocity);
        ShootEvent?.Invoke();
        InstantiateParticles();
    }

    public bool CanShoot()
    {
        bool canShoot = false;

        if (barrel != null)
        {
            float difference = Time.time - timeOfLastShot;
            if (difference >= secondsBetweenShot && currentBulletsInMagazine > 0)
            {
                canShoot = true;
                timeOfLastShot = Time.time;
            }
        }
        else
            throw new System.Exception("No barrel attached, or incorrectly placed.");

        return canShoot;
    }

    private void InstantiateParticles()
    {
        if (muzzleFlashPrefab && muzzleFlashParent)
        {
            Instantiate(muzzleFlashPrefab, muzzleFlashParent);
        }
        if (gunSmokePrefab && smokeEmissionPointsParent)
        {
            for (int i = 0; i < smokeEmissionPointsParent.childCount; i++)
            {
                Instantiate(gunSmokePrefab, smokeEmissionPointsParent.GetChild(i));
            }
        }
    }

    public void NotifyIsAiming(bool value)
    {
        animator.SetBool(isAiming, value);
    }

    public void NotifyReload()
    {
        currentBulletsInMagazine++;
    }
}
