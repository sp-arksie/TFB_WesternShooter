using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGiver : MonoBehaviour, IHitNotifier
{
    HitInfo hitInfoCarrier;

    private void OnTriggerEnter(Collider other)
    {
        CheckCollider(other);
        //this.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        CheckCollider(other.collider);
        //this.gameObject.SetActive(false);
    }

    private void CheckCollider(Collider other)
    {
        other.GetComponent<DamageReceiver>()?.NotifyHit(this);
    }

    public void SetHitInfo(HitInfo hitInfo)
    {
        hitInfoCarrier = hitInfo;
    }

    HitInfo IHitNotifier.GetHitInfo()
    {
        return hitInfoCarrier;
    }
}
