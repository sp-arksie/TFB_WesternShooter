using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeState : BaseState
{
    [SerializeField] float minStrafeDuration = 2f;
    [SerializeField] float maxStrafeDuration = 4f;

    [Space(10)]
    [SerializeField] float minCooldownBetweenStrafe = 2.5f;
    [SerializeField] float maxCooldownBetweenStrafe = 4f;

    float currentStrafeDuration = 0f;
    float timeOfNewStrafeSet = 0f;
    bool right = true;
    float cooldownStartTime = 0f;
    float cooldownDuration = 0f;
    bool inCooldown = false;

    private void OnEnable()
    {
        StopRun();
    }

    private void Update()
    {
        NotifyOrientEntityToTarget();

        if (!inCooldown)
        {
            SetStrafe(right);
        }
        else
        {
            CheckCooldown();
        }

        NotifyOrientEntityToTarget();
    }

    private void SetStrafe(bool right)
    {
        int sign = right ? 1 : -1;

        Vector3 direction = transform.TransformPoint(sign * Vector3.right);
        GetAgent().SetDestination(direction);

        if (Time.time - timeOfNewStrafeSet > currentStrafeDuration)
            SetCooldown();
    }

    private void SetCooldown()
    {
        inCooldown = true;
        cooldownStartTime = Time.time;
        cooldownDuration = Random.Range(minCooldownBetweenStrafe, maxCooldownBetweenStrafe);
    }

    private void CheckCooldown()
    {
        if(Time.time - cooldownStartTime > cooldownDuration)
        {
            inCooldown = false;

            right = Random.Range(0, 2) == 1 ? true : false;

            timeOfNewStrafeSet = Time.time;
            currentStrafeDuration = Random.Range(minStrafeDuration, maxStrafeDuration);
        }
    }
}
