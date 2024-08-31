using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [Header("Item Grabbing")]
    [SerializeField] public Transform GrabPointsParent;
    [SerializeField] public bool RightArmOnly = false;

    [Header("Default position + Switching Transform")]
    [SerializeField] protected Transform hipPositionTransform;
    [SerializeField] protected Transform hiddenTransform;


    public Transform HipPositionTransform { get { return hipPositionTransform; } private set { hipPositionTransform = value; } }
    public Transform HiddenTransform { get { return hiddenTransform; } private set { hiddenTransform = value; } }

    public event Action<bool> onUnskippableActionInProgress;
    public event Action<ICameraShaker> onShakeCamera;

    internal abstract void NotifyClick();

    internal abstract void NotifyPressStart();

    internal abstract void NotifyPressFinish();

    internal abstract void NotifySelected();

    internal abstract void NotifyUnselected();

    protected virtual void OnUnskippableActionInProgress(bool inProgress)
    {
        onUnskippableActionInProgress?.Invoke(inProgress);
    }

    protected virtual void OnShakeCamera(ICameraShaker cameraShakeInfo)
    {
        onShakeCamera?.Invoke(cameraShakeInfo);
    }
}
