using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] float baseHealth = 100f;
    [SerializeField] DamageReceiver[] damageReceivers;

    public enum ActionOnHealthDepleted
    {
        None,
        Deactivate,
        Destroy
    }
    [SerializeField] ActionOnHealthDepleted actionOnHealthDepleted = ActionOnHealthDepleted.Deactivate;

    float currentHealth;

    private void Awake()
    {
        currentHealth = baseHealth;
    }

    private void OnEnable()
    {
        foreach(DamageReceiver dr in damageReceivers)
        {
            dr.onHit += OnHit;
        }
    }

    private void OnDisable()
    {
        foreach(DamageReceiver dr in damageReceivers)
        {
            dr.onHit -= OnHit;
        }
    }

    private void OnHit(HitInfo hitInfo, float damageModifier)
    {
        if(currentHealth > 0)
        {
            currentHealth -= GetDamage(hitInfo, damageModifier);
            Debug.Log($"BaseDamage:  {hitInfo.baseDamage}");
            Debug.Log($"Bodypart damage modifier:  {damageModifier}");
            Debug.Log($"Final damage:   {hitInfo.baseDamage * DamageFalloffDefinitions.GetDamageFalloffModifier(hitInfo, transform.position) * damageModifier}");
            if(currentHealth <= 0)
            {
                OnHealthDepleted();
            }
        }
    }

    private float GetDamage(HitInfo hitInfo, float damageModifier)
    {
        float damage = 0f;
        if(hitInfo.weaponCalliber == WeaponCalliber.None)
        {
            damage = hitInfo.baseDamage;
        }
        else if(hitInfo.weaponCalliber == WeaponCalliber.Melee)
        {
            damage = hitInfo.baseDamage * damageModifier;
        }
        else
        {
            damage = hitInfo.baseDamage * damageModifier * DamageFalloffDefinitions.GetDamageFalloffModifier(hitInfo, transform.position);
        }
        
        return damage;
    }

    private void OnHealthDepleted()
    {
        switch (actionOnHealthDepleted)
        {
            case ActionOnHealthDepleted.None:
                break;
            case ActionOnHealthDepleted.Deactivate:
                gameObject.SetActive(false);
                break;
            case ActionOnHealthDepleted.Destroy:
                Destroy(gameObject);
                break;
        }
    }
}
