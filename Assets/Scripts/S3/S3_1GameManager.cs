using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class S3_1GameManager : MonoBehaviour
{
    //�÷��̾ ��ĵ�� ������Ʈ �������� & UI 
    public TextMeshProUGUI talkText;
    public GameObject scanObject;

    public GameObject talkPanel;
    public bool isAction; //Ȱ��ȭ ���� �Ǵ� ����

    public S3_1TalkManager talkManager;
    public int talkIndex;

    public List<int> selectedOrder = new List<int>(); // ���õ� ���� ����
    private int[] correctOrder = { 1002, 1005, 1004, 1001, 1003 }; // ���� ����

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
