using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] GameObject projectile;

    Rigidbody rb;

    public void Shoot(float muzzleVelocity, HitInfo hitInfoToSend)
    {
        GameObject go = Instantiate(projectile, transform.position, transform.localRotation);
        go.GetComponent<DamageGiver>().SetHitInfo(hitInfoToSend);
        rb = go.GetComponent<Rigidbody>();
        rb.AddForce(this.transform.forward * muzzleVelocity, ForceMode.VelocityChange);
    }
}
