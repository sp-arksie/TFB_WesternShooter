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
    [SerializeField] bool receivesStatusEffects = true;

    public enum ActionOnHealthDepleted
    {
        None,
        Deactivate,
        Destroy,
        EnableRagdoll,
    }
    [SerializeField] ActionOnHealthDepleted actionOnHealthDepleted = ActionOnHealthDepleted.Deactivate;

    float currentHealth;
    float burnedHealth = 0f;
    float currentDamageAbsorb = 1f;
    float currentRegenerationTime = 0f;

    RagdollController ragdollController;
    DamageReceiver[] damageReceivers;
    StatusEffectsController statusEffectsController;

    Coroutine applyHealthRegeneration;
    Coroutine applyDamageAbsorb;

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
            statusEffectsController.ApplyStatusEffect(StatusEffect.Bleeding);
        }
        if (debugCureBleeding)
        {
            debugCureBleeding = false;
            StatusEffect[] se = new StatusEffect[1];
            se[0] = StatusEffect.Bleeding;
            statusEffectsController.ApplyStatusEffectsCures(se);
        }
        if(debugApplyBurning)
        {
            debugApplyBurning = false;
            statusEffectsController.ApplyStatusEffect(StatusEffect.Burning);
        }
        if(debugCureBurning)
        {
            debugCureBurning = false;
            StatusEffect[] se = new StatusEffect[1];
            se[0] = StatusEffect.Burning;
            statusEffectsController.ApplyStatusEffectsCures(se);
        }
        if (debugApplyPoison)
        {
            debugApplyPoison = false;
            statusEffectsController.ApplyStatusEffect(StatusEffect.Poison);
        }
        if (debugCurePoison)
        {
            debugCurePoison = false;
            StatusEffect[] se = new StatusEffect[1];
            se[0] = StatusEffect.Poison;
            statusEffectsController.ApplyStatusEffectsCures(se);
        }
    }

    private void Update()
    {
        if (debugHP)
        {
            if (debugDAbsorbDuration > 0f)
            {
                float t = 0f;
                t += Time.deltaTime;
                debugDAbsorbDuration -= t;
                debugdAbsorbDuration.text = debugDAbsorbDuration.ToString();
                if (debugDAbsorbDuration <= 0f) debugdAbsorbDuration.text = "0";
            }
            if (currentRegenerationTime > 0f)
            {
                float t = 0f;
                t += Time.deltaTime;
                currentRegenerationTime -= t;
                debugRegenTime.text = currentRegenerationTime.ToString();
                if (currentRegenerationTime <= 0f) debugRegenTime.text = "0";
            }
            debugHP.text = currentHealth.ToString();
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
        statusEffectsController = GetComponent<StatusEffectsController>();

        //DEBUG
        if (debugHP)
        {
            debugHP.text = $"HP: {currentHealth}";
            debugDAbsorbVal.text = currentDamageAbsorb.ToString();
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
            float finaldamage = GetDamage(hitInfo, damageModifierValue);
            ReduceHealth(finaldamage);

            if (hitInfo.statusEffect != StatusEffect.None && receivesStatusEffects) { statusEffectsController.ApplyStatusEffect(hitInfo.statusEffect); }

            Debug.Log($"BaseDamage:  {hitInfo.baseDamage}");
            Debug.Log($"Bodypart damage modifier:  {damageModifierValue}");
            Debug.Log($"Final damage:  {GetDamage(hitInfo, damageModifierValue)}");

            if (debugHP) debugHP.text = $"HP: {currentHealth}";
        }
    }

    private float GetDamage(HitInfo hitInfo, float damageModifier)
    {
        float damage = 0f;
        if(hitInfo.weaponCalliber == WeaponCalliber.None)
        {
            damage = currentDamageAbsorb * hitInfo.baseDamage;
        }
        else if(hitInfo.weaponCalliber == WeaponCalliber.Melee)
        {
            damage = currentDamageAbsorb * hitInfo.baseDamage * damageModifier;
        }
        else
        {
            damage = currentDamageAbsorb * hitInfo.baseDamage * damageModifier * DamageFalloffDefinitions.GetDamageFalloffModifier(hitInfo, transform.position);
        }
        
        return damage;
    }

    #region Health
    private void ReduceHealth(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            OnHealthDepleted();
    }


    public void NotifyReduceHealth(float amount)
    {
        ReduceHealth(amount);
    }

    private void RestoreHealth(float amount)
    {
        currentHealth += amount;

        if(currentHealth > baseHealth - burnedHealth)
            currentHealth = baseHealth - burnedHealth;
    }

    public void NotifyRestoreHealth(float amount)
    {
        RestoreHealth(amount);
    }

    public void NotifyRegenerateHealth(float regenerationRate, float duration)
    {
        if (applyHealthRegeneration != null) { StopCoroutine(applyHealthRegeneration); }
        applyHealthRegeneration = StartCoroutine(ApplyRegeneration(regenerationRate, duration));
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
            RestoreHealth(regenerationRate);
            yield return new WaitForSeconds(1f);
        }
        currentRegenerationTime = 0f;
        yield return null;
    }
    #endregion

    #region Burned health
    private void IncreaseBurnedHealth(float amount)
    {
        burnedHealth += amount;

        if (burnedHealth >= baseHealth)
            OnHealthDepleted();
    }

    public void NotifyIncreaseBurnedHealth(float amount)
    {
        IncreaseBurnedHealth(amount);
    }

    private void RecoverBurnedHealth(float amount)
    {
        burnedHealth -= amount;

        if(burnedHealth < 0)
            burnedHealth = 0;
    }

    public void NotifyRecoverBurnedHealth(float amount)
    {
        RecoverBurnedHealth(amount);
    }
    #endregion

    #region Damage absorb
    public void NotifyDamageAbsorbChange(float newValue, float duration)
    {
        if (currentDamageAbsorb < newValue)
        {
            if (applyDamageAbsorb != null) { StopCoroutine(applyDamageAbsorb); }
            applyDamageAbsorb = StartCoroutine(DamageAbsorbCoroutine(newValue, duration));
        }
    }

    private IEnumerator DamageAbsorbCoroutine(float newValue, float duration)
    {
        currentDamageAbsorb = newValue;
        yield return new WaitForSeconds(duration);
        currentDamageAbsorb = 1f;
        yield return null;
    }
    #endregion

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
