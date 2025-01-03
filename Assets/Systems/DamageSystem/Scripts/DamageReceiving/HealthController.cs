using System;
using TMPro;
using UnityEditor;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] float baseHealth = 100f;
    [SerializeField] Transform damageReceiversParent;
    [SerializeField] bool receivesStatusEffects = true;

    public event Action OnHealthDepletedEvent;

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

    RagdollController ragdollController;
    DamageReceiver[] damageReceivers;
    DebuffController debuffController;
    BuffController buffController;

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
    //float debugDAbsorbDuration = 0f;

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
            //OnHit(hitInfo, debugDamageModifier);
        }
        if(debugApplyBleeding)
        {
            debugApplyBleeding = false;
            debuffController.ApplyDebuff(StatusEffect.Bleeding);
        }
        if (debugCureBleeding)
        {
            debugCureBleeding = false;
            StatusEffect[] se = new StatusEffect[1];
            se[0] = StatusEffect.Bleeding;
            debuffController.ApplyStatusEffectsCures(se);
        }
        if(debugApplyBurning)
        {
            debugApplyBurning = false;
            debuffController.ApplyDebuff(StatusEffect.Burning);
        }
        if(debugCureBurning)
        {
            debugCureBurning = false;
            StatusEffect[] se = new StatusEffect[1];
            se[0] = StatusEffect.Burning;
            debuffController.ApplyStatusEffectsCures(se);
        }
        if (debugApplyPoison)
        {
            debugApplyPoison = false;
            debuffController.ApplyDebuff(StatusEffect.Poison);
        }
        if (debugCurePoison)
        {
            debugCurePoison = false;
            StatusEffect[] se = new StatusEffect[1];
            se[0] = StatusEffect.Poison;
            debuffController.ApplyStatusEffectsCures(se);
        }
    }

    private void Update()
    {
        if (debugHP)
        {
            debugHP.text = currentHealth.ToString();
            debugBurningDamage.text = burnedHealth.ToString();
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
        debuffController = GetComponent<DebuffController>();
        buffController = GetComponent<BuffController>();

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

    private void OnHit(HitInfo hitInfo, DamageModifierDefinitions.DamageModifier hitLocationModifier, Rigidbody receiverOfHit)
    {
        if(currentHealth > 0)
        {
            StoreInfoForPotentialRagdoll(hitInfo, receiverOfHit);

            float damageModifierValue = DamageModifierDefinitions.GetDamageModifierValue(hitLocationModifier);
            float finaldamage = GetDamage(hitInfo, damageModifierValue);
            ReduceHealth(finaldamage);

            if (hitInfo.statusEffect != StatusEffect.None && receivesStatusEffects) { debuffController.ApplyDebuff(hitInfo.statusEffect); }

            Debug.Log($"BaseDamage:  {hitInfo.baseDamage}");
            Debug.Log($"Bodypart damage modifier:  {damageModifierValue}");
            Debug.Log($"Final damage:  {GetDamage(hitInfo, damageModifierValue)}");

            if (debugHP) debugHP.text = $"HP: {currentHealth}";
        }
    }

    private float GetDamage(HitInfo hitInfo, float hitLocationModifier)
    {
        float damage = 0f;
        if(hitInfo.weaponCalliber == WeaponCalliber.None)
        {
            damage = currentDamageAbsorb * hitInfo.baseDamage;
        }
        else if(hitInfo.weaponCalliber == WeaponCalliber.Melee)
        {
            damage = currentDamageAbsorb * hitInfo.baseDamage * hitInfo.damageModifier * hitLocationModifier;
        }
        else
        {
            float distance = Vector3.Distance(transform.position, hitInfo.locationOfDamageSource);
            float damageFalloffMultiplier = hitInfo.damageFalloffCurve.Evaluate(distance);

            Debug.Log(damageFalloffMultiplier);

            damage = currentDamageAbsorb * hitInfo.baseDamage * hitInfo.damageModifier * hitLocationModifier * damageFalloffMultiplier;
        }
        
        return damage;
    }

    public void NotifyBuff(MedicalItem item)
    {
        buffController.ApplyBuff(item);
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

    public void NotifyDamageAbsorbChange(float newValue)
    {
        currentDamageAbsorb = newValue;
    }

    public void NotifyDebuffCure(StatusEffect[] statusEffects)
    {
        debuffController.ApplyStatusEffectsCures(statusEffects);
    }

    private void OnHealthDepleted()
    {
        OnHealthDepletedEvent?.Invoke();

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
                ragdollController.NotifyApplyDynamicRagdoll(lastReceiverOfDamage, GetForceToApply());
                break;
        }
    }

    HitInfo lastHitInfo = new();
    Rigidbody lastReceiverOfDamage;
    private void StoreInfoForPotentialRagdoll(HitInfo hitInfo, Rigidbody receiverOfHit)
    {
        lastHitInfo = hitInfo;
        lastReceiverOfDamage = receiverOfHit;
    }

    private Vector3 GetForceToApply()
    {
        Vector3 direction = -(lastHitInfo.locationOfDamageSource - lastReceiverOfDamage.transform.position).normalized;
        float forceModifier = 1f;
        if(lastHitInfo.damageFalloffCurve != null)
        {
            float distance = Vector3.Distance(lastHitInfo.locationOfDamageSource, transform.position);
            forceModifier = lastHitInfo.damageFalloffCurve.Evaluate(distance);
        }
        float forceMagnitude = lastHitInfo.baseDamage * forceModifier * 1.5f;
        
        return direction * forceMagnitude;
    }
}
