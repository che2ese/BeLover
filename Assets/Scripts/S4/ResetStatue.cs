using System.Collections;
using UnityEngine;

public class ResetStatue : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeIntensity = 0.02f;

    private Vector3 originalPosition; // 동상의 원래 위치

    private void Start()
    {
        // 동상의 초기 위치 저장
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if(Input.GetButtonDown("Jump")) {
            TriggerResetEffect();
        }

    }

    public void TriggerResetEffect()
    {
        // 흔들림 효과 시작
        StartCoroutine(ShakeEffect());
    }

    private IEnumerator ShakeEffect()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // X축으로만 흔들림 적용
            float offsetX = Random.Range(-shakeIntensity * 0.1f, shakeIntensity * 0.1f);

            float rotationZ = Random.Range(-shakeIntensity * 60, shakeIntensity * 60);

            transform.localPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y, originalPosition.z);
            transform.localRotation = Quaternion.Euler(0, 0, rotationZ);

            elapsed += Time.deltaTime;
            yield return null; // 한 프레임 대기
        }

        // 흔들림이 끝나면 원래 위치로 복원
        transform.localPosition = originalPosition;
        transform.localRotation = Quaternion.identity;
    }
}
