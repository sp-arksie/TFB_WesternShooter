using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class MedicalItem : ItemBase
{
    [Header("General")]
    [SerializeField] protected float timeToUseItem = 4f;
    [SerializeField] protected bool isQuickAction = false;


    [SerializeField] bool doesRestoreHealth = false;
    [SerializeField] float healthRestoreAmount = 50f;
    public bool DoesRestoreHealth { get => doesRestoreHealth; private set => doesRestoreHealth = value; }
    public float HealthRestoreAmount { get => healthRestoreAmount; private set => healthRestoreAmount = value; }


    [SerializeField] bool doesRestoreBurnedHealth = false;
    [SerializeField] float burnedHealthRestoreAmount = 0f;
    public bool DoesRestoreBurnedHealth { get => doesRestoreBurnedHealth; private set => doesRestoreBurnedHealth = value; }
    public float BurnedHealthRestoreAmount { get => burnedHealthRestoreAmount; private set => burnedHealthRestoreAmount = value; }


    [SerializeField] bool doesDamageAbsorb = false;
    [SerializeField] [Range(0f, 1f)] float damageAbsorbPercent = 0.6f;
    [SerializeField] float damageAbsorbDuration = 15f;
    public bool DoesDamageAbsorb { get => doesDamageAbsorb; private set => doesDamageAbsorb = value; }
    public float DamageAbsorbPercent { get => damageAbsorbPercent; private set => damageAbsorbPercent = value; }
    public float DamageAbsorbDuration {  get => damageAbsorbDuration; private set => damageAbsorbDuration = value; }


    [SerializeField] bool doesHealthRegeneration = false;
    [SerializeField] float regenerationPerSecond = 5f;
    [SerializeField] float regenerationDuration = 15f;
    public bool DoesHealthRegeneration { get => doesHealthRegeneration; private set => doesHealthRegeneration = value; }
    public float RegenerationPerSecond { get => regenerationPerSecond; private set => regenerationPerSecond = value; }
    public float RegenerationDuration { get => regenerationDuration; private set => regenerationDuration = value;}


    [SerializeField] bool doesCureDebuffs = false;
    [SerializeField] StatusEffect[] debuffCures;
    public bool DoesCureDebuffs { get => doesCureDebuffs; private set => doesCureDebuffs = value; }
    public StatusEffect[] DebuffCures { get => debuffCures; private set => debuffCures = value; }

    protected HealthController healthController;

    Coroutine performAction;

    internal override void NotifyQuickAction()
    {
        if(isQuickAction)
        {
            if (performAction != null) { StopCoroutine(performAction); }
            performAction = StartCoroutine(PerformAction());
        }
    }

    internal override void NotifyChargeStart()
    {
        if (!isQuickAction)
        {
            if (performAction != null) { StopCoroutine(performAction); }
            performAction = StartCoroutine(PerformAction());
        }
    }

    internal override void NotifyChargeRelease()
    {
        StopCoroutine(performAction);
    }

    internal override void NotifySelected(HotBarManager hotBarManager)
    {
        healthController = hotBarManager.GetHealthController();
    }

    internal override void NotifyUnselected()
    {
        // NOOP
    }

    protected virtual IEnumerator PerformAction()
    {
        yield return new WaitForSeconds(timeToUseItem);

        healthController.NotifyBuff(this);

        yield return null;
    }
}
