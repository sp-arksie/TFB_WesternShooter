using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CinemachinePOVExtension : CinemachineExtension
{
    [SerializeField] Vector2 rotation;

    [SerializeField] float aimSensitivity = 10f;
    [SerializeField] float clampAngle = 75f;

    InputManager input;

    Vector3 startingRotation;

    protected override void Awake()
    {
        base.Awake();
        startingRotation = transform.localEulerAngles;
        input = InputManager.Instance;
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow && input != null)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                Vector2 inputActionValue = input.GetMouseDelta();
                startingRotation.x += inputActionValue.x * aimSensitivity * Time.deltaTime + rotation.x * Time.deltaTime;
                startingRotation.y += inputActionValue.y * aimSensitivity * Time.deltaTime + rotation.y * Time.deltaTime;
                startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);

                state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);
            }
        }
    }
}
