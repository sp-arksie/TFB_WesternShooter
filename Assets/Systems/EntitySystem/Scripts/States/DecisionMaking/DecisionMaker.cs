using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMaker : MonoBehaviour
{
    [SerializeField] DecisionNode firstDecision;
    EntityController entityController;

    private void Awake()
    {
        entityController = GetComponent<EntityController>();
        DecisionNode[] allDecisions = firstDecision.GetComponentsInChildren<DecisionNode>();
        foreach(DecisionNode dn in allDecisions)
        {
            dn.Init(entityController);
        }
    }

    private void Update()
    {
        StateNode stateNode = firstDecision.Evaluate();
        entityController.SetOrMaintainState(stateNode.state);
    }
}
