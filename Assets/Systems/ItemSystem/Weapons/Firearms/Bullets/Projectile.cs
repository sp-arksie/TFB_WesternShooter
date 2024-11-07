using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] ProjectileType _projectileType;
    [SerializeField] WeaponCalliber matchingCalliber;
    [Space(10)]
    [Header("Behaviour")]
    [SerializeField] float damageModifier = 1f;
    [SerializeField] float velocityModifier = 1f;
    [SerializeField] StatusEffect statusEffect;

    public ProjectileType projectileType { get => _projectileType; private set => _projectileType = value; }
    public WeaponCalliber MatchingCalliber { get => matchingCalliber; private set => matchingCalliber = value; }
    public float DamageModifier { get => damageModifier; private set => damageModifier = value; }
    public float VelocityModifier { get => velocityModifier; private set => velocityModifier = value; }
    public StatusEffect StatusEffect { get => statusEffect; private set => statusEffect = value; }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
