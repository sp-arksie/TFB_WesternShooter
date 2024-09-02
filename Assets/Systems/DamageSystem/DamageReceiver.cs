using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DamageModifierDefinitions;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] DamageModifier damageModifier = DamageModifier.None;

    public event Action<HitInfo, DamageModifier> OnHit;

    public void NotifyHit(IHitNotifier hitNotifier)
    {
        HitInfo hitInfo = hitNotifier.GetHitInfo();
        OnHit?.Invoke(hitInfo, damageModifier);
    }
}
