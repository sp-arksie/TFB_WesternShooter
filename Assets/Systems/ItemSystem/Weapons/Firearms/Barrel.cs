using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    Rigidbody rb;

    public void Shoot(float muzzleVelocity, float velocityModifier, GameObject prefabToInstantiate, HitInfo hitInfoToSend)
    {
        GameObject go = Instantiate(prefabToInstantiate, transform.position, transform.localRotation);
        go.GetComponent<DamageGiver>().SetHitInfo(hitInfoToSend);
        rb = go.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * muzzleVelocity * velocityModifier, ForceMode.VelocityChange);
    }
}
