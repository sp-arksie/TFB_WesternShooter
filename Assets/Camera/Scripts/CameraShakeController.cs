using Cinemachine;
using System.Collections;
using UnityEngine;
using System;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] float lerpTime = 0.2f;

    [NoSaveDuringPlay] CinemachinePOV cinemachinePOV;
    HotBarManager hotBarManager;
    Coroutine cameraShakeCoroutine;


    private void Awake()
    {
        cinemachinePOV = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        hotBarManager = GetComponent<HotBarManager>();
    }

    private void OnEnable()
    {
        hotBarManager.ItemSelectedEvent += RegisterItem;
        hotBarManager.ItemUnselectedEvent += UnregisterItem;
    }

    private void OnDisable()
    {
        hotBarManager.ItemSelectedEvent -= RegisterItem;
        hotBarManager.ItemUnselectedEvent -= UnregisterItem;
    }

    private void RegisterItem(ItemBase item)
    {
        if(item is ICameraShaker) { item.onShakeCamera += ShakeCamera; }
    }

    private void UnregisterItem(ItemBase item)
    {
        if(item is ICameraShaker) { item.onShakeCamera -= ShakeCamera; }
    }

    public void ShakeCamera(ICameraShaker cameraShakeInfo)
    {
        if(cameraShakeCoroutine != null) { StopCoroutine(cameraShakeCoroutine); }
        cameraShakeCoroutine = StartCoroutine(CameraShakeCoroutine(cameraShakeInfo.ReturnCameraShakeInfo()));
    }


    private IEnumerator CameraShakeCoroutine(CameraShakeInfo cameraShakeInfo)
    {
        float dt = 0f;
        float t = 0f;

        Vector2 shake = new Vector2(-cameraShakeInfo.direction.normalized.y, cameraShakeInfo.direction.normalized.x) * cameraShakeInfo.strength;
        Vector2 start = new Vector2(cinemachinePOV.m_VerticalAxis.Value, cinemachinePOV.m_HorizontalAxis.Value);
        Vector2 end = new Vector2(start.x + shake.x, start.y + shake.y);

        while (t <= 1)
        {
            dt += Time.deltaTime;
            t = dt / lerpTime;
            float sinerp = Mathf.Sin(t * Mathf.PI * 0.5f);
            cinemachinePOV.m_VerticalAxis.Value = Mathf.Lerp(start.x, end.x, sinerp);
            cinemachinePOV.m_HorizontalAxis.Value = Mathf.Lerp(start.y, end.y, sinerp);

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(cameraShakeInfo.recoveryStartDelay);

        dt = 0f;
        t = 0f;

        Vector2 difference = new Vector2(shake.x, shake.y) * cameraShakeInfo.recoveryPercent;
        Vector2 previous = Vector2.zero;

        while (t <= 1)
        {
            dt += Time.deltaTime;
            t = dt / cameraShakeInfo.recoveryTime;

            float x = Mathf.SmoothStep(0f, -difference.x, t);
            float y = Mathf.SmoothStep(0f, -difference.y, t);
            cinemachinePOV.m_VerticalAxis.Value += (x - previous.x);
            cinemachinePOV.m_HorizontalAxis.Value += (y - previous.y);
            previous.x = x;
            previous.y = y;

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}
