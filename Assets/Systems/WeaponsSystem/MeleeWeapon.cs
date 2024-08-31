using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class MeleeWeapon : ItemBase, ICameraShaker
{
    [Header("Light Attack")]
    [SerializeField] float lightBaseDamage = 40f;
    [SerializeField] float lightTimeToStartAnimation = 0.5f;
    [SerializeField] float lightAnimationDuration = 0.5f;
    [SerializeField] Transform lightStartTransform;
    [SerializeField] Transform lightEndTransform;

    [Header("HeavyAttack")]
    [SerializeField] float heavyBaseDamage = 70f;
    [SerializeField] float heavyTimeToStartAnimation = 1f;
    [SerializeField] float heavyAnimationDuration = 0.8f;
    [SerializeField] Transform heavyStartTransform;
    [SerializeField] Transform heavyEndTransform;

    [Header("CameraShake")]
    [SerializeField] float shakeStrength = 5f;
    [SerializeField] Vector2 shakeDirection = new Vector2(0f, -1f);
    [SerializeField] float shakeSpeed = 0.1f;
    [SerializeField] float shakeRecoveryTime = 0.3f;
    [SerializeField] float shakeRecoveryStartDelay = 0f;
    [SerializeField] [Range(0f, 1f)] float recoveryPercent = 1f;

    [Header("General")]
    [SerializeField] float timeToReturnToHipPosition = 0.3f;

    [Header("Colliders")]
    [SerializeField] Transform collidersParent;
    Collider[] colliders;

    bool attackReady = false;

    Coroutine lightMeleeAttack;
    Coroutine prepareMeleeAttack;
    Coroutine startMeleeAttack;

    InputManager input;

    //public override event Action<bool> OnUnskippableActionInProgress;

    private void Awake()
    {
        input = InputManager.Instance;

        colliders = new Collider[collidersParent.childCount];
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i] = collidersParent.GetChild(i).GetComponent<Collider>();
            colliders[i].enabled = false;
        }
    }

    internal override void NotifyClick()
    {
        if (lightMeleeAttack != null) StopCoroutine(lightMeleeAttack);
        lightMeleeAttack = StartCoroutine(PerformLightAttack());
    }

    internal override void NotifyPressStart()
    {
        PrepareHeavyAttack();
    }

    internal override void NotifyPressFinish()
    {
        PerformHeavyAttack();
    }

    internal override void NotifySelected()
    {
        // NOOP
    }

    internal override void NotifyUnselected()
    {
        // NOOP
    }

    protected override void OnUnskippableActionInProgress(bool inProgress)
    {
        base.OnUnskippableActionInProgress(inProgress);
    }

    protected override void OnShakeCamera(ICameraShaker cameraShakeInfo)
    {
        base.OnShakeCamera(cameraShakeInfo);
    }

    private IEnumerator PerformLightAttack()
    {
        if (prepareMeleeAttack != null) StopCoroutine(prepareMeleeAttack);
        yield return prepareMeleeAttack = StartCoroutine(PrepareMeleeAttack(lightStartTransform, lightTimeToStartAnimation));

        if (startMeleeAttack != null) StopCoroutine(startMeleeAttack);
        yield return startMeleeAttack = StartCoroutine(StartMeleeAttack(lightEndTransform, lightAnimationDuration));
    }

    private void PrepareHeavyAttack()
    {
        if (prepareMeleeAttack != null) StopCoroutine(prepareMeleeAttack);
        prepareMeleeAttack = StartCoroutine(PrepareMeleeAttack(heavyStartTransform, heavyTimeToStartAnimation));
    }

    private void PerformHeavyAttack()
    {
        if (startMeleeAttack != null) StopCoroutine(startMeleeAttack);
        startMeleeAttack = StartCoroutine(StartMeleeAttack(heavyEndTransform, lightAnimationDuration));
    }

    private IEnumerator PrepareMeleeAttack(Transform startTransform, float timeToStartAnimation)
    {
        OnUnskippableActionInProgress(true);

        Vector3 currentPosition = transform.localPosition;
        Quaternion currentRotation = transform.localRotation;
        float dt = 0f;
        float t = 0f;

        while (t <= 1f)
        {
            dt += Time.deltaTime;
            t = dt / timeToStartAnimation;

            transform.localPosition = Vector3.Lerp(currentPosition, startTransform.localPosition, t);
            transform.localRotation = Quaternion.Lerp(currentRotation, startTransform.localRotation, t);

            yield return new WaitForEndOfFrame();
        }

        attackReady = true;
        yield return null;
    }

    private IEnumerator StartMeleeAttack(Transform endTransform, float animationDuration)
    {
        while (attackReady != true) { yield return null; }

        Vector3 currentPosition = transform.localPosition;
        Quaternion currentRotation = transform.localRotation;

        float dt = 0f;
        float t = 0f;
        foreach (Collider c in colliders) { c.enabled = true; }

        OnShakeCamera(this);
        while (t <= 1f)
        {
            dt += Time.deltaTime;
            t = dt / animationDuration;

            transform.localPosition = Vector3.Lerp(currentPosition, endTransform.localPosition, t);
            transform.localRotation = Quaternion.Lerp(currentRotation, endTransform.localRotation, t);

            yield return new WaitForEndOfFrame();
        }

        foreach (Collider c in colliders) { c.enabled = false; }

        currentPosition = transform.localPosition;
        currentRotation = transform.localRotation;
        dt = 0f;
        t = 0f;

        while (t <= 1f)
        {
            dt += Time.deltaTime;
            t = dt / timeToReturnToHipPosition;

            transform.localPosition = Vector3.Lerp(currentPosition, hipPositionTransform.localPosition, t);
            transform.localRotation = Quaternion.Lerp(currentRotation, hipPositionTransform.localRotation, t);

            yield return new WaitForEndOfFrame();
        }

        OnUnskippableActionInProgress(false);
        attackReady = false;
        yield return null;
    }

    CameraShakeInfo ICameraShaker.ReturnCameraShakeInfo()
    {
        return new CameraShakeInfo(shakeStrength, shakeDirection, shakeSpeed, shakeRecoveryTime, shakeRecoveryStartDelay, recoveryPercent);
    }
}
