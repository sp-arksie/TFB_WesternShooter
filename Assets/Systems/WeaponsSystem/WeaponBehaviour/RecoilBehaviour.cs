using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class RecoilBehaviour : MonoBehaviour
{
    [SerializeField] float recoilAnimationSpeed = 0.1f;

    [SerializeField] float recoilStrength = 12f;

    [SerializeField] float recoilRecoveryTime = 0.5f;

    [SerializeField] float recoveryStartDelay = 0f;

    [SerializeField] Vector2 recoilDirection = Vector2.up;
    public float RecoilStrength { get => recoilStrength; private set => recoilStrength = value; }
    public float RecoilRecoveryTime { get => recoilRecoveryTime; private set => recoilRecoveryTime = value; }
    public float RecoveryStartDelay { get => recoveryStartDelay; private set => recoveryStartDelay = value; }
    public Vector2 RecoilDirection { get => recoilDirection; private set => recoilDirection = value; }

    public event Action<RecoilBehaviour> OnRecoil;
    public event Action OnRecoilComplete;

    ProjectileWeapon weapon;
    Coroutine recoilAnimationCoroutine;

    private void Awake()
    {
        weapon = GetComponent<ProjectileWeapon>();
    }

    private void OnEnable()
    {
        //weapon.ShootEvent += OnShoot;
    }

    private void OnDisable()
    {
        //weapon.ShootEvent -= OnShoot;
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
}
