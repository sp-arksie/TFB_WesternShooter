using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBehaviour : MonoBehaviour
{
    [SerializeField] float baseDamage = 50f;
    [SerializeField] AnimationCurve damageFalloff;

    public float GetBaseDamage() { return baseDamage; }
    public AnimationCurve GetDamageFalloffCurve() { return damageFalloff; }
}
