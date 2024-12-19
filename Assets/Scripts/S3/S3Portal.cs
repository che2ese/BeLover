using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Photon ���ӽ����̽� �߰�

public class S3Portal : MonoBehaviourPun
{
    public GameObject targetSpawnPoint;
    public GameObject pairedPortal; // ����� ��Ż�� �����մϴ�.

    public void OnPlayerEnter(GameObject player)
    {
        if (targetSpawnPoint != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.position = targetSpawnPoint.transform.position;
                Debug.Log($"Player moved to {targetSpawnPoint.transform.position}");
                // M1_2 ��Ż�� �۵��ϸ� M1_1 ��Ż�� Ȱ��ȭ�մϴ�.
                if (gameObject.name == "M1_2Portal" && pairedPortal != null)
                {
                    photonView.RPC("ActivatePairedPortal", RpcTarget.All, pairedPortal.name);
                    Debug.Log("M1_1 ��Ż�� Ȱ��ȭ�Ǿ����ϴ�.");
                }
                // M2_2 ��Ż�� �۵��ϸ� M2_1 ��Ż�� Ȱ��ȭ�մϴ�.
                else if (gameObject.name == "M2_2Portal" && pairedPortal != null)
                {
                    photonView.RPC("ActivatePairedPortal", RpcTarget.All, pairedPortal.name);
                    Debug.Log("M2_1 ��Ż�� Ȱ��ȭ�Ǿ����ϴ�.");
                }
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
                    // M1_2 ��Ż�� �۵��ϸ� M1_1 ��Ż�� Ȱ��ȭ�մϴ�.
                    if (gameObject.name == "M1_2Portal" && pairedPortal != null)
                    {
                        photonView.RPC("ActivatePairedPortal", RpcTarget.All, pairedPortal.name); 
                        Debug.Log("M1_1 ��Ż�� Ȱ��ȭ�Ǿ����ϴ�.");
                    }
                    // M2_2 ��Ż�� �۵��ϸ� M2_1 ��Ż�� Ȱ��ȭ�մϴ�.
                    else if (gameObject.name == "M2_2Portal" && pairedPortal != null)
                    {
                        photonView.RPC("ActivatePairedPortal", RpcTarget.All, pairedPortal.name);
                        Debug.Log("M2_1 ��Ż�� Ȱ��ȭ�Ǿ����ϴ�.");
                    }
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
    //��� Ŭ���̾�Ʈ�� ������ ��Ż Ȱ��ȭ �޼���
    [PunRPC]
    public void ActivatePairedPortal(string portalName)
    {
        GameObject portalToActivate = pairedPortal;
        if (portalToActivate != null)
        {
            portalToActivate.SetActive(true);
            Debug.Log($"{portalName} ��Ż�� Ȱ��ȭ�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogWarning($"{portalName} ��Ż�� ã�� �� �����ϴ�.");
        }
    }
}
