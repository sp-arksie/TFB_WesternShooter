using UnityEngine;

public class HitInfo
{
    public float baseDamage;

    public Vector3 locationOfDamageSource;

    public WeaponCalliber weaponCalliber;

    public ParticleSystem.MinMaxCurve damageFalloffCurve;

    public StatusEffect statusEffect;
}
