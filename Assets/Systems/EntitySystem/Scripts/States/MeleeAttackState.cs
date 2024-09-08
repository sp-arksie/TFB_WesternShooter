using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : BaseState
{
    [SerializeField] MeleeWeapon meleeWeapon;

    private void OnEnable()
    {
        GetAgent().SetDestination(transform.position);
    }

    private void Update()
    {
        if(!GetCurrentItemBusy())
        {
            //meleeWeapon.NotifyQuickAction();
            GetHotBarManager().DoQuickAction();
        }
    }

    private void OnDisable()
    {

    }
}
