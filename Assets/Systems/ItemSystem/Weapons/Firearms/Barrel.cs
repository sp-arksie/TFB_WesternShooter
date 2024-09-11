using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    Rigidbody rb;

    public void Shoot(float muzzleVelocity, ItemInventoryMediator.ProjectileInfo projectileInfo, Projectile projectile, HitInfo hitInfoToSend)
    {
        GameObject go = Instantiate(projectileInfo.prefabToInstatiate, transform.position, transform.localRotation);
        go.GetComponent<DamageGiver>().SetHitInfo(hitInfoToSend);
        rb = go.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * muzzleVelocity * projectile.VelocityModifier, ForceMode.VelocityChange);
    }
}
