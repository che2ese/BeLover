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

    void Start()
    {
        // �ڵ����� Player�� ã�� ī�޶��� Ÿ������ ����
        if (target == null)
        {
            TryFindPlayer();
        }
    }

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

    // Player ������Ʈ�� ã�� �Լ�
    public void TryFindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            Debug.Log("�÷��̾ ã�ҽ��ϴ�: " + target.name);
        }
        else
        {
            Debug.LogWarning("Player�� ã�� �� �����ϴ�. ���߿� �ٽ� �õ��մϴ�.");
            Invoke(nameof(TryFindPlayer), 1f); // 1�� �� �ٽ� �õ�
        }
    }

    // Player�� �������� ������ �� ȣ��
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log("Ÿ���� �������� �����Ǿ����ϴ�: " + target.name);
    }
}
