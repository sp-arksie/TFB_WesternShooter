using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : BaseState
{
    [SerializeField] MeleeWeapon meleeWeapon;
    bool attackInProgress = false;

    private void OnEnable()
    {
        meleeWeapon.onUnskippableActionInProgress += SetUnskippableAction;
        GetAgent().SetDestination(transform.position);
    }

    private void Update()
    {
        if(!attackInProgress)
        {
            meleeWeapon.NotifyQuickAction();
        }
    }

    private void OnDisable()
    {
        meleeWeapon.onUnskippableActionInProgress -= SetUnskippableAction;
        attackInProgress = false;
    }

    private void SetUnskippableAction(bool inProgress)
    {
        attackInProgress = inProgress;
    }
}
