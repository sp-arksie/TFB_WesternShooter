using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DecisionHasPotentialCoverPosition : DecisionNode
{
    Collider[] potentialCoverColliders;
    const float operationFailed = -10f;

    private void Start()
    {
        potentialCoverColliders = new Collider[entityController.NumberOfCollidersToSample];
    }

    protected override bool CheckCondition()
    {
        for (int i = 0; i < potentialCoverColliders.Length; i++)
        {
            potentialCoverColliders[i] = null;
        }

        int numberOfPotentialColliders = Physics.OverlapSphereNonAlloc(
            transform.position, entityController.CoverCheckingAreaRadius, potentialCoverColliders, entityController.StaticOccludersLayerMask);

        Array.Sort(potentialCoverColliders, SortByClosestColliders);

        Vector3 targetPosition = entityController.sight.GetCurrentlyVisiblesToEntity()[0].entityVisible.transform.position;

        bool suitableCoverFound = false;
        for (int i = 0; i < numberOfPotentialColliders && !suitableCoverFound; i++)
        {
            float dotProduct = GetPotentialCoverPosition(potentialCoverColliders[i].transform.position, targetPosition, out Vector3 potentialCoverPosition);
            if (dotProduct != operationFailed && dotProduct <= entityController.HidingAccuracy)
            {
                if( !entityController.IsMovingToCover )
                    entityController.NotifySetCoverDestination(potentialCoverPosition);
                suitableCoverFound = true;
            }
            else if (dotProduct != operationFailed)
            {
                Vector3 newSamplePosition = potentialCoverColliders[i].transform.position - (targetPosition - potentialCoverPosition) * 2f;

                dotProduct = GetPotentialCoverPosition(newSamplePosition, targetPosition, out potentialCoverPosition);
                if (dotProduct != operationFailed && dotProduct <= entityController.HidingAccuracy)
                {
                    if (!entityController.IsMovingToCover)
                        entityController.NotifySetCoverDestination(potentialCoverPosition);
                    suitableCoverFound = true;
                }
            }
        }

        return suitableCoverFound;
    }

    // Gets the closest Edge position from samplePosition (make sure NavMesh is baked, to detect obstacle edges)
    // Returns the dot product between the normal of the Edge that is returned and the Target.
    private float GetPotentialCoverPosition(Vector3 samplePosition, Vector3 targetPosition, out Vector3 coverPosition)
    {
        float dotProduct = operationFailed;
        coverPosition = Vector3.zero;

        if (NavMesh.SamplePosition(samplePosition, out NavMeshHit hit, entityController.agent.height * 2, entityController.agent.areaMask))
        {
            if (NavMesh.FindClosestEdge(hit.position, out hit, entityController.agent.areaMask))
            {
                dotProduct = Vector3.Dot(hit.normal, (targetPosition - hit.position).normalized);
                coverPosition = hit.position;
            }
        }

        return dotProduct;
    }

    private int SortByClosestColliders(Collider a, Collider b)
    {
        int value = 0;

        if(a != null && b == null)
        {
            value = -1;
        }
        else if(a == null && b == null)
        {
            value = 0;
        }
        else if(a == null && b != null)
        {
            value = 1;
        }
        else
        {
            float distanceA = Mathf.Abs( Vector3.Distance(a.transform.position, entityController.transform.position) );
            float distanceB = Mathf.Abs( Vector3.Distance(b.transform.position, entityController.transform.position) );
            value = distanceA.CompareTo(distanceB);
        }

        return value;
    }
}
