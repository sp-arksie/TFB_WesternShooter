using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : ItemBase, IHitNotifier
{
    [Header("Explosion Info")]
    [SerializeField] float baseDamage;
    [SerializeField] float explosionRadius;
    [SerializeField] AnimationCurve damageFalloff;
    [SerializeField] float timeTillExplosion;
    [SerializeField] StatusEffect statusEffect;

    [Header("Throwing")]
    [SerializeField] float throwStrength;
    [SerializeField] float timeToThrow;
    [SerializeField] Transform throwChargeApex;
    [SerializeField] Transform throwReleaseApex;
    [SerializeField] LayerMask layerMask = Physics.DefaultRaycastLayers;

    [Header("VFX")]
    [SerializeField] Transform beforeExplosionParticlesParent;
    [SerializeField] Transform explosionParticlesParent;

    HotBarManager hotBarManager;
    Rigidbody rigidBody;
    Camera mainCamera;

    HitInfo hitInfo;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;
        mainCamera = Camera.main;
    }

    internal override void NotifyQuickAction()
    {
        // NOOP
    }

    internal override void NotifyChargeStart()
    {
        OnUnskippableActionInProgress(true);
        DOVirtual.DelayedCall(timeTillExplosion, () => Explode());
        HandleVFX(beforeExplosionParticlesParent);
    }

    internal override void NotifyChargeRelease()
    {
        StartCoroutine(Throw());
    }

    internal override void NotifySelected(HotBarManager hotBarManager)
    {
        this.hotBarManager = hotBarManager;
    }

    internal override void NotifyUnselected()
    {
        
    }

    private void Explode()
    {
        HandleVFX(explosionParticlesParent);

        Collider[] results = Physics.OverlapSphere(transform.position, explosionRadius, layerMask, QueryTriggerInteraction.Ignore);
        List<GameObject> uniqueHitObjects = new();

        foreach(Collider c in results)
        {
            if (!uniqueHitObjects.Contains(c.transform.root.gameObject))
            {
                DamageReceiver dr = c.GetComponent<DamageReceiver>();
                
                if(dr != null &&
                    (dr.DamageModifier == DamageModifierDefinitions.DamageModifier.Torso || dr.DamageModifier == DamageModifierDefinitions.DamageModifier.None))
                {
                    uniqueHitObjects.Add(c.transform.root.gameObject);

                    hitInfo = new();
                    hitInfo.baseDamage = baseDamage;
                    hitInfo.locationOfDamageSource = transform.position;
                    hitInfo.weaponCalliber = WeaponCalliber.None;
                    hitInfo.damageFalloffCurve = damageFalloff;
                    hitInfo.statusEffect = statusEffect;

                    dr.NotifyHit(this);
                    
                    Rigidbody rb = c.GetComponent<Rigidbody>();
                    if(rb != null)
                    {
                        rb.AddExplosionForce(70f, transform.position, explosionRadius, 600f, ForceMode.VelocityChange);
                    }
                }
            }
        }
    }

    private void HandleVFX(Transform particlesToEmitParent)
    {
        if (particlesToEmitParent != null)
        {
            for (int i = 0; i < particlesToEmitParent.transform.childCount; i++)
            {
                Transform particles = particlesToEmitParent.transform.GetChild(i);
                //ParticleSystem ps = particles.gameObject.GetComponent<ParticleSystem>();
                //var mainModule = ps.main;
                //mainModule.duration = timeTillExplosion;
                particles.gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator Throw()
    {
        float duration = timeToThrow * 2 / 3;
        yield return StartCoroutine(AnimateThrow(transform.position, throwChargeApex.position, duration));
        duration = timeToThrow - duration;
        yield return StartCoroutine(AnimateThrow(transform.position, throwReleaseApex.position, duration));

        ApplyForce();
        OnUnskippableActionInProgress(false);
        hotBarManager.NotifyRemoveItem(this.gameObject);
    }

    private IEnumerator AnimateThrow(Vector3 start, Vector3 end, float duration)
    {
        float dt = 0f;
        float t = 0f;

        while(t < 1)
        {
            t = dt / duration;

            transform.position = Vector3.Lerp(start, end, t);
            yield return new WaitForEndOfFrame();

            dt += Time.deltaTime;
        }
        transform.position = end;
    }

    private void ApplyForce()
    {
        rigidBody.isKinematic = false;

        Vector3 direction = mainCamera.transform.forward;

        rigidBody.AddForce(direction * throwStrength, ForceMode.VelocityChange);
        rigidBody.AddRelativeTorque(new Vector3(12f, 0f, 6f), ForceMode.VelocityChange);
    }

    HitInfo IHitNotifier.GetHitInfo()
    {
        return hitInfo;
    }
}
