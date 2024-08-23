using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class HotBarManager : MonoBehaviour
{
    [SerializeField] Transform hotBarItems;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    [Header("Grab Points")]
    [SerializeField] ParentConstraint leftArmTarget;
    [SerializeField] ParentConstraint leftArmHint;
    [SerializeField] ParentConstraint RightArmTarget;
    [SerializeField] ParentConstraint RightArmHint;
    [SerializeField] TwoBoneIKConstraint leftArmRig;
    [SerializeField] TwoBoneIKConstraint rightArmRig;

    [Header("Aiming")]
    [SerializeField] float ironSightFOV = 35f;
    [SerializeField] float fovLerpSpeed = 2f;
    Coroutine aimingCoroutine;
    float defaultFOV;
    float currentFOV;

    int startingIndex = 0;
    int currentIndex = 0;

    InputManager input;
    Animator animator;

    int hasGunHash;
    int rightArmOnlyHash;

    public event Action<WeaponBase> WeaponSelectedEvent;
    public event Action<WeaponBase> WeaponUnselectedEvent;


    private void Awake()
    {
        input = InputManager.Instance;
        animator = GetComponentInChildren<Animator>();
        defaultFOV = currentFOV = virtualCamera.m_Lens.FieldOfView;
    }

    private void OnEnable()
    {
        input.GetUseAction().started += OnUse;
        input.GetUseAction().canceled += OnUse;
        input.GetShootAction().performed += OnShoot;
        input.GetAimAction().started += OnAim;
        input.GetAimAction().canceled += OnAim;
        input.GetReloadAction().performed += OnReload;
        input.GetScrollAction().performed += OnScroll;
        for(int i = 0; i < input.GetHotBarSlotActions().Count; i++)
        {
            input.GetHotBarSlotActions()[i].performed += (context) => OnHotBarSelect(i);
        }
    }

    private void Start()
    {
        hasGunHash = Animator.StringToHash("HasGun");
        rightArmOnlyHash = Animator.StringToHash("RightHandOnly");

        for(int i = 0; i < hotBarItems.childCount; i++)
        {
            hotBarItems.GetChild(i).gameObject.SetActive(false);
        }
        currentIndex = startingIndex;
        hotBarItems.GetChild(currentIndex).gameObject.SetActive(true);
        WeaponBase weapon = hotBarItems.GetChild(currentIndex).GetComponent<WeaponBase>();
        animator.SetBool(hasGunHash, true);
        animator.SetBool(rightArmOnlyHash, weapon.RightArmOnly);
        WeaponSelectedEvent?.Invoke(weapon);
        GrabPoints();
    }

    float desiredRightArmWeight = 0;
    float desiredLeftArmWeight = 0;
    private void Update()
    {
        if(rightArmRig.weight != desiredRightArmWeight) { rightArmRig.weight = desiredRightArmWeight; }
        if(leftArmRig.weight != desiredLeftArmWeight) { leftArmRig.weight = desiredLeftArmWeight; }
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
            weapon.NotifyAttack();
        }
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        if (hotBarItems.GetChild(currentIndex).TryGetComponent<ProjectileWeapon>(out ProjectileWeapon weapon))
        {
            if (context.started)
            {
                if (aimingCoroutine != null) StopCoroutine(aimingCoroutine);
                aimingCoroutine = StartCoroutine(AimingCoroutine(currentFOV, ironSightFOV, weapon));

            }
            else if (context.canceled)
            {
                if (aimingCoroutine != null) StopCoroutine(aimingCoroutine);
                aimingCoroutine = StartCoroutine(AimingCoroutine(currentFOV, defaultFOV, weapon));
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

    private void OnReload(InputAction.CallbackContext context)
    {
        if(hotBarItems.GetChild(currentIndex).TryGetComponent<ProjectileWeapon>(out var weapon))
        {
            weapon.NotifyReload();
        }
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
        UngrabPoints();
        hotBarItems.GetChild(currentIndex).gameObject.SetActive(false);

        if(index < 0)
            index = hotBarItems.childCount - 1;
        if(index > hotBarItems.childCount - 1)
            index = 0;
        currentIndex = index;

        weapon = hotBarItems.GetChild(currentIndex).GetComponent<WeaponBase>();
        weapon.NotifySelected();
        WeaponSelectedEvent?.Invoke(weapon);
        animator.SetBool(rightArmOnlyHash, weapon.RightArmOnly);
        GrabPoints();
        hotBarItems.GetChild(currentIndex).gameObject.SetActive(true);
    }
    
    private void UngrabPoints()
    {
        WeaponBase weapon = hotBarItems.GetChild(currentIndex).GetComponent<WeaponBase>();
        Transform grabPointsParent = weapon.GrabPointsParent;
        if (weapon.RightArmOnly)
        {
            UnapplyConstraint(grabPointsParent.GetChild(0), RightArmTarget);
            UnapplyConstraint(grabPointsParent.GetChild(1), RightArmHint);
        }
        else
        {
            UnapplyConstraint(grabPointsParent.GetChild(0), leftArmTarget);
            UnapplyConstraint(grabPointsParent.GetChild(1), leftArmHint);
            UnapplyConstraint(grabPointsParent.GetChild(2), RightArmTarget);
            UnapplyConstraint(grabPointsParent.GetChild(3), RightArmHint);
        }
        desiredLeftArmWeight = 0;
        desiredRightArmWeight = 0;
    }

    private void UnapplyConstraint(Transform target, ParentConstraint constrainedObject)
    {
        if (constrainedObject)
        {
            constrainedObject.constraintActive = false;
            for (int i = 0; i < constrainedObject.sourceCount; i++)
            {
                constrainedObject.RemoveSource(i);
            }
        }
        else throw new System.Exception("Arm rigs are missing ParentConstraints");
    }

    private void GrabPoints()
    {
        WeaponBase weapon = hotBarItems.GetChild(currentIndex).GetComponent<WeaponBase>();
        Transform grabPointsParent = weapon.GrabPointsParent;
        if (weapon.RightArmOnly)
        {
            ApplyConstraint(grabPointsParent.GetChild(0), RightArmTarget);
            ApplyConstraint(grabPointsParent.GetChild(1), RightArmHint);
            desiredRightArmWeight = 1;
        }
        else
        {
            ApplyConstraint(grabPointsParent.GetChild(0), leftArmTarget);
            ApplyConstraint(grabPointsParent.GetChild(1), leftArmHint);
            ApplyConstraint(grabPointsParent.GetChild(2), RightArmTarget);
            ApplyConstraint(grabPointsParent.GetChild(3), RightArmHint);
            desiredLeftArmWeight = 1;
            desiredRightArmWeight = 1;
        }
    }

    private void ApplyConstraint(Transform target, ParentConstraint constrainedObject)
    {
        if (constrainedObject)
        {
            ConstraintSource source = new();
            source.sourceTransform = target;
            source.weight = 1f;
            constrainedObject.AddSource(source);
            constrainedObject.SetTranslationOffset(0, Vector3.zero);
            constrainedObject.SetRotationOffset(0, Vector3.zero);
            constrainedObject.constraintActive = true;

        }
        else throw new System.Exception("Arm rigs are missing ParentConstraints");
    }

    private void OnHotBarSelect(int i)
    {
        throw new NotImplementedException();
    }
    
}
