using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] ProjectileType _projectileType;
    [SerializeField] WeaponCalliber matchingCalliber;
    [Space(10)]
    [Header("Behaviour")]
    [SerializeField] ParticleSystem.MinMaxCurve damageFalloff;
    [SerializeField] float velocityModifier = 1f;
    [SerializeField] StatusEffect statusEffect;

    public ProjectileType projectileType { get => _projectileType; private set => _projectileType = value; }
    public WeaponCalliber MatchingCalliber { get => matchingCalliber; private set => matchingCalliber = value; }
    public ParticleSystem.MinMaxCurve DamageFalloff { get => damageFalloff; private set => damageFalloff = value; }
    public float VelocityModifier { get => velocityModifier; private set => velocityModifier = value; }
    public StatusEffect StatusEffect { get => statusEffect; private set => statusEffect = value; }
}
