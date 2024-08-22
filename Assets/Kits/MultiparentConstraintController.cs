using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MultiparentConstraintController : MonoBehaviour
{
    MultiParentConstraint constraint;

    private void Awake()
    {
        constraint = GetComponent<MultiParentConstraint>();
    }

    public void NotifyApplyConstraint(Transform target)
    {
        ApplyConstraint(target);
    }

    private void ApplyConstraint(Transform target)
    {
        constraint.data.sourceObjects.Clear();
        constraint.data.sourceObjects.Add(new WeightedTransform(target, 1));
    }

    public void NotifyUnapplyConstraint()
    {
        UnapplyConstraint();
    }

    private void UnapplyConstraint()
    {
        constraint.data.sourceObjects.Clear();
    }
}
