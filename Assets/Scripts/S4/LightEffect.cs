using UnityEngine;

public class LightEffect : MonoBehaviour
{
    public float minScale = 0.8f; // 최소 크기
    public float maxScale = 1.2f; // 최대 크기
    public float speed = 0.5f;    // 크기 변경 속도 (낮은 값으로 느리게 설정 가능)

    private Vector3 targetScale;  // 목표 크기
    private Vector3 originalScale; // 원래 크기

    void Start()
    {
        // 초기 크기를 현재 크기로 설정
        originalScale = transform.localScale;

        // 랜덤 목표 크기 설정
        SetRandomTargetScale();
    }

    void Update()
    {
        // 현재 크기에서 목표 크기로 부드럽게 전환
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, speed * Time.deltaTime);

        // 목표 크기에 가까워지면 새로운 랜덤 크기 설정
        if (Vector3.Distance(transform.localScale, targetScale) < 0.01f)
        {
            SetRandomTargetScale();
        }
    }

    // 새로운 랜덤 목표 크기 설정
    private void SetRandomTargetScale()
    {
        float randomScale = Random.Range(minScale, maxScale);
        targetScale = originalScale * randomScale;
    }
}
