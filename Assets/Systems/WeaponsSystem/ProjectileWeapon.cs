using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Video;

public class ProjectileWeapon : WeaponBase
{
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

    ShootingBehaviour shootingBehaviour;
    AmmoBehaviour ammoBehaviour;
    DamageBehaviour damageBehaviour;
    
    Barrel barrel;

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
        shootingBehaviour = GetComponent<ShootingBehaviour>();
        ammoBehaviour = GetComponent<AmmoBehaviour>();
        damageBehaviour = GetComponent<DamageBehaviour>();
    }

    private void Start()
    {

    }

    internal override void NotifyAttack()
    {
        if(CanShoot()) Shoot();
    }

    internal override void NotifySelected()
    {
        SelectedEvent?.Invoke();
    }

    internal override void NotifyUnselected()
    {
        // NOOP
    }

    internal void NotifyReload()
    {
        if (CanReload()) Reload();
    }
    public bool CanShoot()
    {
        return ammoBehaviour.GetHasBulletInMagazine() && shootingBehaviour.GetCanShoot();
    }

    private void Shoot()
    {
        barrel.Shoot(shootingBehaviour.GetMuzzleVelocity(), GetHitInfoToSend());
        ShootEvent?.Invoke();
        InstantiateParticles();
    }

    private HitInfo GetHitInfoToSend()
    {
        HitInfo hitInfo = new();
        hitInfo.baseDamage = damageBehaviour.GetBaseDamage();
        hitInfo.locationOfDamageSource = transform.position;
        hitInfo.weaponCalliber = damageBehaviour.GetWeaponCalliber();

        return hitInfo;
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

    private bool CanReload()
    {
        return ammoBehaviour.CanReload();
    }

    private void Reload()
    {
        //  TODO: some kind of delay for each reload (1 reload = 1 bullet -for example-, and is on a loop > reload finished > start new reload)
        //  => reload started? have to wait for finish (no shooting, item switching etc). + global reload can be canceled


    }
}
