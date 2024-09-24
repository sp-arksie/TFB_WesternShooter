using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoCoverState : BaseState
{
    private void OnEnable()
    {
        GetAgent().SetDestination( GetCurrentCoverDestination() );
    }
}
