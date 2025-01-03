using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SeekState : BaseState
{
    private void OnEnable()
    {
        
    }

    private void Update()
    {
        if(GetSight().GetCurrentlyVisiblesToEntity().Count >= 1)
        {
            GetAgent().SetDestination(GetSight().GetCurrentlyVisiblesToEntity()[0].entityVisible.transform.position);
        }
    }

    private void OnDisable()
    {
        
    }
}
