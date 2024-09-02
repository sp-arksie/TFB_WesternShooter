using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class EntitySight : MonoBehaviour
{
    [SerializeField] float sightRadius = 5f;
    [SerializeField] float sightDepth = 10f;
    [SerializeField] float timeVisiblesStayInMemory = 1.5f;
    [SerializeField] LayerMask visibleObjectsLayerMask = Physics.DefaultRaycastLayers;
    [SerializeField] LayerMask objectOccluderLayerMask = Physics.DefaultRaycastLayers;
    [SerializeField] LayerMask visibilityPointsLayerMask = Physics.DefaultRaycastLayers;
    [SerializeField] string[] acceptedTags = { "Untagged" };

    [System.Serializable]
    public class VisibleInMemory
    {
        public EntityVisible entityVisible;
        public float lastSeenTime;
    }

    [SerializeField] List<VisibleInMemory> currentVisiblesInMemory = new();


    private void Update()
    {
        Collider[] potentialVisibleObjects = Physics.OverlapCapsule(
            transform.position + (transform.forward * sightRadius),
            transform.position + (transform.forward * sightRadius) + (transform.forward * sightDepth),
            sightRadius,
            visibleObjectsLayerMask
            );

        foreach(Collider c in potentialVisibleObjects)
        {
            if (acceptedTags.Contains(c.tag))
            {
                EntityVisible entityVisible = c.GetComponent<EntityVisible>();

                if(entityVisible != null)
                {
                    bool lineOfSightConfirmed = false;
                    if (entityVisible.VisibilityPointsParent)
                    {
                        for (int i = 0; i < entityVisible.VisibilityPointsParent.childCount && !lineOfSightConfirmed; i++)
                        {
                            Transform visibilityPoint = entityVisible.VisibilityPointsParent.GetChild(i);
                            lineOfSightConfirmed = ConfirmLineOfSight(visibilityPoint);
                        }
                    }
                    else
                    {
                        lineOfSightConfirmed = ConfirmLineOfSight(entityVisible.transform);
                    }

                    if (lineOfSightConfirmed) ManageVisibles(entityVisible);
                }
            }
        }

        currentVisiblesInMemory.RemoveAll(x => (x.entityVisible == null) || (Time.time - x.lastSeenTime > timeVisiblesStayInMemory));
        currentVisiblesInMemory.Sort(
            (a, b) =>
                Vector3.Distance(transform.position, a.entityVisible.transform.position) >
                Vector3.Distance(transform.position, b.entityVisible.transform.position) ?
                1 : 0);
    }

    private bool ConfirmLineOfSight(Transform visibilityPoint)
    {
        Vector3 direction = visibilityPoint.position - transform.position;
        bool lineOfSightConfirmed = false;
        
        if(Physics.Raycast(transform.position, direction, out RaycastHit hit, direction.magnitude, objectOccluderLayerMask | visibilityPointsLayerMask))
        {
            lineOfSightConfirmed = ((1 << hit.collider.gameObject.layer) & visibilityPointsLayerMask) != 0;
        }
        return lineOfSightConfirmed;
    }

    private void ManageVisibles(EntityVisible entityVisible)
    {
        if (entityVisible.gameObject != this.gameObject)
        {
            VisibleInMemory visibleInMemory = currentVisiblesInMemory.Find(x => x.entityVisible == entityVisible);
            if (visibleInMemory == null)
            {
                visibleInMemory = new();
                visibleInMemory.entityVisible = entityVisible;
                currentVisiblesInMemory.Add(visibleInMemory);
            }
            visibleInMemory.lastSeenTime = Time.time;
        }
    }

    public List<VisibleInMemory> GetCurrentlyVisiblesToEntity()
    {
        currentVisiblesInMemory.RemoveAll(x => x.entityVisible == null);
        return currentVisiblesInMemory;
    }
}
