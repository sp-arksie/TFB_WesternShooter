using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class ProjectileWeapon : ItemBase, ICameraShaker
{
    [Header("VFX")]
    [SerializeField] Transform muzzleFlashParent;
    [SerializeField] Transform smokeEmissionPointsParent;
    [SerializeField] GameObject muzzleFlashPrefab;
    [SerializeField] GameObject gunSmokePrefab;

    [Header("Shooting + Aiming Transform")]
    [SerializeField] Transform adsPositionTransform;
    [SerializeField] Transform shootingRotationTransform;
    [SerializeField] Transform baseRotationTransform;
    public Transform AdsPositionTransform { get => adsPositionTransform; private set => adsPositionTransform = value; }
    public Transform ShootingRotationTransform { get => shootingRotationTransform; private set => shootingRotationTransform = value; }
    public Transform BaseRotationTransform { get => baseRotationTransform; private set => baseRotationTransform = value; }


    //public event Action SelectedEvent;

    Barrel barrel;
    Animator animator;

    ShootingBehaviour shootingBehaviour;
    AmmoBehaviour ammoBehaviour;
    DamageBehaviour damageBehaviour;
    RecoilBehaviour recoilBehaviour;

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

    float effectiveRange;
    public float EffectiveRange { get => effectiveRange; private set => effectiveRange = value; }


    #region DEBUG

    [Header("Debug")]
    [SerializeField] bool debugShoot = false;

    private void OnValidate()
    {
        if (debugShoot)
        {
            debugShoot = false;
            NotifyQuickAction();
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

        //effectiveRange = DamageFalloffDefinitions.GetEffectiveRange(damageBehaviour.GetWeaponCalliber());
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

    internal override void NotifyQuickAction()
    {
        if(CanShoot()) Shoot();
        else if (isReloading) { StopReload(); }
    }

    internal override void NotifyChargeStart()
    {
        // NOOP
    }

    internal override void NotifyChargeRelease()
    {
        // NOOP
    }

    internal override void NotifySelected(HotBarManager hotBarManager)
    {
        //SelectedEvent?.Invoke();
    }

    internal override void NotifyUnselected()
    {
        // NOOP
    }

    internal void NotifyAmmoSwitch(int increaseDecrease)
    {
        // TODO: only switch if other types available

        // animate removing bullets
        // after this delay. prompt to switch bullet type (only send value from input - index managed on AmmoBehaviour)
        ammoBehaviour.SwitchAmmoType(increaseDecrease);
    }

    protected override void OnShakeCamera(ICameraShaker cameraShakeInfo)
    {
        base.OnShakeCamera(cameraShakeInfo);
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
        ItemInventoryMediator.ProjectileInfo projectileInfo =  ItemInventoryMediator.Instance.projectileTracker[ammoBehaviour.CurrentProjectileSelected];
        Projectile projectile = projectileInfo.prefabToInstatiate.GetComponent<Projectile>();

        barrel.Shoot(shootingBehaviour.GetMuzzleVelocity(), projectileInfo, projectile, GetHitInfoToSend(projectile));
        OnShakeCamera(this);
        recoilBehaviour.ApplyRecoil();
        ammoBehaviour.ConsumeMagazineAmmo();
        animator?.SetTrigger(shootHash);
        InstantiateParticles();

        
    }

    private HitInfo GetHitInfoToSend(Projectile projectile)
    {
        HitInfo hitInfo = new();
        hitInfo.baseDamage = damageBehaviour.GetBaseDamage();
        hitInfo.locationOfDamageSource = transform.position;
        hitInfo.weaponCalliber = projectile.MatchingCalliber;

        hitInfo.damageFalloffCurve = projectile.DamageFalloff;
        hitInfo.statusEffect = projectile.StatusEffect;
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

    CameraShakeInfo ICameraShaker.ReturnCameraShakeInfo()
    {
        return recoilBehaviour.GetCameraShakeInfo();
    }
}
