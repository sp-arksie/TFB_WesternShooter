using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] float damageModifer = 1f;

    public event Action<HitInfo, float> onHit;

    public void NotifyHit(IHitNotifier hitNotifier)
    {
        HitInfo hitInfo = hitNotifier.GetHitInfo();
        onHit?.Invoke(hitInfo, damageModifer);
    }
}
