using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private float shakeDuration = 0.2f;
    private float shakeSpentTime = 0f;

    [SerializeField]
    private float shakeAmount = 0.2f;

    [SerializeField]
    private float decreaseFactor = 1.0f;

    Vector3 originalPos;

    public void Shake()
    {
        if (shakeSpentTime == 0)
        {
            originalPos = transform.localPosition;
        }
        shakeSpentTime = shakeDuration;
        StartCoroutine(Shake_coroutine());
    }

    private IEnumerator Shake_coroutine()
    {
        yield return new WaitForSeconds(shakeDuration);
        transform.localPosition = originalPos;
    }

    private void Update()
    {
        if (shakeSpentTime > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeSpentTime -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeSpentTime = 0f;
        }
    }
}