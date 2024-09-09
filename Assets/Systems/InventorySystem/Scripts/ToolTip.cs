using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    public static ToolTip Instance {  get; private set; }

    [SerializeField] RectTransform canvasRectTransform;
    [SerializeField] RectTransform backgroundRectTransform;
    [SerializeField] TextMeshProUGUI tooltipText;

    [SerializeField] Vector2 paddingSize = new Vector2(16, 8);

    RectTransform tooltipRect;
    InputManager input;

    private void Awake()
    {
        tooltipRect = GetComponent<RectTransform>();
        input = InputManager.Instance;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        //SetText("Hello World");
    }

    private void Update()
    {
        Vector2 anchoredPosition = input.GetMousePosition() / canvasRectTransform.localScale.x;

        if(anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width)
        {
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        }
        if(anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height)
        {
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
        }

        tooltipRect.anchoredPosition = anchoredPosition;
    }

    private void SetText(string text)
    {
        tooltipText.SetText(text);
        tooltipText.ForceMeshUpdate();

        Vector2 totalTextSize = tooltipText.GetRenderedValues(false);
        backgroundRectTransform.sizeDelta = totalTextSize + paddingSize;
    }

    public static void ShowToolTip(string textToSet)
    {
        Instance.ShowToolTipInternal(textToSet);
    }

    public static void HideToolTip()
    {
        Instance.HideToolTipInternal();
    }

    private void ShowToolTipInternal(string textToSet)
    {
        gameObject.SetActive(true);
        SetText(textToSet);
    }

    private void HideToolTipInternal()
    {
        gameObject.SetActive(false);
    }
}
