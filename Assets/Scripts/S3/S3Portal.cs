using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S3Portal : MonoBehaviour
{
    public GameObject targetSpawnPoint;

    public void OnPlayerEnter(GameObject player)
    {
        if (targetSpawnPoint != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.position = targetSpawnPoint.transform.position;
                Debug.Log($"Player moved to {targetSpawnPoint.transform.position}");
            }
        }
        else
        {
            Debug.LogWarning("Target spawn point is not assigned!");
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (targetSpawnPoint != null)
            {
                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // �÷��̾� �̵�
                    rb.position = targetSpawnPoint.transform.position;
                    Debug.Log($"Player moved to {targetSpawnPoint.transform.position}");
                }

                // ī�޶� �̵�
                S3Camera cameraFollow = Camera.main.GetComponent<S3Camera>();
                if (cameraFollow != null)
                {
                    cameraFollow.SnapToTarget(); // ��� �̵�
                }
            }
            else
            {
                Debug.LogWarning("Target spawn point is not assigned!");
            }
        }
    }
}
