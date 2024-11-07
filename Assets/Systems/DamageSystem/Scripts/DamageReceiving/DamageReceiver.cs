using System;
using UnityEngine;
using static DamageModifierDefinitions;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] DamageModifier damageModifier = DamageModifier.None;
    public DamageModifier DamageModifier { get => damageModifier; }

    public event Action<HitInfo, DamageModifier, Rigidbody> OnHit;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void NotifyHit(IHitNotifier hitNotifier)
    {
        HitInfo hitInfo = hitNotifier.GetHitInfo();
        OnHit?.Invoke(hitInfo, damageModifier, rb);
    }
}
