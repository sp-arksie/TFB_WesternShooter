using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class EntitySight : MonoBehaviour
{
    [SerializeField] Vector3 sightSize = new Vector3(10, 10, 10);
    [SerializeField] LayerMask visibleObjectsLayer = Physics.DefaultRaycastLayers;
    [SerializeField] string[] acceptedTags = { "Untagged" };

    Transform[] visibleObjects;

    [SerializeField] List<EntityVisible> currentlyVisibleToEntity = new();

    private void Update()
    {
        Collider[] potentialVisibleObjects = Physics.OverlapBox(
            transform.position + (transform.forward * (sightSize.z / 2f)),
            sightSize / 2f,
            transform.rotation,
            visibleObjectsLayer
            );

        currentlyVisibleToEntity.Clear();
        foreach(Collider c in potentialVisibleObjects)
        {
            if (acceptedTags.Contains(c.tag))
            {
                EntityVisible entityVisible = c.GetComponent<EntityVisible>();
                if(entityVisible != null )
                {
                    currentlyVisibleToEntity.Add(entityVisible);
                }
            }
        }

        currentlyVisibleToEntity.Sort(
            (a, b) =>
                Vector3.Distance(transform.position, a.transform.position) >
                Vector3.Distance(transform.position, b.transform.position) ?
                1 : 0);
    }

    public List<EntityVisible> GetCurrentlyVisiblesToEntity()
    {
        currentlyVisibleToEntity.RemoveAll(x => x == null || x.gameObject == this.gameObject);

        return currentlyVisibleToEntity;
    }
}
