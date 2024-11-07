using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffController : MonoBehaviour
{
    Dictionary<StatusEffect, bool> currentStatusEffects = new Dictionary<StatusEffect, bool>();

    float statusEffectTickRate = 1f;

    Coroutine applyBleeding;
    Coroutine applyBurning;
    Coroutine applyPoison;

    HealthController healthController;

    private void Awake()
    {
        foreach(StatusEffect se in Enum.GetValues(typeof(StatusEffect)))
        {
            currentStatusEffects.Add(se, false);
        }

        healthController = GetComponent<HealthController>();
    }


    public void ApplyDebuff(StatusEffect statusEffect)
    {
        switch (statusEffect)
        {
            case StatusEffect.Bleeding:
                if (currentStatusEffects[StatusEffect.Bleeding] == false)
                {
                    currentStatusEffects[StatusEffect.Bleeding] = true;
                    applyBleeding = StartCoroutine(ApplyBleeding(statusEffect));
                }
                break;
            case StatusEffect.Burning:
                if (currentStatusEffects[StatusEffect.Burning] == false)
                {
                    currentStatusEffects[StatusEffect.Burning] = true;
                    applyBurning = StartCoroutine(ApplyBurning(statusEffect));
                }
                break;
            case StatusEffect.Poison:
                if (currentStatusEffects[StatusEffect.Poison] == false && applyPoison != null) StopCoroutine(applyPoison);
                currentStatusEffects[StatusEffect.Poison] = true;
                applyPoison = StartCoroutine(ApplyPoison());
                break;
            default:
                Debug.LogWarning($"{statusEffect} not implemented in ApplyStatusEffect in {this}");
                break;
        }
    }

    private IEnumerator ApplyBleeding(StatusEffect statusEffect)
    {
        float effectStart = Time.time;
        float elapsedTime = 0f;

        while (currentStatusEffects[StatusEffect.Bleeding] == true)
        {
            elapsedTime = Time.time - effectStart;
            float damage = StatusEffectsDefinitions.GetStatusEffectDamage(statusEffect, elapsedTime);
            healthController.NotifyReduceHealth(damage);
            yield return new WaitForSeconds(statusEffectTickRate);
        }
    }

    private IEnumerator ApplyBurning(StatusEffect statusEffect)
    {
        float effectStart = Time.time;
        float elapsedTime = 0f;
        while (currentStatusEffects[StatusEffect.Burning] == true)
        {
            elapsedTime = Time.time - effectStart;
            float damage = StatusEffectsDefinitions.GetStatusEffectDamage(statusEffect, elapsedTime);
            healthController.NotifyIncreaseBurnedHealth(damage);
            healthController.NotifyReduceHealth(damage);
            yield return new WaitForSeconds(statusEffectTickRate);
        }
    }

    private IEnumerator ApplyPoison()
    {
        yield return new WaitForSeconds(StatusEffectsDefinitions.poisonEffectDuration);
        currentStatusEffects[StatusEffect.Poison] = false;
    }

    public void ApplyStatusEffectsCures(StatusEffect[] statusEffectsToCure)
    {
        foreach (StatusEffect se in statusEffectsToCure)
        {
            CureStatusEffect(se);
        }
    }

    private void CureStatusEffect(StatusEffect statusEffect)
    {
        switch (statusEffect)
        {
            case StatusEffect.Bleeding:
                currentStatusEffects[StatusEffect.Bleeding] = false;
                if(applyBleeding != null) StopCoroutine(applyBleeding);
                break;
            case StatusEffect.Burning:
                currentStatusEffects[StatusEffect.Burning] = false;
                if(applyBurning != null) StopCoroutine(applyBurning);
                break;
            case StatusEffect.Poison:
                currentStatusEffects[StatusEffect.Poison] = false;
                if(applyPoison != null) StopCoroutine(applyPoison);
                break;
            default:
                Debug.LogWarning($"{statusEffect} cure not implemented in {this}");
                break;
        }
    }
}
