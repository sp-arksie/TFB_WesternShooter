using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.AI;
using static DamageModifierDefinitions;

public class HealthController : MonoBehaviour
{
    [SerializeField] float baseHealth = 100f;
    [SerializeField] Transform damageReceiversParent;

    public enum ActionOnHealthDepleted
    {
        None,
        Deactivate,
        Destroy,
        EnableRagdoll,
    }
    [SerializeField] ActionOnHealthDepleted actionOnHealthDepleted = ActionOnHealthDepleted.Deactivate;

    RagdollController ragdollController;
    DamageReceiver[] damageReceivers;

    float currentHealth;

    private void Awake()
    {
        currentHealth = baseHealth;

        damageReceivers = damageReceiversParent.GetComponentsInChildren<DamageReceiver>();
        if(actionOnHealthDepleted == ActionOnHealthDepleted.EnableRagdoll)
        {
            ragdollController = GetComponent<RagdollController>();
        }
    }

    private void OnEnable()
    {
        foreach(DamageReceiver dr in damageReceivers)
        {
            dr.OnHit += OnHit;
        }
    }

    private void OnDisable()
    {
        foreach(DamageReceiver dr in damageReceivers)
        {
            dr.OnHit -= OnHit;
        }
    }

    private void OnHit(HitInfo hitInfo, DamageModifier damageModifier)
    {
        if(currentHealth > 0)
        {
            float damageModifierValue = GetDamageModifierValue(damageModifier);
            currentHealth -= GetDamage(hitInfo, damageModifierValue);
            Debug.Log($"BaseDamage:  {hitInfo.baseDamage}");
            Debug.Log($"Bodypart damage modifier:  {damageModifierValue}");
            Debug.Log($"Final damage:  {GetDamage(hitInfo, damageModifierValue)}");
            
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
            case ActionOnHealthDepleted.EnableRagdoll:
                ragdollController.EnableRagdoll();
                break;
        }
    }


}
