using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] Transform ragdollRoot;
    [SerializeField] bool startInRagdoll = false;

    Animator animator;
    Rigidbody[] rigidBodies;
    Joint[] joints;

    public class DynamicRagdollInfo
    {
        public Rigidbody rigidbodyToAffect;
        public Vector3 damageSourceLocation;
        public float forceToApply;
    }

    private void Awake()
    {
        rigidBodies = ragdollRoot.GetComponentsInChildren<Rigidbody>();
        joints = ragdollRoot.GetComponentsInChildren<Joint>();
        animator = GetComponentInChildren<Animator>();

        if (startInRagdoll)
            EnableRagdoll();
        else
            EnableAnimator();
    }

    internal void EnableAnimator()
    {
        animator.enabled = true;

        foreach (Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        foreach (Joint joint in joints)
        {
            joint.enableCollision = false;
        }
    }

    internal void EnableRagdoll()
    {
        animator.enabled = false;

        foreach (Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        foreach (Joint joint in joints)
        {
            joint.enableCollision = true;
        }
    }

    public void NotifyApplyDynamicRagdoll(Rigidbody rigidBodyToAffect, Vector3 force)
    {
        rigidBodyToAffect.AddForceAtPosition(force, rigidBodyToAffect.transform.position, ForceMode.Impulse);
    }
}
