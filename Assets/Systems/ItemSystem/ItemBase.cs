using System;
using UnityEditor.Build.Content;
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

    public string instanceID { get; private set; }
    public int currentAmount { get; protected set; }

    public GameObject prefabReference { get; private set; }

    public void Init(GameObject prefabReference)
    {
        this.prefabReference = prefabReference;
    }

    internal abstract void NotifyQuickAction();

    internal abstract void NotifyChargeStart();

    internal abstract void NotifyChargeRelease();

    internal abstract void NotifySelected(HotBarManager hotBarManager);

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
