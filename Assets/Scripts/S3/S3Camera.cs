using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S3Camera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target; // 추적할 대상 (Player)
    public float smoothSpeed = 0.125f; // 이동 속도
    public Vector3 offset; // 카메라 오프셋

    private Vector3 targetPosition;

    void FixedUpdate()
    {
        if (target != null)
        {
            // 타겟 위치에 오프셋 추가
            targetPosition = target.position + offset;

            // Lerp를 사용하여 부드럽게 이동
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    public void SnapToTarget()
    {
        // 타겟 위치로 즉시 이동
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
