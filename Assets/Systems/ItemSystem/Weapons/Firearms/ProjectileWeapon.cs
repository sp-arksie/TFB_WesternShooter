using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

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
    int reloadTimeHash;
    Coroutine reloadStartDelayCoroutine;
    Coroutine reloadAnimationCoroutine;
    Coroutine cockWeapon;

    Coroutine reloadCoroutine;
    Coroutine stopReloadCoroutine;

    protected bool isReloading = false;
    protected float shotTime = 0f;
    bool cockGunFinished = true;


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
        //recoilBehaviour.OnRecoilComplete += ProceedGunAnimation;
    }

    private void Start()
    {
        shootHash = Animator.StringToHash("Shoot");
        reloadHash = Animator.StringToHash("Reload");
        isReloadingHash = Animator.StringToHash("IsReloading");
        reloadStartDelayHash = Animator.StringToHash("ReloadStartDelay");
        cockGunHash = Animator.StringToHash("CockGun");
        timeToCockHash = Animator.StringToHash("TimeToCock");
        reloadTimeHash = Animator.StringToHash("ReloadTime");
    }

    private void OnDisable()
    {
        ammoBehaviour.ReloadComplete -= StopReload;
        //recoilBehaviour.OnRecoilComplete -= ProceedGunAnimation;
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
        //if (ammoBehaviour.GetCanReload() && !isReloading) Reload();
        if (ammoBehaviour.GetCanReload() && !isReloading)
        {
            if(reloadCoroutine != null) { StopCoroutine(reloadCoroutine); }
            reloadCoroutine = StartCoroutine(Reload());
        }
    }
    public bool CanShoot()
    {
        return ammoBehaviour.GetHasBulletInMagazine() && shootingBehaviour.GetCanShoot() && !isReloading && cockGunFinished;
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

        cockGunFinished = false;
        SetWhenToStartWeaponCock();
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

    //private void Reload()
    //{
    //    isReloading = true;
    //    animator?.SetTrigger(reloadHash);
    //    animator?.SetBool(isReloadingHash, true);
    //    float delay = ammoBehaviour.GetReloadStartOrFinishDelay();
    //    float reloadTime = ammoBehaviour.GetReloadTime();
    //    if (delay > 0)
    //    {
    //        if(reloadStartDelayCoroutine != null) StopCoroutine(reloadStartDelayCoroutine);
    //        reloadStartDelayCoroutine = StartCoroutine(StartDelayReloadAnimation(delay));
    //        DOVirtual.DelayedCall(delay + delay * 0.1f, () => 
    //            {
    //                ammoBehaviour.StartReload();
    //                if(reloadAnimationCoroutine != null) StopCoroutine(reloadAnimationCoroutine);
    //                StartCoroutine(ReloadAnimation(reloadTime));
    //            });
    //    }
    //    else
    //    {
    //        ammoBehaviour.StartReload();

    //        if (reloadAnimationCoroutine != null) StopCoroutine(reloadAnimationCoroutine);
    //        StartCoroutine(ReloadAnimation(reloadTime));
    //    }
    //}

    private IEnumerator Reload()
    {
        isReloading = true;
        animator?.SetTrigger(reloadHash);
        animator?.SetBool(isReloadingHash, true);

        if(RightArmOnly) { ActivateLeftArm(); }
        
        float delay = ammoBehaviour.GetReloadStartOrFinishDelay();
        float reloadTime = ammoBehaviour.GetReloadTime();

        if (delay > 0)
        {
            if (reloadStartDelayCoroutine != null) StopCoroutine(reloadStartDelayCoroutine);
            yield return reloadStartDelayCoroutine = StartCoroutine(StartDelayReloadAnimation(delay));
        }

        ammoBehaviour.StartReload();
        if (reloadAnimationCoroutine != null) StopCoroutine(reloadAnimationCoroutine);
        yield return reloadAnimationCoroutine = StartCoroutine(ReloadAnimation(reloadTime));
    }

    private void StopReload()
    {
        if(stopReloadCoroutine != null) StopCoroutine(stopReloadCoroutine);
        stopReloadCoroutine = StartCoroutine(StopReloadCoroutine());
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

    private IEnumerator ReloadAnimation(float timeToReload)
    {
        float t = 0f;
        while (isReloading)
        {
            while (t / timeToReload <= 1)
            {
                animator?.SetFloat(reloadTimeHash, t / timeToReload);
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            t = 0f;
        }
        yield return null;
    }

    private IEnumerator StopReloadCoroutine()
    {
        animator?.SetBool(isReloadingHash, false);
        isReloading = false;
        ammoBehaviour.CancelReload();

        float delay = ammoBehaviour.GetReloadStartOrFinishDelay();
        if (delay > 0)
        {
            if (reloadStartDelayCoroutine != null) StopCoroutine(reloadStartDelayCoroutine);
            yield return reloadStartDelayCoroutine = StartCoroutine(StartDelayReloadAnimation(delay));
        }

        if(RightArmOnly) DeactivateLeftArm();
    }

    private void ProceedGunAnimation()
    {
        //animator?.SetTrigger(cockGunHash);
        //if(cockWeapon != null) { StopCoroutine(cockWeapon); }
        //cockWeapon = StartCoroutine(CockWeapon());
    }

    //private IEnumerator CockWeapon()
    //{
    //    float dt = 0f;
    //    float t = 0f;
    //    float timeToCock = shootingBehaviour.SecondsBetweenShot - (Time.time - shotTime);
    //    if(timeToCock > 0)
    //    {
    //        while (t <= 1)
    //        {
    //            t = dt / timeToCock;
    //            animator?.SetFloat(timeToCockHash, t);
    //            yield return new WaitForEndOfFrame();
    //            dt += Time.deltaTime;
    //        }
    //    }
    //    else
    //    {
    //        animator?.SetFloat(timeToCockHash, 1f);
    //    }

    //    cockGunFinished = true;
    //}

    private IEnumerator CockWeapon(float animationLength)
    {
        animator?.SetTrigger(cockGunHash);

        float dt = 0f;
        float t = 0f;

        while (t <= 1)
        {
            t = dt / animationLength;
            animator?.SetFloat(timeToCockHash, t);
            yield return new WaitForEndOfFrame();
            dt += Time.deltaTime;
        }


        cockGunFinished = true;
    }

    private void SetWhenToStartWeaponCock()
    {
        float delay = shootingBehaviour.SecondsBetweenShot * shootingBehaviour.TimeToCockStartPercent;
        float animationTime = shootingBehaviour.SecondsBetweenShot - delay;

        DOVirtual.DelayedCall(delay, () => cockWeapon = StartCoroutine(CockWeapon(animationTime)));
    }
}
