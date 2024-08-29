using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class EntityAnimation : MonoBehaviour
{
    Animator animator;
    InputManager input;
    IAnimatableEntity animatableEntity;

    int xSpeedHash;
    int zSpeedHash;
    int ySpeedHash;
    int groundedHash;
    int crouchingHash;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        input = InputManager.Instance;
        animatableEntity = GetComponent<IAnimatableEntity>();

        xSpeedHash = Animator.StringToHash("XSpeed");
        zSpeedHash = Animator.StringToHash("ZSpeed");
        ySpeedHash = Animator.StringToHash("YSpeed");
        groundedHash = Animator.StringToHash("Grounded");
        crouchingHash = Animator.StringToHash("Crouching");
    }

    private void Update()
    {
        animator.SetFloat(xSpeedHash, animatableEntity.GetXSpeed());
        animator.SetFloat(zSpeedHash, animatableEntity.GetZSpeed());
        animator.SetFloat(ySpeedHash, animatableEntity.GetYSpeed());
        animator.SetBool(groundedHash, animatableEntity.GetIsGrounded());
        animator.SetBool(crouchingHash, input.GetIsCrouching());
    }
}
