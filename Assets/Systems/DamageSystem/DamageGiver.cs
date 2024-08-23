using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGiver : MonoBehaviour, IHitNotifier
{
    private void OnTriggerEnter(Collider other)
    {
        CheckCollider(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        CheckCollider(other.collider);
    }

    private void CheckCollider(Collider other)
    {
        other.GetComponent<DamageReceiver>()?.NotifyHit(this);
    }

    HitInfo IHitNotifier.GetHitInfo()
    {
        throw new System.NotImplementedException();
    }
}
