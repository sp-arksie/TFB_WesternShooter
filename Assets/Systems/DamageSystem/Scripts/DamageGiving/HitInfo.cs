using UnityEngine;

public class HitInfo
{
    public float baseDamage;

    public float damageModifier;

    public Vector3 locationOfDamageSource;

    public WeaponCalliber weaponCalliber;

    public AnimationCurve damageFalloffCurve;

    public StatusEffect statusEffect;
}
