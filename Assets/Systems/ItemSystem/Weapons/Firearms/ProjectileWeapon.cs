using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Video;

[RequireComponent(typeof(ShootingBehaviour))]
[RequireComponent(typeof(DamageBehaviour))]
[RequireComponent(typeof(RecoilBehaviour))]
public class ProjectileWeapon : ItemBase
{
    [Header("VFX")]
    [SerializeField] Transform muzzleFlashParent;
    [SerializeField] Transform smokeEmissionPointsParent;
    [SerializeField] GameObject muzzleFlashPrefab;

    [Header("Shooting + Aiming Transform")]
    [SerializeField] Transform adsPositionTransform;
    [SerializeField] Transform shootingRotationTransform;
    [SerializeField] Transform baseRotationTransform;
    public Transform AdsPositionTransform { get => adsPositionTransform; private set => adsPositionTransform = value; }
    public Transform ShootingRotationTransform { get => shootingRotationTransform; private set => shootingRotationTransform = value; }
    public Transform BaseRotationTransform { get => baseRotationTransform; private set => baseRotationTransform = value; }


    protected Barrel barrel;
    protected Animator animator;

    protected ShootingBehaviour shootingBehaviour;
    protected AmmoBehaviour ammoBehaviour;
    protected DamageBehaviour damageBehaviour;
    protected RecoilBehaviour recoilBehaviour;

    int shootHash;
    int reloadHash;
    int isReloadingHash;
    int reloadStartDelayHash;
    int cockGunHash;
    int timeToCockHash;
    Coroutine reloadStartDelayCoroutine;
    Coroutine cockWeapon;

    protected bool isReloading = false;
    protected float shotTime = 0f;


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

    protected virtual void Awake()
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
        // NOOP
    }

    internal override void NotifyUnselected()
    {
        // NOOP
    }

    internal void NotifyReload()
    {
        if (ammoBehaviour.GetCanReload() && !isReloading) Reload();
    }
    public bool CanShoot()
    {
        return ammoBehaviour.GetHasBulletInMagazine() && shootingBehaviour.GetCanShoot() && !isReloading;
    }

    protected virtual void Shoot()
    {
        shotTime = Time.time;
        Projectile projectile = GetPrefabToInstantiate().GetComponent<Projectile>();

        barrel.Shoot(shootingBehaviour, GetPrefabToInstantiate(), projectile, GetHitInfoToSend(projectile));
        recoilBehaviour.ApplyRecoil();
        ammoBehaviour.NotifyProjectileConsumed();
        animator?.SetTrigger(shootHash);
        InstantiateParticles();
    }

    protected virtual GameObject GetPrefabToInstantiate() { throw new NotImplementedException(); }

    private HitInfo GetHitInfoToSend(Projectile projectile)
    {
        HitInfo hitInfo = new();
        hitInfo.baseDamage = damageBehaviour.GetBaseDamage();
        hitInfo.damageModifier = projectile.DamageModifier;
        hitInfo.locationOfDamageSource = transform.position;
        hitInfo.weaponCalliber = projectile.MatchingCalliber;
        hitInfo.damageFalloffCurve = damageBehaviour.GetDamageFalloffCurve();
        hitInfo.statusEffect = projectile.StatusEffect;
        return hitInfo;
    }

    private void InstantiateParticles()
    {
        if (muzzleFlashPrefab && muzzleFlashParent)
        {
            Instantiate(muzzleFlashPrefab, muzzleFlashParent);
        }
        if (smokeEmissionPointsParent)
        {
            for (int i = 0; i < smokeEmissionPointsParent.childCount; i++)
            {
                smokeEmissionPointsParent.GetChild(i).GetComponentInChildren<VisualEffect>().SendEvent("OnPlay");
            }
        }
    }

    private void Reload()
    {
        isReloading = true;
        animator?.SetTrigger(reloadHash);
        animator?.SetBool(isReloadingHash, true);
        float delay = ammoBehaviour.GetReloadStartOrFinishDelay();
        if (delay > 0)
        {
            if(reloadStartDelayCoroutine != null) StopCoroutine(reloadStartDelayCoroutine);
            reloadStartDelayCoroutine = StartCoroutine(StartDelayReloadAnimation(delay));
            DOVirtual.DelayedCall(delay, () => ammoBehaviour.StartReload());
        }
        else
        {
            ammoBehaviour.StartReload();
        }
    }

    private void StopReload()
    {
        animator?.SetBool(isReloadingHash, false);
        ammoBehaviour.CancelReload();
        float delay = ammoBehaviour.GetReloadStartOrFinishDelay();
        if (delay > 0)
        {
            if (reloadStartDelayCoroutine != null) StopCoroutine(reloadStartDelayCoroutine);
            reloadStartDelayCoroutine = StartCoroutine(StartDelayReloadAnimation(delay));
        }
        isReloading = false;
    }

    private IEnumerator StartDelayReloadAnimation(float delay)
    {
        float t = 0f;
        while (t / delay <= 1)
        {
            animator?.SetFloat(reloadStartDelayHash, t / delay);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private void ProceedGunAnimation()
    {
        animator?.SetTrigger(cockGunHash);
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
            animator?.SetFloat(timeToCockHash, t);
            yield return new WaitForEndOfFrame();
            dt += Time.deltaTime;
        }
    }
}
