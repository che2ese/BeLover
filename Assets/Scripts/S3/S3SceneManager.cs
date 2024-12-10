using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class S3SceneManager : MonoBehaviour
{
    //�÷��̾ ��ĵ�� ������Ʈ �������� & UI 
    public TextMeshProUGUI talkText;
    public GameObject scanObject;

    public GameObject talkPanel;
    public bool isAction; //Ȱ��ȭ ���� �Ǵ� ����

    public S3_1TalkManager talkManager;
    public int talkIndex;

    public static S3SceneManager Instance { get; private set; }


    public List<int> selectedOrder = new List<int>(); // ���õ� ���� ����
    private int[] correctOrder = { 1002, 1005, 1004, 1001, 1003 }; // ���� ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ���� ���� ���̺�
    private readonly Dictionary<string, string> correctPairs = new Dictionary<string, string>
    {
        { "Button1", "Road1" },
        { "Button2", "Road2" },
        { "Button3", "Road3" }
    };

    public static string p1ObjectName = "None";
    public static string p2ObjectName = "None";
    public bool isButtonInteracted = false; // ��ư ��ȣ�ۿ� ����

    public bool IsButtonInteracted()
    {
        return isButtonInteracted;
    }
    public void SetP1ObjectName(string objectName)
    {
        if (!isButtonInteracted) // ��ư ��ȣ�ۿ��� ���� �̷������ ���� ���
        {
            isButtonInteracted = true;
            Debug.Log($"Player1�� ��ư {objectName}��(��) ��ȣ�ۿ��߽��ϴ�.");
        }
        p1ObjectName = objectName;
        CheckMatch();
    }

    public void SetP2ObjectName(string objectName)
    {
        p2ObjectName = objectName;
        CheckMatch();
    }

    private void CheckMatch()
    {
        // Button�� Road �̸��� ��� ������ ���
        if (p1ObjectName != "None" && p2ObjectName != "None")
        {
            // ���� ���̺��� ���� Ȯ��
            if (correctPairs.TryGetValue(p1ObjectName, out string correctRoad) && correctRoad == p2ObjectName)
            {
                Debug.Log($"����! {p1ObjectName} �� {p2ObjectName} ��Ī ����!");
                OnCorrectMatch();
            }
            else
            {
                Debug.Log($"����! {p1ObjectName} �� {p2ObjectName} ��Ī ����!");
                OnIncorrectMatch();
            }

            // ��Ī �̸� �ʱ�ȭ
            p1ObjectName = "None";
            p2ObjectName = "None";
        }
    }

    private void OnCorrectMatch()
    {
        Debug.Log("���信 ���� ������ �ݴϴ�. �ӵ��� �����մϴ�.");
        PlayerScript[] players = FindObjectsOfType<PlayerScript>();
        foreach (var player in players)
        {
            if (player.CompareTag("player2"))
            {
                player.photonView.RPC("SetSpeedRPC", RpcTarget.All, 1f); // ��� Ŭ���̾�Ʈ�� �ӵ� ����
                Debug.Log("Player 2�� �ӵ��� 1�� �����Ǿ����ϴ�.");
            }
        }
    }

    private void OnIncorrectMatch()
    {
        Debug.Log("���信 ���� ���Ƽ�� �ݴϴ�. �ӵ��� �����մϴ�.");
        PlayerScript[] players = FindObjectsOfType<PlayerScript>();
        foreach (var player in players)
        {
            if (player.CompareTag("player2"))
            {
                player.photonView.RPC("SetSpeedRPC", RpcTarget.All, 0.5f); // ��� Ŭ���̾�Ʈ�� �ӵ� ����
                Debug.Log("Player 2�� �ӵ��� 0.5�� �����Ǿ����ϴ�.");
            }
        }
    }


    public void Action(GameObject scanObj)
    {
        if (isAction)
        {
            isAction = false;
        }
        else
        {
            isAction = true;
            scanObject = scanObj;
            S3ObjectData objData = scanObj.GetComponent<S3ObjectData>();
            Talk(objData.id, objData.isTomb);


            // ������ ��� ���õ� ������ �߰�
            if (objData.isTomb)
            {
                selectedOrder.Add(objData.id);

                // ���õ� ���� �˻�
                CheckOrder();
            }
        }

        talkPanel.SetActive(isAction);
    }

    void CheckOrder()
    {
        //������ 2�� > 5�� > 4�� > 1�� > 3��
        // ������ Ʋ�ȴٸ� �ʱ�ȭ
        if (!IsCorrectSequence())
        {
            Debug.Log("�߸��� �����Դϴ�! �ٽ� �õ��ϼ���.");
            selectedOrder.Clear();
        }
        else if (selectedOrder.Count == correctOrder.Length)
        {
            Debug.Log("����Ʈ �Ϸ�! ���� �����ϴ�.");
            // ���⿡�� ���� ����(��: �� ����)�� ȣ��
        }
    }

    bool IsCorrectSequence()
    {
        for (int i = 0; i < selectedOrder.Count; i++)
        {
            if (selectedOrder[i] != correctOrder[i])
            {
                return false;
            }
        }
        return true;
    }

    void Talk(int id, bool isTomb)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);
        if (talkData == null)
        {
            talkIndex = 0;
            isAction = false;
            return;
        }
        talkText.text = talkData;

    }
}
