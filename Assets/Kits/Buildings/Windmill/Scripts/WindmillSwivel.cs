using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillSwivel : MonoBehaviour
{
    [SerializeField] Vector2 rotationBounds = new Vector2(-70f, 70f);
    [Tooltip("Less is more")]
    [SerializeField][Range(0.01f, 1f)] float rotationVariance = 0.7f;

    [Space(10)]

    [SerializeField] float swivelDuration = 8f;
    [Tooltip("Less is more")]
    [SerializeField][Range(0.01f, 1f)] float swivelDurationVariance = 0.7f;
    bool rotatingRight = true;

    [Space(10)]

    [SerializeField] float pauseBetweenSwivel = 10f;
    [Tooltip("Less is more")]
    [SerializeField][Range(0.01f, 1f)] float pauseDurationVariance = 0.2f;

    private void Start()
    {
        StartCoroutine(Swivel());
        //StartCoroutine(Rotate());
    }

    IEnumerator Swivel()
    {
        while (true)
        {
            float maxTargetRotation = rotatingRight ? rotationBounds.y : rotationBounds.x;
            float rotationAmountDegrees = Random.Range(maxTargetRotation, maxTargetRotation * rotationVariance);
            Quaternion rotationAmount = Quaternion.AngleAxis(rotationAmountDegrees, Vector3.forward);
            Quaternion targetRotation = transform.rotation * rotationAmount;
            Quaternion startRotation = transform.rotation;

            float dt = 0f;
            float t = 0f;
            float rotationDuration = Random.Range(swivelDuration, swivelDuration * swivelDurationVariance);

            while (t < 1)
            {
                t = dt / rotationDuration;

                Quaternion newRotation = Quaternion.Lerp(startRotation, targetRotation, t);
                transform.rotation = newRotation;
                yield return null;

                dt += Time.deltaTime;
            }

            transform.rotation = targetRotation;
            rotatingRight = !rotatingRight;

            float pauseDuration = Random.Range(pauseBetweenSwivel, pauseBetweenSwivel * pauseDurationVariance);
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    [SerializeField] Transform blades;
    float revolutionTime = 6f;
    IEnumerator Rotate()
    {
        while (true)
        {
            float dt = 0;
            Quaternion start = blades.transform.rotation;
            Quaternion end = blades.transform.rotation * Quaternion.AngleAxis(120, Vector3.right);
            float fraction = revolutionTime / 3;

            while (dt < fraction)
            {
                Quaternion newRotation = Quaternion.Lerp(start, end, dt / fraction);
                blades.transform.rotation = newRotation;
                yield return null;

                dt += Time.deltaTime;
            }
            blades.transform.rotation = end;
        }
    }
}
