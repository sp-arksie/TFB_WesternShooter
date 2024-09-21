using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponForEntity : ProjectileWeapon
{
    public float effectiveRange { get; private set; } = 0f;

    int curveSampleRate = 10;

    public AmmoBehaviourForEntity ammoBehaviourForEntity { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        ammoBehaviourForEntity = GetComponent<AmmoBehaviourForEntity>();

        AnimationCurve damageFalloffCurve = damageBehaviour.GetDamageFalloffCurve();

        bool effectiveRangeFound = false;
        for (int i = 0; i < 200 && !effectiveRangeFound; i += curveSampleRate)
        {
            float falloffValue = damageFalloffCurve.Evaluate((float)i);
            if(falloffValue < 1)
            {
                effectiveRange = (float)i;
                effectiveRangeFound = true;
            }
        }
    }


    protected override GameObject GetPrefabToInstantiate()
    {
        return ammoBehaviourForEntity.GetProjectilePrefabToShoot();
    }
}
