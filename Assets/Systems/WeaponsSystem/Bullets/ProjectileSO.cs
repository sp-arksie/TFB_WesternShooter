using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Projectile")]
public class ProjectileSO : ScriptableObject
{
    public WeaponCalliber calliber;
    public DamageType bulletType;

    [Space(10)]
    public float baseDamage;
}
