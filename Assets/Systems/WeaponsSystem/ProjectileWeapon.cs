using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class ProjectileWeapon : ItemBase
{
    [Header("VFX")]
    [SerializeField] Transform muzzleFlashParent;
    [SerializeField] Transform smokeEmissionPointsParent;
    [SerializeField] GameObject muzzleFlashPrefab;
    [SerializeField] GameObject gunSmokePrefab;

    [Header("Animation")]
    [SerializeField] Transform adsPositionTransform;
    [SerializeField] Transform shootingRotationTransform;
    [SerializeField] Transform baseRotationTransform;
    public Transform AdsPositionTransform { get { return adsPositionTransform; } private set { adsPositionTransform = value; } }
    public Transform ShootingRotationTransform { get { return shootingRotationTransform; } private set { shootingRotationTransform = value; } }
    public Transform BaseRotationTransform { get { return baseRotationTransform; } private set { baseRotationTransform = value; } }

    //public event Action ShootEvent;
    public event Action SelectedEvent;
    
    Barrel barrel;
    ShootingBehaviour shootingBehaviour;
    AmmoBehaviour ammoBehaviour;
    DamageBehaviour damageBehaviour;
    RecoilBehaviour recoilBehaviour;
    Animator animator;

    int shootHash;
    int reloadHash;
    int isReloadingHash;
    int reloadStartDelayHash;
    int cockGunHash;
    int timeToCockHash;
    Coroutine reloadStartDelay;
    Coroutine cockWeapon;

    bool isReloading = false;
    float shotTime = 0f;

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
        recoilBehaviour = GetComponent<RecoilBehaviour>();
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        ammoBehaviour.ReloadComplete += StopReload;
        recoilBehaviour.OnRecoilComplete += ProceedGunAnimation;
    }

    private void Start()
    {
        shootHash = Animator.StringToHash("Shoot");
        reloadHash = Animator.StringToHash("Reload");
        isReloadingHash = Animator.StringToHash("IsReloading");
        reloadStartDelayHash = Animator.StringToHash("ReloadStartDelay");
        cockGunHash = Animator.StringToHash("CockGun");
        timeToCockHash = Animator.StringToHash("TimeToCock");
    }

    private void OnDisable()
    {
        ammoBehaviour.ReloadComplete -= StopReload;
        recoilBehaviour.OnRecoilComplete -= ProceedGunAnimation;
    }

    internal override void NotifyAttack()
    {
        if(CanShoot()) Shoot();
        else if (isReloading) { StopReload(); }
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
        if (ammoBehaviour.CanReload() && !isReloading) Reload();
    }
    public bool CanShoot()
    {
        return ammoBehaviour.GetHasBulletInMagazine() && shootingBehaviour.GetCanShoot() && !isReloading;
    }

    private void Shoot()
    {
        shotTime = Time.time;
        barrel.Shoot(shootingBehaviour.GetMuzzleVelocity(), GetHitInfoToSend());
        //ShootEvent?.Invoke();
        recoilBehaviour.ApplyRecoil();
        ammoBehaviour.ConsumeAmmo();
        animator?.SetTrigger(shootHash);
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

    private void Reload()
    {
        isReloading = true;
        animator.SetTrigger(reloadHash);
        animator.SetBool(isReloadingHash, true);
        float delay = ammoBehaviour.GetReloadStartDelay();
        if (delay > 0)
        {
            if(reloadStartDelay != null) StopCoroutine(reloadStartDelay);
            reloadStartDelay = StartCoroutine(StartDelayReloadAnimation(delay));
            DOVirtual.DelayedCall(delay, () => ammoBehaviour.StartReload());
        }
        else
        {
            ammoBehaviour.StartReload();
        }
    }

    private void StopReload()
    {
        animator.SetBool(isReloadingHash, false);
        ammoBehaviour.CancelReload();
        float delay = ammoBehaviour.GetReloadStartDelay();
        if (delay > 0)
        {
            if (reloadStartDelay != null) StopCoroutine(reloadStartDelay);
            reloadStartDelay = StartCoroutine(StartDelayReloadAnimation(delay));
        }
        isReloading = false;
    }

    private IEnumerator StartDelayReloadAnimation(float delay)
    {
        float t = 0f;
        while (t / delay <= 1)
        {
            animator.SetFloat(reloadStartDelayHash, t/delay);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private void ProceedGunAnimation()
    {
        animator.SetTrigger(cockGunHash);
        if(cockWeapon != null) { StopCoroutine(cockWeapon); }
        cockWeapon = StartCoroutine(CockWeapon());
    }

    private IEnumerator CockWeapon()
    {
        float dt = 0f;
        float t = 0f;
        float timeToCock = shootingBehaviour.SecondsBetweenShot - (Time.time - shotTime);
        while (t <= 1)
        {
            t = dt / timeToCock;
            animator.SetFloat(timeToCockHash, t);
            yield return new WaitForEndOfFrame();
            dt += Time.deltaTime;
        }
    }
}
