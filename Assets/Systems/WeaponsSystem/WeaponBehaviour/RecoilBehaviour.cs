using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class RecoilBehaviour : MonoBehaviour
{
    [SerializeField] float recoilStrength = 12f;

    [SerializeField] Vector2 recoilDirection = Vector2.up;

    [SerializeField] float recoilAnimationSpeed = 0.1f;

    [SerializeField] float recoilRecoveryTime = 0.5f;

    [SerializeField] float recoveryStartDelay = 0f;

    [SerializeField] [Range(0f, 1f)] float recoveryPercent = 0.9f;

    public event Action<RecoilBehaviour> OnRecoil;
    public event Action OnRecoilComplete;

    ProjectileWeapon weapon;
    Coroutine recoilAnimationCoroutine;

    private void Awake()
    {
        weapon = GetComponent<ProjectileWeapon>();
    }

    internal void ApplyRecoil()
    {
        Recoil();
    }

    private void Recoil()
    {
        OnRecoil?.Invoke(this);

        if (recoilAnimationCoroutine != null) StopCoroutine(recoilAnimationCoroutine);
        recoilAnimationCoroutine = StartCoroutine(ShootingAnimationCoroutine());
    }

    private IEnumerator ShootingAnimationCoroutine()
    {
        float dt = 0f;
        float t = 0f;
        Quaternion start = weapon.BaseRotationTransform.localRotation;
        Quaternion finish = weapon.ShootingRotationTransform.localRotation;

        while (t <= 1)
        {
            dt += Time.deltaTime;
            t = dt / recoilAnimationSpeed;
            weapon.transform.localRotation = Quaternion.Lerp(start, finish, t);
            yield return new WaitForEndOfFrame();
        }
        dt = t = 0f;
        while (t <= 1)
        {
            dt += Time.deltaTime;
            t = dt / recoilRecoveryTime;
            weapon.transform.localRotation = Quaternion.Lerp(finish, start, t);
            yield return new WaitForEndOfFrame();
        }
        OnRecoilComplete?.Invoke();
        yield return null;
    }

    internal CameraShakeInfo GetCameraShakeInfo()
    {
        return new CameraShakeInfo(recoilStrength, recoilDirection, recoilAnimationSpeed, recoilRecoveryTime, recoveryStartDelay, recoveryPercent);
    }
}
