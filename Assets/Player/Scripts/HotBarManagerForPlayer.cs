using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem;
using System;

public class HotBarManagerForPlayer : HotBarManager
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    [Header("Aiming")]
    [SerializeField] float ironSightFOV = 35f;
    [SerializeField] float fovLerpSpeed = 2f;
    Coroutine aimingCoroutine;
    float defaultFOV;
    float currentFOV;

    InputManager input;
    ItemInventoryMediator inventoryMediator;

    protected override void Awake()
    {
        base.Awake();
        input = InputManager.Instance;
        inventoryMediator = ItemInventoryMediator.Instance;
        defaultFOV = currentFOV = virtualCamera.m_Lens.FieldOfView;
    }

    protected override void Start()
    {
        startingIndex = -1;
        base.Start();
    }

    private void OnEnable()
    {
        input.GetUseAction().performed += OnClick;
        input.GetUseAction().started += OnPressStarted;
        input.GetUseAction().performed += OnPressFinished;

        input.GetAimAction().started += OnAim;
        input.GetAimAction().canceled += OnAim;
        input.GetReloadAction().performed += OnReload;
        input.GetScrollAction().performed += OnScroll;
        input.GetAmmoSwitchAction().performed += OnAmmoSwitch;

        for (int i = 0; i < input.GetHotBarSlotActions().Count; i++)
        {
            input.GetHotBarSlotActions()[i].performed += (context) => OnHotBarSelect(i);
        }

        inventoryMediator.onItemEquipped += EquipItem;
        inventoryMediator.onItemUnequipped += UnequipItem;
    }

    private void OnDisable()
    {
        input.GetUseAction().performed -= OnClick;
        input.GetUseAction().started -= OnPressStarted;
        input.GetUseAction().performed -= OnPressFinished;

        input.GetAimAction().started -= OnAim;
        input.GetAimAction().canceled -= OnAim;
        input.GetScrollAction().performed -= OnScroll;
        input.GetAmmoSwitchAction().performed -= OnAmmoSwitch;

        for (int i = 0; i < input.GetHotBarSlotActions().Count; i++)
        {
            input.GetHotBarSlotActions()[i].performed -= (context) => OnHotBarSelect(i);
        }

        inventoryMediator.onItemEquipped -= EquipItem;
        inventoryMediator.onItemUnequipped -= UnequipItem;
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction && currentIndex != -1)
        {
            QuickAction();
        }
    }

    private void OnPressStarted(InputAction.CallbackContext context)
    {
        if (context.interaction is SlowTapInteraction && currentIndex != -1)
        {
            ChargeStart();
        }
    }

    private void OnPressFinished(InputAction.CallbackContext context)
    {
        if (context.interaction is SlowTapInteraction && currentIndex != -1)
        {
            ChargeRelease();
        }
    }

    #region Aiming
    private void OnAim(InputAction.CallbackContext context)
    {
        if(currentIndex != -1)
        {
            ItemBase item = hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>();
            if (item is ProjectileWeaponForPlayer)
            {
                ProjectileWeaponForPlayer projectileWeapon = item as ProjectileWeaponForPlayer;
                if (context.started)
                {
                    if (aimingCoroutine != null) StopCoroutine(aimingCoroutine);
                    aimingCoroutine = StartCoroutine(AimingCoroutine(currentFOV, ironSightFOV, projectileWeapon));

                }
                else if (context.canceled)
                {
                    if (aimingCoroutine != null) StopCoroutine(aimingCoroutine);
                    aimingCoroutine = StartCoroutine(AimingCoroutine(currentFOV, defaultFOV, projectileWeapon));
                }
            }
        }
    }

    private IEnumerator AimingCoroutine(float startFov, float targetFov, ProjectileWeaponForPlayer weapon)
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
        Reload();
    }
    #endregion

    private void OnScroll(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        if (value.y < 0f)
        {
            NotifySelectItem(currentIndex + 1);
        }
        if (value.y > 0f)
        {
            NotifySelectItem(currentIndex - 1);
        }
    }

    private void OnHotBarSelect(int i)
    {
        throw new NotImplementedException();
    }

    private void OnAmmoSwitch(InputAction.CallbackContext context)
    {
        ProjectileWeaponForPlayer weapon = hotBarItems.GetChild(currentIndex).GetComponent<ProjectileWeaponForPlayer>();
        if(weapon != null)
        {
            Vector2 value = context.ReadValue<Vector2>();
            int i = value.y > 0f ? 1 : -1;
            weapon.NotifyAmmoSwitch(i);
        }
    }

    private void EquipItem(ItemInventoryMediator.EquippedItem item)
    {
        GameObject go = Instantiate(item.prefabToInstantiate, hotBarItems.transform);
        go.SetActive(false);
        ItemBase itemBase = go.GetComponent<ItemBase>();
    }

    private void UnequipItem(int index)
    {
        StartCoroutine(StartUnequip(index));
    }

    private IEnumerator StartUnequip(int index)
    {
        if (currentIndex == index)
        {
            yield return SelectItem(-1);
        }
        Destroy(hotBarItems.GetChild(index).gameObject);

        yield return null;
    }

    private void OnApplicationQuit()
    {
        virtualCamera.m_Lens.FieldOfView = defaultFOV;
    }
}
