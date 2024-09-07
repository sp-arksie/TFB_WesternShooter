using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrenalineShot : MedicalItem
{
    protected override IEnumerator PerformAction()
    {
        yield return new WaitForSeconds(timeToUseItem);

        //healthController.NotifyDamageAbsorbChange(damageAbsorbPercent, damageAbsorbDuration);
        //healthController.NotifyRegenerateHealth(regenerationPerSecond, regenerationDuration);

        yield return null;
    }
}
