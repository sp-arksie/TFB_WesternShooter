using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [Header("Item Grabbing")]
    [SerializeField] public Transform GrabPointsParent;
    [SerializeField] public bool RightArmOnly = false;

    [Header("Animation Transforms")]
    [SerializeField] Transform hipPositionTransform;
    [SerializeField] Transform hiddenTransform;

    public Transform HipPositionTransform { get { return hipPositionTransform; } private set { hipPositionTransform = value; } }
    public Transform HiddenTransform { get { return hiddenTransform; } private set { hiddenTransform = value; } }

    internal abstract void NotifyAttack();

    internal abstract void NotifySelected();

    internal abstract void NotifyUnselected();
}
