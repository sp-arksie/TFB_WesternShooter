using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RagdollEnabler : MonoBehaviour
{
    [SerializeField] Transform ragdollRoot;
    [SerializeField] bool startInRagdoll = false;

    Animator animator;
    Rigidbody[] rigidBodies;
    Joint[] joints;

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
}
