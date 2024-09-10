using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBehaviour : MonoBehaviour
{
    [SerializeField] float baseDamage = 50f;

    public float GetBaseDamage() { return baseDamage; }
}
