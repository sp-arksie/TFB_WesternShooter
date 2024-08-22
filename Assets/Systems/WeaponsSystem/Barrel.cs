using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] GameObject projectile;

    Rigidbody rb;

    public void Shoot(float muzzleVelocity)
    {
        GameObject go = Instantiate(projectile, transform.position, transform.rotation);
        rb = go.GetComponent<Rigidbody>();
        rb.AddForce(this.transform.forward * muzzleVelocity, ForceMode.VelocityChange);
    }
}
