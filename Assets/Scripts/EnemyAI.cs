using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform[] checkpoints; // �̵��� üũ����Ʈ �迭
    public float speed = 0.8f; // �⺻ �ӵ�
    public float speedIncreaseRate = 0.05f; // 1�ʴ� �ӵ� ������
    public float defaultSpeed = 0.8f; // �ʱ� �ӵ�
    private int currentCheckpointIndex = 0; // ���� �̵� ���� üũ����Ʈ �ε���
    private bool isStopped = false; // ���� ���� Ȯ��
    private Rigidbody2D rb; // Rigidbody2D ������Ʈ
    private Vector2 moveDirection; // �̵� ����

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (checkpoints.Length > 0)
        {
            SetNextCheckpoint();
        }

        StartCoroutine(IncreaseSpeedOverTime());
    }

    void Update()
    {
        if (isStopped || checkpoints.Length == 0) return;

        // üũ����Ʈ �������� �̵�
        Vector2 targetPosition = checkpoints[currentCheckpointIndex].position;
        moveDirection = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = moveDirection * speed;

        // üũ����Ʈ ���� Ȯ��
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            AdvanceToNextCheckpoint();
        }
    }

    private void AdvanceToNextCheckpoint()
    {
        currentCheckpointIndex++;
        if (currentCheckpointIndex >= checkpoints.Length)
        {
            // ������ üũ����Ʈ�� �����ϸ� �̵� ����
            rb.velocity = Vector2.zero;
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }
        SetNextCheckpoint();
    }

    private void SetNextCheckpoint()
    {
        if (checkpoints.Length > 0)
        {
            moveDirection = (checkpoints[currentCheckpointIndex].position - transform.position).normalized;
        }
    }

    private IEnumerator IncreaseSpeedOverTime()
    {
        while (true)
        {
            if (!isStopped)
            {
                speed += speedIncreaseRate; // �ӵ� ����
            }
            yield return new WaitForSeconds(1f); // 1�� ���
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MirrorAttack"))
        {
            StartCoroutine(StopAndResetSpeed());
        }
    }

    private IEnumerator StopAndResetSpeed()
    {
        isStopped = true;
        rb.velocity = Vector2.zero; // ����
        speed = defaultSpeed; // �ӵ� �ʱ�ȭ
        yield return new WaitForSeconds(3f); // 3�� ���
        isStopped = false;
    }
}
