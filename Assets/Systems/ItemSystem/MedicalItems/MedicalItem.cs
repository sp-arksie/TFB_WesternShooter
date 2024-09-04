using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MedicalItem : ItemBase
{
    [Header("General")]
    [SerializeField] protected float timeToUseItem = 4f;
    [SerializeField] protected bool isQuickAction = false;
    [SerializeField] protected StatusEffect[] statusEffectCures;

    [Header("Health Restore")]
    [SerializeField] protected float healthRestoreAmount = 50f;

    [Header("Damage absorb")]
    [SerializeField] [Range(0f, 1f)] protected float damageAbsorb = 0.6f;
    [SerializeField] protected float damageAbsorbDuration = 15f;

    [Header("Health regeneration")]
    [SerializeField] protected float regenerationPerSecond = 5f;
    [SerializeField] protected float regenerationDuration = 15f;

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

    protected virtual IEnumerator PerformAction() { yield return null; }
}
