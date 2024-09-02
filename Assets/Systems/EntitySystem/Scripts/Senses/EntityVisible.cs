using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityVisible : MonoBehaviour
{
    [SerializeField] Transform visibilityPointsParent;

    public Transform VisibilityPointsParent { get => visibilityPointsParent; private set => visibilityPointsParent = value; }
}
