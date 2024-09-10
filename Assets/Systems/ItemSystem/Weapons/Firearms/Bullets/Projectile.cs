using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] ProjectileType _projectileType;
    [SerializeField] WeaponCalliber _matchingCalliber;
    [Space(10)]
    [SerializeField] ParticleSystem.MinMaxCurve _damageFalloff;

    public ProjectileType projectileType { get => _projectileType; private set => _projectileType = value; }
    public WeaponCalliber matchingCalliber { get => _matchingCalliber; private set => _matchingCalliber = value; }
    public ParticleSystem.MinMaxCurve damageFalloff { get => _damageFalloff; private set => _damageFalloff = value; }

    public class ProjectileBehaviour
    {
        public float velocityModifier;
        public StatusEffect statusEffect;
    }


    public enum ProjectileType
    {
        Compact,
        CompactHighVelocity,
        CompactIncendiary,
        CompactPoison,
        CompactHollowPoint,

        Medium,
        MediumHighVelocity,
        MediumIncendiary,
        MediumPoison,
        MediumHollowPoint,

        Long,
        LongHighVelocity,
        LongIncendiary,
        LongPoison,
        LongHollowPoint,

        Buckshot,
        BuckshotFlechettes,
        BuckshotDragonsBreath,
    }

    public ProjectileBehaviour GetProjectileBehaviour()
    {
        ProjectileBehaviour projectileInfo = new();

        switch (projectileType)
        {
            case ProjectileType.Compact:
                projectileInfo.velocityModifier = 1f;
                projectileInfo.statusEffect = StatusEffect.None;
                break;
            case ProjectileType.CompactHighVelocity:
                projectileInfo.velocityModifier = 1.25f;
                projectileInfo.statusEffect = StatusEffect.None;
                break;
            case ProjectileType.CompactIncendiary:
                projectileInfo.velocityModifier = 0.8f;
                projectileInfo.statusEffect = StatusEffect.Burning;
                break;
            case ProjectileType.CompactPoison:
                projectileInfo.velocityModifier = 0.9f;
                projectileInfo.statusEffect = StatusEffect.Poison;
                break;
            case ProjectileType.CompactHollowPoint:
                projectileInfo.velocityModifier = 0.85f;
                projectileInfo.statusEffect = StatusEffect.Bleeding;
                break;

            case ProjectileType.Medium:
                projectileInfo.velocityModifier = 1f;
                projectileInfo.statusEffect = StatusEffect.None;
                break;
            case ProjectileType.MediumHighVelocity:
                projectileInfo.velocityModifier = 1.3f;
                projectileInfo.statusEffect = StatusEffect.None;
                break;
            case ProjectileType.MediumIncendiary:
                projectileInfo.velocityModifier = 0.75f;
                projectileInfo.statusEffect = StatusEffect.Burning;
                break;
            case ProjectileType.MediumPoison:
                projectileInfo.velocityModifier = 0.88f;
                projectileInfo.statusEffect = StatusEffect.Poison;
                break;
            case ProjectileType.MediumHollowPoint:
                projectileInfo.velocityModifier = 0.83f;
                projectileInfo.statusEffect = StatusEffect.Bleeding;
                break;

            case ProjectileType.Long:
                projectileInfo.velocityModifier = 1f;
                projectileInfo.statusEffect = StatusEffect.None;
                break;
            case ProjectileType.LongHighVelocity:
                projectileInfo.velocityModifier = 1.5f;
                projectileInfo.statusEffect = StatusEffect.None;
                break;
            case ProjectileType.LongIncendiary:
                projectileInfo.velocityModifier = 0.7f;
                projectileInfo.statusEffect = StatusEffect.Burning;
                break;
            case ProjectileType.LongPoison:
                projectileInfo.velocityModifier = 0.87f;
                projectileInfo.statusEffect = StatusEffect.Poison;
                break;
            case ProjectileType.LongHollowPoint:
                projectileInfo.velocityModifier = 0.8f;
                projectileInfo.statusEffect = StatusEffect.Bleeding;
                break;

            case ProjectileType.Buckshot:
                projectileInfo.velocityModifier = 1f;
                projectileInfo.statusEffect = StatusEffect.None;
                break;
            case ProjectileType.BuckshotFlechettes:
                projectileInfo.velocityModifier = 0.9f;
                projectileInfo.statusEffect = StatusEffect.Bleeding;
                break;
            case ProjectileType.BuckshotDragonsBreath:
                projectileInfo.velocityModifier = 0.9f;
                projectileInfo.statusEffect = StatusEffect.Burning;
                break;
        }

        return projectileInfo;
    }

    public ProjectileType GetBulletType() { return projectileType; }
}
