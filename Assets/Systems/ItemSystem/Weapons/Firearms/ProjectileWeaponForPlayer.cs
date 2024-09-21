using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ProjectileWeaponForPlayer : ProjectileWeapon, ICameraShaker
{
    AmmoBehaviourForPlayer ammoBehaviourForPlayer;
    ItemInventoryMediator inventoryMediator;

    protected override void Awake()
    {
        base.Awake();
        ammoBehaviourForPlayer = GetComponent<AmmoBehaviourForPlayer>();
        inventoryMediator = ItemInventoryMediator.Instance;
    }

    internal void NotifyAmmoSwitch(int increaseDecrease)
    {
        // TODO: only switch if other types available

        // animate removing bullets
        // after this delay. prompt to switch bullet type (only send value from input - index managed on AmmoBehaviour)
        ammoBehaviourForPlayer.SwitchAmmoType(increaseDecrease);
    }

    protected override void Shoot()
    {
        base.Shoot();
        OnShakeCamera(this);
    }

    protected override GameObject GetPrefabToInstantiate()
    {
        return inventoryMediator.projectileTracker[ammoBehaviourForPlayer.CurrentProjectileSelected].prefabToInstatiate;
    }

    protected override void OnShakeCamera(ICameraShaker cameraShakeInfo)
    {
        base.OnShakeCamera(cameraShakeInfo);
    }

    CameraShakeInfo ICameraShaker.ReturnCameraShakeInfo()
    {
        return recoilBehaviour.GetCameraShakeInfo();
    }
}
