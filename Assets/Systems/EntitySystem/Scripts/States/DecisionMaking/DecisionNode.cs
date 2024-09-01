using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecisionNode : MonoBehaviour
{
    protected EntityController entityController;

    internal void Init(EntityController entityController)
    {
        this.entityController = entityController;
    }

    internal StateNode Evaluate()
    {
        StateNode stateNode = null;

        if (CheckCondition())
        {
            stateNode = ProcessChild(0);
        }
        else
        {
            stateNode = ProcessChild(1);
        }

        return stateNode;
    }

    private StateNode ProcessChild(int childIndex)
    {
        Transform child = transform.GetChild(childIndex);
        StateNode stateNode = child.GetComponent<StateNode>();

        if (!stateNode)
            stateNode = child.GetComponent<DecisionNode>().Evaluate();
        
        return stateNode;
    }

    protected abstract bool CheckCondition();

}
