using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotBarManager : MonoBehaviour
{
    [SerializeField] Transform hotBarItems;

    [SerializeField] CinemachineVirtualCamera virtualCamera;

    int startingIndex = 0;
    int currentIndex = 0;

    InputManager input;
    CameraShakeController cameraShakeController;

    [Header("Aiming")]
    [SerializeField] float ironSightFOV = 35f;
    [SerializeField] float fovLerpSpeed = 2f;
    Coroutine aimingCoroutine;
    float defaultFOV;
    float currentFOV;

    [SerializeField] float recoilAnimationSpeed = 0.1f;
    Coroutine shootingAnimationCoroutine;

    public event Action<WeaponBase> WeaponSelectedEvent;
    public event Action<WeaponBase> WeaponUnselectedEvent;


    private void Awake()
    {
        input = InputManager.Instance;
        cameraShakeController = GetComponent<CameraShakeController>();
        defaultFOV = currentFOV = virtualCamera.m_Lens.FieldOfView;
    }

    private void OnEnable()
    {
        input.GetAimAction().started += OnUse;
        input.GetAimAction().canceled += OnUse;
        input.GetShootAction().performed += OnShoot;
        input.GetAimAction().started += OnAim;
        input.GetAimAction().canceled += OnAim;
        input.GetScrollAction().performed += OnScroll;
        for(int i = 0; i < input.GetHotBarSlotActions().Count; i++)
        {
            input.GetHotBarSlotActions()[i].performed += (context) => OnHotBarSelect(i);
        }
    }

    private void Start()
    {
        for(int i = 0; i < hotBarItems.childCount; i++)
        {
            hotBarItems.GetChild(i).gameObject.SetActive(false);
        }
        currentIndex = startingIndex;
        hotBarItems.GetChild(currentIndex).gameObject.SetActive(true);
        WeaponSelectedEvent?.Invoke(hotBarItems.GetChild(currentIndex).GetComponent<WeaponBase>());
    }

    private void Update()
    {

    }

    private void OnDisable()
    {
        input.GetAimAction().started -= OnUse;
        input.GetAimAction().canceled -= OnUse;
        input.GetShootAction().performed -= OnShoot;
        input.GetAimAction().started -= OnAim;
        input.GetAimAction().canceled -= OnAim;
        input.GetScrollAction().performed -= OnScroll;
        for (int i = 0; i < input.GetHotBarSlotActions().Count; i++)
        {
            input.GetHotBarSlotActions()[i].performed -= (context) => OnHotBarSelect(i);
        }
    }

    private void OnApplicationQuit()
    {
        virtualCamera.m_Lens.FieldOfView = defaultFOV;
    }

    private void OnUse(InputAction.CallbackContext context)
    {
        // TODO: press and hold to use item (item takes, eg: 3s to consume)
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if(hotBarItems.GetChild(currentIndex).TryGetComponent<ProjectileWeapon>(out ProjectileWeapon weapon))
        {
            if (weapon.CanShoot())
            {
                weapon.NotifyAttack();
                //if (cameraShakeController && weapon.TryGetComponent<RecoilBehaviour>(out RecoilBehaviour recoilStats))
                //{
                //    cameraShakeController.StartRecoil(recoilStats);
                //}
            }
        }
    }

    private IEnumerator ShootingAnimationCoroutine(ProjectileWeapon weapon)
    {
        float dt = 0f;
        float t = 0f;
        Quaternion start = weapon.transform.localRotation;
        Quaternion finish = weapon.ShootingRotationTransform.localRotation;

        while(t <= 1)
        {
            dt += Time.deltaTime;
            t = dt / recoilAnimationSpeed;
            weapon.transform.localRotation = Quaternion.Lerp(start, finish, t);
            yield return new WaitForEndOfFrame();
        }
        dt = t = 0f;
        while(t <= 1)
        {
            dt += Time.deltaTime;
            t = dt / weapon.GetComponent<RecoilBehaviour>().recoilRecoveryTime;
            weapon.transform.localRotation = Quaternion.Lerp(finish, start, t);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        // TODO: ADS
        if (hotBarItems.GetChild(currentIndex).TryGetComponent<ProjectileWeapon>(out ProjectileWeapon weapon))
        {
            if (context.started)
            {
                if (aimingCoroutine != null) StopCoroutine(aimingCoroutine);
                aimingCoroutine = StartCoroutine(AimingCoroutine(currentFOV, ironSightFOV, weapon));
                //weapon.NotifyIsAiming(true);

            }
            else if (context.canceled)
            {
                if (aimingCoroutine != null) StopCoroutine(aimingCoroutine);
                aimingCoroutine = StartCoroutine(AimingCoroutine(currentFOV, defaultFOV, weapon));
                //weapon.NotifyIsAiming(false);
            }
        }
    }

    private IEnumerator AimingCoroutine(float startFov, float targetFov, ProjectileWeapon weapon)
    {
        float dt = 0f;
        float t = 0f;

        if (startFov > targetFov)
        {
            while (t <= 1)
            {
                dt += Time.deltaTime;
                t = dt / fovLerpSpeed;
                LerpFOV(startFov, targetFov, t);
                SinerpBetweenHipAndAim(weapon, weapon.HipPositionTransform, weapon.AdsPositionTransform, t);
                yield return new WaitForEndOfFrame();
                currentFOV = virtualCamera.m_Lens.FieldOfView;
            }
        }
        else
        {
            while (t <= 1)
            {
                dt += Time.deltaTime;
                t = dt / fovLerpSpeed;
                LerpFOV(startFov, targetFov, t);
                SinerpBetweenHipAndAim(weapon, weapon.AdsPositionTransform, weapon.HipPositionTransform, t);
                yield return new WaitForEndOfFrame();
                currentFOV = virtualCamera.m_Lens.FieldOfView;
            }
        }
    }

    private void LerpFOV(float current, float target, float time)
    {
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(current, target, time);
    }

    private void SinerpBetweenHipAndAim(ProjectileWeapon weapon, Transform current, Transform target, float time)
    {
        time = Mathf.Sin(time * Mathf.PI * 0.5f);
        weapon.transform.localPosition = Vector3.Lerp(current.localPosition, target.localPosition, time);
    }

    private void OnScroll(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        if(value.y > 0f)
        {
            SelectItem(currentIndex + 1);
        }
        if(value.y < 0f)
        {
            SelectItem(currentIndex - 1);
        }
    }

    private void SelectItem(int index)
    {
        WeaponBase weapon = hotBarItems.GetChild(currentIndex).GetComponent<WeaponBase>();
        weapon.NotifyUnselected();
        WeaponUnselectedEvent?.Invoke(weapon);
        hotBarItems.GetChild(currentIndex).gameObject.SetActive(false);

        if(index < 0)
            index = hotBarItems.childCount - 1;
        if(index > hotBarItems.childCount - 1)
            index = 0;
        currentIndex = index;

        weapon = hotBarItems.GetChild(currentIndex).GetComponent<WeaponBase>();
        weapon.NotifySelected();
        WeaponSelectedEvent?.Invoke(weapon);
        hotBarItems.GetChild(currentIndex).gameObject.SetActive(true);
    }

    private void OnHotBarSelect(int i)
    {
        throw new NotImplementedException();
    }
    
}
