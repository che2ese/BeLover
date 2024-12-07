using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class S3SceneManager : MonoBehaviour
{
    //플레이어가 스캔한 오브젝트 가져오기 & UI 
    public TextMeshProUGUI talkText;
    public GameObject scanObject;

    public GameObject talkPanel;
    public bool isAction; //활성화 상태 판단 변수

    public S3_1TalkManager talkManager;
    public int talkIndex;

    public List<int> selectedOrder = new List<int>(); // 선택된 묘비 순서
    private int[] correctOrder = { 1002, 1005, 1004, 1001, 1003 }; // 정답 순서

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


            // 묘비인 경우 선택된 순서에 추가
            if (objData.isTomb)
            {
                selectedOrder.Add(objData.id);

                // 선택된 순서 검사
                CheckOrder();
            }
        }

        talkPanel.SetActive(isAction);
    }

    void CheckOrder()
    {
        //정답은 2번 > 5번 > 4번 > 1번 > 3번
        // 순서가 틀렸다면 초기화
        if (!IsCorrectSequence())
        {
            Debug.Log("잘못된 순서입니다! 다시 시도하세요.");
            selectedOrder.Clear();
        }
        else if (selectedOrder.Count == correctOrder.Length)
        {
            Debug.Log("퀘스트 완료! 문이 열립니다.");
            // 여기에서 다음 스텝(예: 문 열기)을 호출
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
