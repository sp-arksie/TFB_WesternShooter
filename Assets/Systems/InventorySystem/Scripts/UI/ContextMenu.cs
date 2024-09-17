using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    [SerializeField] RectTransform canvasRectTransform;
    [SerializeField] RectTransform backgroundRectTransform;
    [SerializeField] Transform optionsParent;
    TextMeshProUGUI[] texts;

    [SerializeField] Vector2 paddingSize = new Vector2(16, 16);

    public static ContextMenu Instance { get; private set; }

    RectTransform contextMenuRect;
    InputManager input;

    public static Action OnEquipSelected;
    public static Action OnUnequipSelected;
    public static Action OnContextMenuClose;

    [SerializeField] GraphicRaycaster graphicRaycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }

        contextMenuRect = GetComponent<RectTransform>();
        input = InputManager.Instance;
        texts = GetComponentsInChildren<TextMeshProUGUI>();

        eventSystem = GetComponent<EventSystem>();

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(TrackMouseOver());
    }

    private void OnDisable()
    {
        OnContextMenuClose?.Invoke();
    }

    IEnumerator TrackMouseOver()
    {
        yield return new WaitForEndOfFrame();

        while (true)
        {
            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = input.GetMousePosition();
            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            bool exitedBounds = true;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponent<Image>() != null)
                {
                    Image i = result.gameObject.GetComponent<Image>();
                    if (i.sprite == null)
                        exitedBounds = false;
                }
            }
            if (exitedBounds)
                gameObject.SetActive(false);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public static void NotifyOpenContextMenu(string name)
    {
        Instance.OpenContextMenu(name);
    }

    private void OpenContextMenu(string name)
    {
        gameObject.SetActive(true);

        texts[0].SetText($"Equip {name}");
        texts[1].SetText($"Unequip {name}");

        backgroundRectTransform.sizeDelta = GetBackgroundSizeToSet() + paddingSize;

        Vector2 anchoredPosition = input.GetMousePosition() / canvasRectTransform.localScale.x;
        contextMenuRect.anchoredPosition = anchoredPosition;
    }

    private Vector2 GetBackgroundSizeToSet()
    {
        float widestText = 0f;
        float height = 0f;

        foreach(TextMeshProUGUI t in texts)
        {
            t.ForceMeshUpdate();
            Vector2 v = t.GetRenderedValues(false);
            widestText = v.x > widestText ? v.x : widestText;
            height = v.y;
        }

        return new Vector2( widestText, 2 * height );
    }

    public void EquipOption()
    {
        OnEquipSelected?.Invoke();
        gameObject.SetActive(false);
    }

    public void UnequipOption()
    {
        OnUnequipSelected?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}
