using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class HotBarManager : MonoBehaviour
{
    [SerializeField] protected Transform hotBarItems;

    [Header("Grab Points")]
    [SerializeField] ParentConstraint leftArmTarget;
    [SerializeField] ParentConstraint leftArmHint;
    [SerializeField] ParentConstraint RightArmTarget;
    [SerializeField] ParentConstraint RightArmHint;
    [SerializeField] TwoBoneIKConstraint leftArmRig;
    [SerializeField] TwoBoneIKConstraint rightArmRig;

    [Header("Item switching")]
    [SerializeField] float timeToSwitch = 0.6f;
    protected Coroutine selectItem;
    protected Coroutine animateItemSwitch;
    protected bool currentItemBusy = false;

    // Items index
    protected int startingIndex = 0;
    protected int currentIndex = 0;

    // References
    Animator animator;
    HealthController healthController;

    // Animator hashes
    int hasGunHash;
    int rightArmOnlyHash;

    public event Action<ItemBase> ItemSelectedEvent;
    public event Action<ItemBase> ItemUnselectedEvent;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        healthController = GetComponent<HealthController>();
    }

    protected virtual void Start()
    {
        hasGunHash = Animator.StringToHash("HasGun");
        rightArmOnlyHash = Animator.StringToHash("RightHandOnly");

        for(int i = 0; i < hotBarItems.childCount; i++)
        {
            hotBarItems.GetChild(i).gameObject.SetActive(false);
        }
        currentIndex = startingIndex;
        if(currentIndex > -1)
            NotifySelectItem(currentIndex);
    }

    private void PrepareStartingItem()
    {
        hotBarItems.GetChild(currentIndex).gameObject.SetActive(true);

        ItemBase item = hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>();

        animator.SetBool(hasGunHash, true); // TODO: dont hardcode this
        animator.SetBool(rightArmOnlyHash, item.RightArmOnly);
        ItemSelectedEvent?.Invoke(item);
        item.onUnskippableActionInProgress += SetItemBusy;
        GrabPoints(item);
    }

    float desiredRightArmWeight = 0;
    float desiredLeftArmWeight = 0;
    private void Update()
    {
        if(rightArmRig.weight != desiredRightArmWeight) { rightArmRig.weight = desiredRightArmWeight; }
        if(leftArmRig.weight != desiredLeftArmWeight) { leftArmRig.weight = desiredLeftArmWeight; }
    }

    protected void QuickAction()
    {
        hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>().NotifyQuickAction();
    }

    protected void ChargeStart()
    {
        hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>().NotifyChargeStart();
    }

    protected void ChargeRelease()
    {
        hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>().NotifyChargeRelease();
    }

    #region Item Switching
    protected void NotifySelectItem(int index)
    {
        if (!currentItemBusy && hotBarItems.childCount > 0)
        {
            if (selectItem != null) { StopCoroutine(selectItem); }
            selectItem = StartCoroutine(SelectItem(index));
        }
    }

    protected IEnumerator SelectItem(int index)
    {
        if (currentIndex != -1)
        {
            ItemBase item = hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>();
            UnsubscribeItem(item);

            Transform current = item.transform;
            Transform goal = item.HiddenTransform;
            if (animateItemSwitch != null) StopCoroutine(animateItemSwitch);
            yield return animateItemSwitch = StartCoroutine(AnimateItemSwitch(item, current, goal));

            UngrabPoints(item);
            hotBarItems.GetChild(currentIndex).gameObject.SetActive(false);
        }

        if (index < -1)
            index = hotBarItems.childCount - 1;
        if (index > hotBarItems.childCount - 1)
            index = -1;
        currentIndex = index;

        if (currentIndex != -1)
        {
            ItemBase item = hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>();
            SubscribeItem(item);

            animator.SetBool(hasGunHash, true);
            animator.SetBool(rightArmOnlyHash, item.RightArmOnly);

            item.gameObject.SetActive(true);
            GrabPoints(item);

            Transform current = item.transform;
            Transform goal = item.HipPositionTransform;
            if (animateItemSwitch != null) StopCoroutine(animateItemSwitch);
            yield return StartCoroutine(AnimateItemSwitch(item, current, goal));
        }
        else
        {
            animator.SetBool(hasGunHash, false);
        }
    }

    private void UnsubscribeItem(ItemBase item)
    {
        item.NotifyUnselected();
        item.onUnskippableActionInProgress -= SetItemBusy;
        item.activateLeftArm -= ActivateLeftArmConstraints;
        item.deactivateLeftArm -= DeactivateLeftArmConstraints;
        ItemUnselectedEvent?.Invoke(item);
    }

    private void SubscribeItem(ItemBase item)
    {
        item.NotifySelected(this);
        item.onUnskippableActionInProgress += SetItemBusy;
        item.activateLeftArm += ActivateLeftArmConstraints;
        item.deactivateLeftArm += DeactivateLeftArmConstraints;
        ItemSelectedEvent?.Invoke(item);
    }

    private void SetItemBusy(bool itemBusy)
    {
        this.currentItemBusy = itemBusy;
    }

    private IEnumerator AnimateItemSwitch(ItemBase item, Transform current, Transform goal)
    {
        float dt = 0f;

        while(dt < timeToSwitch * 0.5)
        {
            dt += Time.deltaTime;
            item.transform.localPosition = Vector3.Lerp(current.localPosition, goal.localPosition, dt);
            item.transform.localRotation = Quaternion.Lerp(current.localRotation, goal.localRotation, dt);
            yield return null;
        }
        item.transform.localPosition = goal.localPosition;
        item.transform.localRotation = goal.localRotation;
    }
    
    private void UngrabPoints(ItemBase item)
    {
        Transform grabPointsParent = item.GrabPointsParent;
        if (item.RightArmOnly)
        {
            UnapplyConstraint(grabPointsParent.GetChild(2), RightArmTarget);
            UnapplyConstraint(grabPointsParent.GetChild(3), RightArmHint);
        }
        else
        {
            UnapplyConstraint(grabPointsParent.GetChild(0), leftArmTarget);
            UnapplyConstraint(grabPointsParent.GetChild(1), leftArmHint);
            UnapplyConstraint(grabPointsParent.GetChild(2), RightArmTarget);
            UnapplyConstraint(grabPointsParent.GetChild(3), RightArmHint);
        }

        desiredLeftArmWeight = 0;
        desiredRightArmWeight = 0;
    }

    private void UnapplyConstraint(Transform target, ParentConstraint constrainedObject)
    {
        if (constrainedObject)
        {
            constrainedObject.constraintActive = false;
            for (int i = 0; i < constrainedObject.sourceCount; i++)
            {
                constrainedObject.RemoveSource(i);
            }
        }
    }

    private void GrabPoints(ItemBase item)
    {
        Transform grabPointsParent = item.GrabPointsParent;
        if (item.RightArmOnly)
        {
            ApplyConstraint(grabPointsParent.GetChild(2), RightArmTarget);
            ApplyConstraint(grabPointsParent.GetChild(3), RightArmHint);
            desiredRightArmWeight = 1;
        }
        else
        {
            ApplyConstraint(grabPointsParent.GetChild(0), leftArmTarget);
            ApplyConstraint(grabPointsParent.GetChild(1), leftArmHint);
            ApplyConstraint(grabPointsParent.GetChild(2), RightArmTarget);
            ApplyConstraint(grabPointsParent.GetChild(3), RightArmHint);
            desiredLeftArmWeight = 1;
            desiredRightArmWeight = 1;
        }
    }

    private void ApplyConstraint(Transform target, ParentConstraint constrainedObject)
    {
        if (constrainedObject)
        {
            ConstraintSource source = new();
            source.sourceTransform = target;
            source.weight = 1f;
            constrainedObject.AddSource(source);
            constrainedObject.SetTranslationOffset(0, Vector3.zero);
            constrainedObject.SetRotationOffset(0, Vector3.zero);
            constrainedObject.constraintActive = true;

        }
    }

    private void ActivateLeftArmConstraints()
    {
        ItemBase weapon = hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>();
        Transform grabPointsParent = weapon.GrabPointsParent;

        ApplyConstraint(grabPointsParent.GetChild(0), leftArmTarget);
        ApplyConstraint(grabPointsParent.GetChild(1), leftArmHint);
        desiredLeftArmWeight = 1;
    }

    private void DeactivateLeftArmConstraints()
    {
        ItemBase weapon = hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>();
        Transform grabPointsParent = weapon.GrabPointsParent;

        UnapplyConstraint(grabPointsParent.GetChild(0), leftArmTarget);
        UnapplyConstraint(grabPointsParent.GetChild(1), leftArmHint);
        desiredLeftArmWeight = 0;
    }
    #endregion

    #region Reload
    protected void Reload()
    {
        ItemBase item = hotBarItems.GetChild(currentIndex).GetComponent<ItemBase>();
        if (item is ProjectileWeapon)
        {
            ProjectileWeapon projectileWeapon = item as ProjectileWeapon;
            projectileWeapon.NotifyReload();
        }
    }
    #endregion

    public void NotifyRemoveItem(GameObject itemToRemove)
    {
        RemoveItem(itemToRemove);
    }

    private void RemoveItem(GameObject itemToRemoveGO)
    {
        int indexItemToRemove = 0;

        for (int i = 0; i < hotBarItems.childCount; i++)
        {
            if (itemToRemoveGO == hotBarItems.GetChild(i).gameObject)
                indexItemToRemove = i;
        }

        ItemBase itemToRemove = hotBarItems.GetChild(indexItemToRemove).GetComponent<ItemBase>();

        if (currentIndex == indexItemToRemove)
        {
            UnsubscribeItem(itemToRemove);
            UngrabPoints(itemToRemove);
            currentIndex = -1;

            if (selectItem != null) { StopCoroutine(selectItem); }
            selectItem = StartCoroutine(SelectItem(indexItemToRemove - 1));
        }

        itemToRemove.transform.parent = null;

        Debug.Log(currentIndex);
    }

    #region Helper Functions
    public HealthController GetHealthController() { return healthController; }
    #endregion
}
