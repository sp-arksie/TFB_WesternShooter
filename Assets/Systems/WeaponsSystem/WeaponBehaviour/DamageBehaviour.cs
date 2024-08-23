using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBehaviour : MonoBehaviour
{
    [SerializeField] float baseDamage = 50f;
    [SerializeField] WeaponCalliber weaponCalliber;

    public float GetBaseDamage() { return baseDamage; }
    public WeaponCalliber GetWeaponCalliber() { return weaponCalliber; }
}
