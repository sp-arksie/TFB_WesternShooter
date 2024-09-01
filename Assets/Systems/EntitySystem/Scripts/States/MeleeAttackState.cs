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
        GetAgent().isStopped = true;
    }

    private void Update()
    {
        if(!attackInProgress)
        {
            meleeWeapon.NotifyClick();
        }
    }

    private void OnDisable()
    {
        meleeWeapon.onUnskippableActionInProgress -= SetUnskippableAction;
    }

    private void SetUnskippableAction(bool inProgress)
    {
        attackInProgress = inProgress;
    }
}
