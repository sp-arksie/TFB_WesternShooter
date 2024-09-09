using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextMenu : MonoBehaviour
{
    [SerializeField] RectTransform canvasRectTransform;
    [SerializeField] RectTransform backgroundRectTransform;
    [SerializeField] Transform optionsParent;

    [SerializeField] Vector2 paddingSize = new Vector2(8, 8);

    RectTransform contextMenuRect;
    InputManager input;

    private void Awake()
    {
        contextMenuRect = GetComponent<RectTransform>();
        input = InputManager.Instance;
    }


    public void NotifyEquip(GameObject itemToEquip)
    {

    }
}
