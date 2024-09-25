using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    Rigidbody rb;

    public void Shoot(ShootingBehaviour shootingBehaviour, GameObject prefabToInstantiate, Projectile projectile, HitInfo hitInfoToSend)
    {
        if(shootingBehaviour.ApplySpread == false)
        {
            for (int i = 0; i < shootingBehaviour.BulletsPerShot; i++)
            {
                GameObject go = Instantiate(prefabToInstantiate, transform.position, transform.localRotation);
                go.GetComponent<DamageGiver>().SetHitInfo(hitInfoToSend);
                rb = go.GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * shootingBehaviour.MuzzleVelocity * projectile.VelocityModifier, ForceMode.VelocityChange);
            }
        }
        else
        {
            for (int i = 0; i < shootingBehaviour.BulletsPerShot; i++)
            {
                GameObject go = Instantiate(prefabToInstantiate, transform.position, transform.localRotation);
                go.GetComponent<DamageGiver>().SetHitInfo(hitInfoToSend);
                rb = go.GetComponent<Rigidbody>();
                rb.AddForce(GetSpread(shootingBehaviour) * transform.forward * shootingBehaviour.MuzzleVelocity * projectile.VelocityModifier, ForceMode.VelocityChange);
            }
        }
    }

    private Quaternion GetSpread(ShootingBehaviour shootingBehaviour)
    {
        float maxYRotation = shootingBehaviour.MaxSpread.x;
        float maxXRotation = shootingBehaviour.MaxSpread.y;

        float degreesY = Random.Range(-maxYRotation, maxYRotation);
        float degreesX = Random.Range(-maxXRotation, maxXRotation);

        Quaternion rotY = Quaternion.AngleAxis(degreesY, Vector3.up);
        Quaternion rotX = Quaternion.AngleAxis(degreesX, Vector3.right);

        Quaternion rotationToApply = transform.localRotation * rotY * rotX;
        
        return rotationToApply;
    }
}
