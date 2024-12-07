using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S3Camera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target; // ������ ��� (Player)
    public float smoothSpeed = 0.125f; // �̵� �ӵ�
    public Vector3 offset; // ī�޶� ������

    private Vector3 targetPosition;

    void FixedUpdate()
    {
        if (target != null)
        {
            // Ÿ�� ��ġ�� ������ �߰�
            targetPosition = target.position + offset;

            // Lerp�� ����Ͽ� �ε巴�� �̵�
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    public void SnapToTarget()
    {
        // Ÿ�� ��ġ�� ��� �̵�
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
