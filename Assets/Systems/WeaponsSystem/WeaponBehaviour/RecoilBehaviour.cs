using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RecoilBehaviour : MonoBehaviour
{
    [SerializeField] float recoilAnimationSpeed = 0.1f;

    public float recoilStrength;

    public float recoilRecoveryTime;

    public float recoveryStartDelay;

    public Vector2 recoilDirection;

    public event Action<RecoilBehaviour> OnRecoil;

    ProjectileWeapon weapon;
    Coroutine recoilAnimationCoroutine;

    private void Awake()
    {
        weapon = GetComponent<ProjectileWeapon>();
    }

    private void OnEnable()
    {
        weapon.ShootEvent += OnShoot;
    }

    private void OnDisable()
    {
        weapon.ShootEvent -= OnShoot;
    }

    private void OnShoot()
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
            t = dt / weapon.GetComponent<RecoilBehaviour>().recoilRecoveryTime;
            weapon.transform.localRotation = Quaternion.Lerp(finish, start, t);
            yield return new WaitForEndOfFrame();
        }
    }
}
