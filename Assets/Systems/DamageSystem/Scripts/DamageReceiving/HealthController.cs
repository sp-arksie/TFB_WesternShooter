using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

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

    float statusEffectTickRate = 1f;
    float burnedHealth = 0f;
    bool isBleeding = false;
    bool isBurning = false;
    bool isPoisoned = false;
    Coroutine applyBleeding;
    Coroutine applyBurning;
    Coroutine applyPoison;

    float damageAbsorb = 1f;
    Coroutine damageAbsorbCoroutine;

    float currentRegenerationTime = 0f;
    Coroutine healthRegenerationCoroutine;

    #region DEBUG

    [Header("DEBUG")]
    [SerializeField] float debugDamageAmount = 50f;
    [SerializeField] DamageModifierDefinitions.DamageModifier debugDamageModifier = DamageModifierDefinitions.DamageModifier.None;
    [SerializeField] WeaponCalliber debugCalliber = WeaponCalliber.None;
    [SerializeField] StatusEffect debugStatustatusEffect = StatusEffect.None;
    [SerializeField] bool debugDealDamage = false;
    HitInfo hitInfo = new();

    [Header("DEBUG - UI")]
    [SerializeField] TextMeshProUGUI debugHP;
    [SerializeField] TextMeshProUGUI debugDAbsorbVal;
    [SerializeField] TextMeshProUGUI debugdAbsorbDuration;
    [SerializeField] TextMeshProUGUI debugRegenVal;
    [SerializeField] TextMeshProUGUI debugRegenTime;
    [SerializeField] TextMeshProUGUI debugBurningDamage;
    float debugDAbsorbDuration = 0f;

    [Header("DEBUG - Status effects")]
    [SerializeField] bool debugApplyBleeding = false;
    [SerializeField] bool debugCureBleeding = false;
    [SerializeField] bool debugApplyBurning = false;
    [SerializeField] bool debugCureBurning = false;
    [SerializeField] bool debugApplyPoison = false;
    [SerializeField] bool debugCurePoison = false;

    private void OnValidate()
    {
        if (debugDealDamage)
        {
            debugDealDamage = false;
            hitInfo.locationOfDamageSource = transform.position;
            hitInfo.baseDamage = debugDamageAmount;
            hitInfo.weaponCalliber = debugCalliber;
            hitInfo.statusEffect = debugStatustatusEffect;
            OnHit(hitInfo, debugDamageModifier);
        }
        if(debugApplyBleeding)
        {
            debugApplyBleeding = false;
            ApplyStatusEffect(StatusEffect.Bleeding);
        }
        if (debugCureBleeding)
        {
            debugCureBleeding = false;
            StatusEffect[] se = new StatusEffect[1];
            se[0] = StatusEffect.Bleeding;
            ApplyStatusEffectsCures(se);
        }
        if(debugApplyBurning)
        {
            debugApplyBurning = false;
            ApplyStatusEffect(StatusEffect.Burning);
        }
        if(debugCureBurning)
        {
            debugCureBurning = false;
            StatusEffect[] se = new StatusEffect[1];
            se[0] = StatusEffect.Burning;
            ApplyStatusEffectsCures(se);
        }
        if (debugApplyPoison)
        {
            debugApplyPoison = false;
            ApplyStatusEffect(StatusEffect.Poison);
        }
        if (debugCurePoison)
        {
            debugCurePoison = false;
            StatusEffect[] se = new StatusEffect[1];
            se[0] = StatusEffect.Poison;
            ApplyStatusEffectsCures(se);
        }
    }

    private void Update()
    {
        if(debugDAbsorbDuration > 0f)
        {
            float t = 0f;
            t += Time.deltaTime;
            debugDAbsorbDuration -= t;
            debugdAbsorbDuration.text = debugDAbsorbDuration.ToString();
            if (debugDAbsorbDuration <= 0f) debugdAbsorbDuration.text = "0";
        }
        if(currentRegenerationTime > 0f)
        {
            float t = 0f;
            t += Time.deltaTime;
            currentRegenerationTime -= t;
            debugRegenTime.text = currentRegenerationTime.ToString();
            if(currentRegenerationTime <= 0f) debugRegenTime.text = "0";
        }
    }

    #endregion

    private void Awake()
    {
        currentHealth = baseHealth;

        damageReceivers = damageReceiversParent.GetComponentsInChildren<DamageReceiver>();
        if(actionOnHealthDepleted == ActionOnHealthDepleted.EnableRagdoll)
        {
            ragdollController = GetComponent<RagdollController>();
        }

        //DEBUG
        if (debugHP)
        {
            debugHP.text = $"HP: {currentHealth}";
            debugDAbsorbVal.text = damageAbsorb.ToString();
            debugdAbsorbDuration.text = "-";
            debugRegenVal.text = "0";
            debugRegenTime.text = "-";
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

    private void OnHit(HitInfo hitInfo, DamageModifierDefinitions.DamageModifier damageModifier)
    {
        if(currentHealth > 0)
        {
            float damageModifierValue = DamageModifierDefinitions.GetDamageModifierValue(damageModifier);
            currentHealth -= GetDamage(hitInfo, damageModifierValue);

            if (hitInfo.statusEffect != StatusEffect.None) { ApplyStatusEffect(hitInfo.statusEffect); }

            Debug.Log($"BaseDamage:  {hitInfo.baseDamage}");
            Debug.Log($"Bodypart damage modifier:  {damageModifierValue}");
            Debug.Log($"Final damage:  {GetDamage(hitInfo, damageModifierValue)}");

            if (debugHP) debugHP.text = $"HP: {currentHealth}";

            CheckHealth();
        }
    }

    private float GetDamage(HitInfo hitInfo, float damageModifier)
    {
        float damage = 0f;
        if(hitInfo.weaponCalliber == WeaponCalliber.None)
        {
            damage = damageAbsorb * hitInfo.baseDamage;
        }
        else if(hitInfo.weaponCalliber == WeaponCalliber.Melee)
        {
            damage = damageAbsorb * hitInfo.baseDamage * damageModifier;
        }
        else
        {
            damage = damageAbsorb * hitInfo.baseDamage * damageModifier * DamageFalloffDefinitions.GetDamageFalloffModifier(hitInfo, transform.position);
        }
        
        return damage;
    }

    private void CheckHealth()
    {
        if (currentHealth <= 0)
        {
            OnHealthDepleted();
        }
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

    #region Status effects

    private void ApplyStatusEffect(StatusEffect statusEffect)
    {
        switch (statusEffect)
        {
            case StatusEffect.Bleeding:
                if (!isBleeding) { isBleeding = true; applyBleeding = StartCoroutine(ApplyBleeding(statusEffect)); }
                break;
            case StatusEffect.Burning:
                if (!isBurning) { isBurning = true; applyBurning = StartCoroutine(ApplyBurning(statusEffect)); }
                break;
            case StatusEffect.Poison:
                if(applyPoison != null) StopCoroutine(applyPoison);
                isPoisoned = true;
                StartCoroutine(ApplyPoison());
                break;
        }
    }

    private IEnumerator ApplyBleeding(StatusEffect statusEffect)
    {
        float effectStart = Time.time;
        float elapsedTime = 0f;

        while (isBleeding)
        {
            elapsedTime = Time.time - effectStart;
            float damage = StatusEffectsDefinitions.GetStatusEffectDamage(statusEffect, elapsedTime);
            DealStatusEffectDamage(damage);
            yield return new WaitForSeconds(statusEffectTickRate);
        }
    }

    private IEnumerator ApplyBurning(StatusEffect statusEffect)
    {
        float effectStart = Time.time;
        float elapsedTime = 0f;
        while (isBurning)
        {
            elapsedTime = Time.time - effectStart;
            float damage = StatusEffectsDefinitions.GetStatusEffectDamage(statusEffect, elapsedTime);
            burnedHealth += damage;
            if(debugBurningDamage) debugBurningDamage.text = burnedHealth.ToString();
            DealStatusEffectDamage(damage);
            yield return new WaitForSeconds(statusEffectTickRate);
        }
    }

    private IEnumerator ApplyPoison()
    {
        isPoisoned = true;
        yield return new WaitForSeconds(StatusEffectsDefinitions.poisonEffectDuration);
        isPoisoned = false;
    }

    public void ApplyStatusEffectsCures(StatusEffect[] statusEffectsToCure)
    {
        foreach(StatusEffect se in statusEffectsToCure)
        {
            CureStatusEffect(se);
        }
    }

    private void CureStatusEffect(StatusEffect statusEffect)
    {
        switch (statusEffect)
        {
            case StatusEffect.Bleeding:
                isBleeding = false;
                StopCoroutine(applyBleeding);
                break;
            case StatusEffect.Burning:
                isBurning = false;
                StopCoroutine(applyBurning);
                break;
            case StatusEffect.Poison:
                isPoisoned = false;
                StopCoroutine(applyPoison);
                break;
            default:
                Debug.LogWarning($"{statusEffect} cure not implemented in {this}");
                break;
        }
    }

    private void DealStatusEffectDamage(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            if(debugHP) debugHP.text = $"HP: {currentHealth}";
            CheckHealth();
        }
    }

    #endregion

    #region Health restoring
    public void NotifyRestoreHealth(float amount)
    {
        RestoreHealth(amount);
    }

    private void RestoreHealth(float amount)
    {
        if(!isPoisoned)
        {
            currentHealth += amount;
            if (currentHealth > baseHealth) { currentHealth = baseHealth; }
            debugHP.text = $"HP: {currentHealth}";
        }
    }

    public void NotifyRegenerateHealth(float regenerationRate, float duration)
    {
        if(healthRegenerationCoroutine != null) { StopCoroutine(healthRegenerationCoroutine); }
        healthRegenerationCoroutine = StartCoroutine(HealthRegenerationCoroutine(regenerationRate, duration));
    }

    private IEnumerator HealthRegenerationCoroutine(float regenerationRate, float duration)
    {
        currentRegenerationTime += duration;
        float startTime = Time.time;
        float t = 0f;
        debugRegenVal.text = regenerationRate.ToString();

        while(currentRegenerationTime > 0)
        {
            t += Time.deltaTime;
            currentRegenerationTime -= t;
            RestoreHealth(regenerationRate);
            yield return new WaitForSeconds(1f);
        }
        currentRegenerationTime = 0f;
        debugRegenVal.text = "0";
        yield return null;
    }
    #endregion

    #region Damage absorb
    public void NotifyDamageAbsorbChange(float newValue, float duration)
    {
        if(damageAbsorbCoroutine != null) { StopCoroutine(damageAbsorbCoroutine); }
        damageAbsorbCoroutine = StartCoroutine(DamageAbsorbCoroutine(newValue, duration));
    }

    private IEnumerator DamageAbsorbCoroutine(float newValue, float duration)
    {
        damageAbsorb = newValue;
        debugDAbsorbVal.text = damageAbsorb.ToString();
        debugDAbsorbDuration = duration;
        yield return new WaitForSeconds(duration);
        damageAbsorb = 1f;
        debugDAbsorbVal.text = damageAbsorb.ToString();
        yield return null;
    }
    #endregion
}
