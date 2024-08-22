using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] Transform GrabPointsParent;

    internal abstract void NotifyAttack();

    internal abstract void NotifySelected();

    internal abstract void NotifyUnselected();
}
