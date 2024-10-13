using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Blades : MonoBehaviour
{
    [SerializeField] float lerpSpeed = 5f;

    private void Start()
    {
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            float dt = Time.deltaTime;
            float t = dt / lerpSpeed;
            Quaternion start = transform.rotation;
            Quaternion end = transform.rotation * Quaternion.AngleAxis(120, Vector3.right);

            Quaternion newRotation = Quaternion.Lerp(start, end, t);
            transform.rotation = newRotation;
            yield return null;
        }
    }
}
