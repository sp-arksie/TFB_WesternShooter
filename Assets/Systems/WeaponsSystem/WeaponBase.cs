using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] public Transform GrabPointsParent;
    [SerializeField] public bool RightArmOnly = false;

    internal abstract void NotifyAttack();

    internal abstract void NotifySelected();

    internal abstract void NotifyUnselected();
}
