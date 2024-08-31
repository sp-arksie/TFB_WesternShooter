using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

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

    [Header("Item switching")]
    [SerializeField] float timeToSwitch = 0.6f;
    Coroutine selectItem;
    Coroutine animateItemSwitch;
    bool currentItemBusy = false;

    // Items index
    int startingIndex = 0;
    int currentIndex = 0;

    // References
    InputManager input;
    Animator animator;

    // Animator hashes
    int hasGunHash;
    int rightArmOnlyHash;

    public event Action<ItemBase> ItemSelectedEvent;
    public event Action<ItemBase> ItemUnselectedEvent;


    private void Awake()
    {
        input = InputManager.Instance;
        animator = GetComponentInChildren<Animator>();
        defaultFOV = currentFOV = virtualCamera.m_Lens.FieldOfView;
    }

    private void OnEnable()
    {
        //input.GetShootAction().performed += OnClick;
        //input.GetUseAction().started += OnPressStarted;
        //input.GetUseAction().canceled += OnPressFinished;
        input.GetUseAction().performed += OnClick;
        input.GetUseAction().started += OnPressStarted;
        input.GetUseAction().performed += OnPressFinished;

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
        ItemBase item = hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>();
        animator.SetBool(hasGunHash, true);
        animator.SetBool(rightArmOnlyHash, item.RightArmOnly);
        ItemSelectedEvent?.Invoke(item);
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
        //input.GetShootAction().performed -= OnClick;
        //input.GetUseAction().started -= OnPressStarted;
        //input.GetUseAction().canceled -= OnPressFinished;
        input.GetUseAction().performed -= OnClick;
        input.GetUseAction().started -= OnPressStarted;
        input.GetUseAction().performed -= OnPressFinished;

        input.GetAimAction().started -= OnAim;
        input.GetAimAction().canceled -= OnAim;
        input.GetScrollAction().performed -= OnScroll;
        for (int i = 0; i < input.GetHotBarSlotActions().Count; i++)
        {
            input.GetHotBarSlotActions()[i].performed -= (context) => OnHotBarSelect(i);
        }
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        //hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>().NotifyClick();
        if(context.interaction is TapInteraction)
        {
            hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>().NotifyClick();
        }
    }

    private void OnPressStarted(InputAction.CallbackContext context)
    {
        //hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>().NotifyPressStart();
        if (context.interaction is SlowTapInteraction)
        {
            hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>().NotifyPressStart();
        }
    }

    private void OnPressFinished(InputAction.CallbackContext context)
    {
        if (context.interaction is SlowTapInteraction)
        {
            hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>().NotifyPressFinish();
        }
    }

    #region Aiming
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
    #endregion

    #region Reload
    private void OnReload(InputAction.CallbackContext context)
    {
        if(hotBarItems.GetChild(currentIndex).TryGetComponent<ProjectileWeapon>(out var weapon))
        {
            weapon.NotifyReload();
        }
    }


    #endregion

    #region Item Switching
    private void OnScroll(InputAction.CallbackContext context)
    {
        if (!currentItemBusy)
        {
            Vector2 value = context.ReadValue<Vector2>();
            if (value.y > 0f)
            {
                if (selectItem != null) { StopCoroutine(selectItem); }
                StartCoroutine(SelectItem(currentIndex + 1));
            }
            if (value.y < 0f)
            {
                if (selectItem != null) { StopCoroutine(selectItem); }
                StartCoroutine(SelectItem(currentIndex - 1));
            }
        }
    }

    private IEnumerator SelectItem(int index)
    {
        ItemBase item = hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>();
        item.NotifyUnselected();
        item.onUnskippableActionInProgress -= SetItemBusy;
        ItemUnselectedEvent?.Invoke(item);

        Transform current = item.transform;
        Transform goal = item.HiddenTransform;
        if(animateItemSwitch != null) StopCoroutine(animateItemSwitch);
        yield return animateItemSwitch = StartCoroutine(AnimateItemSwitch(item, current, goal));
        
        UngrabPoints();
        hotBarItems.GetChild(currentIndex).gameObject.SetActive(false);


        if (index < 0)
            index = hotBarItems.childCount - 1;
        if (index > hotBarItems.childCount - 1)
            index = 0;
        currentIndex = index;


        hotBarItems.GetChild(currentIndex).gameObject.SetActive(true);
        GrabPoints();
        item = hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>();
        item.NotifySelected();
        item.onUnskippableActionInProgress += SetItemBusy;
        ItemSelectedEvent?.Invoke(item);
        animator.SetBool(rightArmOnlyHash, item.RightArmOnly);

        current = item.transform;
        goal = item.HipPositionTransform;
        if (animateItemSwitch != null) StopCoroutine(animateItemSwitch);
        yield return StartCoroutine(AnimateItemSwitch(item, current, goal));
    }

    private void SetItemBusy(bool itemBusy)
    {
        this.currentItemBusy = itemBusy;
    }

    private IEnumerator AnimateItemSwitch(ItemBase item, Transform current, Transform goal)
    {
        float dt = 0f;

        while(dt < timeToSwitch * 0.5)
        {
            dt += Time.deltaTime;
            item.transform.localPosition = Vector3.Lerp(current.localPosition, goal.localPosition, dt);
            item.transform.localRotation = Quaternion.Lerp(current.localRotation, goal.localRotation, dt);
            yield return null;
        }
        item.transform.localPosition = goal.localPosition;
        item.transform.localRotation = goal.localRotation;
    }
    
    private void UngrabPoints()
    {
        ItemBase weapon = hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>();
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
        ItemBase weapon = hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>();
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
    #endregion

    private void OnApplicationQuit()
    {
        virtualCamera.m_Lens.FieldOfView = defaultFOV;
    }
}
