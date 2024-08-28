using Cinemachine;
using System.Collections;
using UnityEngine;
using System;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] float lerpTime = 0.2f;
    [SerializeField] float attenuateRecovery = 1f;

    [NoSaveDuringPlay] CinemachinePOV cinemachinePOV;
    HotBarManager hotBarManager;
    Coroutine recoilCoroutine;

    RecoilBehaviour recoilBehaviour;


    private void Awake()
    {
        cinemachinePOV = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        hotBarManager = GetComponent<HotBarManager>();
    }

    private void OnEnable()
    {
        hotBarManager.WeaponSelectedEvent += RegisterWeapon;
        hotBarManager.WeaponUnselectedEvent += UnregisterWeapon;
    }

    private void OnDisable()
    {
        hotBarManager.WeaponSelectedEvent -= RegisterWeapon;
        hotBarManager.WeaponUnselectedEvent -= UnregisterWeapon;
    }

    private void RegisterWeapon(ItemBase weapon)
    {
        if(weapon.TryGetComponent<RecoilBehaviour>(out recoilBehaviour))
        {
            recoilBehaviour.OnRecoil += StartRecoil;
        }
    }

    private void UnregisterWeapon(ItemBase weapon)
    {
        if(recoilBehaviour) recoilBehaviour.OnRecoil -= StartRecoil;
    }

    public void StartRecoil(RecoilBehaviour recoilStats)
    {
        if(recoilCoroutine != null) { StopCoroutine(recoilCoroutine); }
        recoilCoroutine = StartCoroutine(RecoilCoroutine(recoilStats));
    }

    private IEnumerator RecoilCoroutine(RecoilBehaviour recoilStats)
    {
        float dt = 0f;
        float t = 0f;

        Vector2 recoil = new Vector2(-recoilStats.RecoilDirection.normalized.y, recoilStats.RecoilDirection.normalized.x) * recoilStats.RecoilStrength;
        Vector2 start = new Vector2(cinemachinePOV.m_VerticalAxis.Value, cinemachinePOV.m_HorizontalAxis.Value);
        Vector2 end = new Vector2(start.x + recoil.x, start.y + recoil.y);

        while (t <= 1)
        {
            dt += Time.deltaTime;
            t = dt / lerpTime;
            float sinerp = Mathf.Sin(t * Mathf.PI * 0.5f);
            cinemachinePOV.m_VerticalAxis.Value = Mathf.Lerp(start.x, end.x, sinerp);
            cinemachinePOV.m_HorizontalAxis.Value = Mathf.Lerp(start.y, end.y, sinerp);
            
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(recoilStats.RecoveryStartDelay);

        dt = 0f;
        t = 0f;

        Vector2 difference = new Vector2(recoil.x, recoil.y) * attenuateRecovery;
        Vector2 previous = Vector2.zero;

        while (t <= 1)
        {
            dt += Time.deltaTime;
            t = dt / recoilStats.RecoilRecoveryTime;

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
