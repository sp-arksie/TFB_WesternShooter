using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    HealthController healthController;

    float currentRegenerationTime = 0f;
    float currentDamageAbsorb = 1f;

    Coroutine applyHealthRegeneration;
    Coroutine applyDamageAbsorb;

    private void Awake()
    {
        healthController = GetComponent<HealthController>();
    }

    public void ApplyBuff(MedicalItem source)
    {
        if (source.DoesRestoreHealth)
        {
            healthController.NotifyRestoreHealth(source.HealthRestoreAmount);
        }
        if (source.DoesRestoreBurnedHealth)
        {
            healthController.NotifyRecoverBurnedHealth(source.BurnedHealthRestoreAmount);
        }
        if (source.DoesDamageAbsorb)
        {
            if (currentDamageAbsorb > source.DamageAbsorbPercent)
            {
                if (applyDamageAbsorb != null) { StopCoroutine(applyDamageAbsorb); }
                applyDamageAbsorb = StartCoroutine(ApplyDamageAbsorb(source.DamageAbsorbPercent, source.DamageAbsorbDuration));
            }
        }
        if (source.DoesHealthRegeneration)
        {
            if (applyHealthRegeneration != null) { StopCoroutine(applyHealthRegeneration); }
            applyHealthRegeneration = StartCoroutine(ApplyRegeneration(source.RegenerationPerSecond, source.RegenerationPerSecond));
        }
        if (source.DoesCureDebuffs)
        {
            healthController.NotifyDebuffCure(source.DebuffCures);
        }
    }

    private IEnumerator ApplyRegeneration(float regenerationRate, float duration)
    {
        currentRegenerationTime += duration;
        float startTime = Time.time;
        float t = 0f;

        while (currentRegenerationTime > 0)
        {
            t += Time.deltaTime;
            currentRegenerationTime -= t;
            healthController.NotifyRestoreHealth(regenerationRate);
            yield return new WaitForSeconds(1f);
        }
        currentRegenerationTime = 0f;
        yield return null;
    }

    private IEnumerator ApplyDamageAbsorb(float newValue, float duration)
    {
        healthController.NotifyDamageAbsorbChange(newValue);
        yield return new WaitForSeconds(duration);
        healthController.NotifyDamageAbsorbChange(1f);
        yield return null;
    }
}
