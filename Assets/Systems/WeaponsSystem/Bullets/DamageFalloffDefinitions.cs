using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageFalloffDefinitions
{
    public const float SmallCalliberFalloffPoint_1 = 30f;
    public const float SmallCalliberFalloffPoint_2 = 55f;
    public const float SmallCalliberFalloffPoint_3 = 75f;
    public const float SmallCalliberFalloffPoint_End = 200f;
    public const float SmallCalliberFalloffPoint_1Damage = 0.8f;
    public const float SmallCalliberFalloffPoint_2Damage = 0.5f;
    public const float SmallCalliberFalloffPoint_3Damage = 0.3f;

    public const float MediumCalliberFalloffPoint_1 = 60f;
    public const float MediumCalliberFalloffPoint_2 = 90f;
    public const float MediumCalliberFalloffPoint_3 = 110f;
    public const float MediumCalliberFalloffPoint_End = 250f;
    public const float MediumCalliberFalloffPoint_1Damage = 0.8f;
    public const float MediumCalliberFalloffPoint_2Damage = 0.5f;
    public const float MediumCalliberFalloffPoint_3Damage = 0.3f;

    public const float LargeCalliberFalloffPoint_1 = 90f;
    public const float LargeCalliberFalloffPoint_2 = 115f;
    public const float LargeCalliberFalloffPoint_3 = 150f;
    public const float LargeCalliberFalloffPoint_End = 300f;
    public const float LargeCalliberFalloffPoint_1Damage = 0.8f;
    public const float LargeCalliberFalloffPoint_2Damage = 0.5f;
    public const float LargeCalliberFalloffPoint_3Damage = 0.3f;


    public static float GetDamageFalloffModifier(HitInfo hitInfo, Vector3 positionAtHit)
    {
        Vector3 difference = positionAtHit - hitInfo.locationOfDamageSource;
        float distance = Mathf.Abs(difference.magnitude);
        Debug.Log($"Distance:   {distance}");

        switch (hitInfo.weaponCalliber)
        {
            case WeaponCalliber.SmallCalliber:
                if (distance < SmallCalliberFalloffPoint_1) return 1f;
                else if (distance < SmallCalliberFalloffPoint_2) return SmallCalliberFalloffPoint_1Damage;
                else if (distance < SmallCalliberFalloffPoint_3) return SmallCalliberFalloffPoint_2Damage;
                else if (distance < SmallCalliberFalloffPoint_End) return SmallCalliberFalloffPoint_3Damage;
                else return 0f;
            case WeaponCalliber.MediumCalliber:
                if (distance < MediumCalliberFalloffPoint_1) return 1f;
                else if (distance < MediumCalliberFalloffPoint_2) return SmallCalliberFalloffPoint_1Damage;
                else if (distance < MediumCalliberFalloffPoint_3) return SmallCalliberFalloffPoint_2Damage;
                else if (distance < MediumCalliberFalloffPoint_End) return SmallCalliberFalloffPoint_3Damage;
                else return 0f;
            case WeaponCalliber.LargeCalliber:
                if (distance < LargeCalliberFalloffPoint_1) return 1f;
                else if (distance < LargeCalliberFalloffPoint_2) return SmallCalliberFalloffPoint_1Damage;
                else if (distance < LargeCalliberFalloffPoint_3) return SmallCalliberFalloffPoint_2Damage;
                else if (distance < LargeCalliberFalloffPoint_End) return SmallCalliberFalloffPoint_3Damage;
                else return 0f;
            default:
                return 0f;
        }
    }
}
