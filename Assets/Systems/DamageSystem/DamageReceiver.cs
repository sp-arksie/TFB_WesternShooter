using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    public event Action<HitInfo> onHit;

    public void NotifyHit(IHitNotifier hitNotifier)
    {
        HitInfo hitInfo = hitNotifier.GetHitInfo();
        onHit.Invoke(hitInfo);
    }
}
