using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;


public class S3SceneManager : MonoBehaviour
{
    //플레이어가 스캔한 오브젝트 가져오기 & UI 
    public TextMeshProUGUI talkText;
    public GameObject scanObject;

    public GameObject talkPanel;
    public bool isAction; //활성화 상태 판단 변수

    public S3_1TalkManager talkManager;
    public int talkIndex;

    public static S3SceneManager Instance { get; private set; }


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

    // 정답 매핑 테이블
    private readonly Dictionary<string, string> correctPairs = new Dictionary<string, string>
    {
        { "Button1", "Road1" },
        { "Button2", "Road2" },
        { "Button3", "Road3" }
    };

    public static string p1ObjectName = "None";
    public static string p2ObjectName = "None";
    public bool isButtonInteracted = false; // 버튼 상호작용 여부

    public bool IsButtonInteracted()
    {
        return isButtonInteracted;
    }
    public void SetP1ObjectName(string objectName)
    {
        if (!isButtonInteracted) // 버튼 상호작용이 아직 이루어지지 않은 경우
        {
            isButtonInteracted = true;
            Debug.Log($"Player1이 버튼 {objectName}을(를) 상호작용했습니다.");
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
        // Button과 Road 이름이 모두 설정된 경우
        if (p1ObjectName != "None" && p2ObjectName != "None")
        {
            // 매핑 테이블에서 정답 확인
            if (correctPairs.TryGetValue(p1ObjectName, out string correctRoad) && correctRoad == p2ObjectName)
            {
                Debug.Log($"정답! {p1ObjectName} ↔ {p2ObjectName} 매칭 성공!");
                OnCorrectMatch();
            }
            else
            {
                Debug.Log($"오답! {p1ObjectName} ↔ {p2ObjectName} 매칭 실패!");
                OnIncorrectMatch();
            }

            // 매칭 이름 초기화
            p1ObjectName = "None";
            p2ObjectName = "None";
        }
    }

    private void OnCorrectMatch()
    {
        Debug.Log("정답에 대한 보상을 줍니다. 속도가 증가합니다.");
        PlayerScript[] players = FindObjectsOfType<PlayerScript>();
        foreach (var player in players)
        {
            if (player.CompareTag("player2"))
            {
                player.photonView.RPC("SetSpeedRPC", RpcTarget.All, 1f); // 모든 클라이언트에 속도 변경
                Debug.Log("Player 2의 속도가 1로 설정되었습니다.");
            }
        }
    }

    private void OnIncorrectMatch()
    {
        Debug.Log("오답에 대한 페널티를 줍니다. 속도가 감소합니다.");
        PlayerScript[] players = FindObjectsOfType<PlayerScript>();
        foreach (var player in players)
        {
            if (player.CompareTag("player2"))
            {
                player.photonView.RPC("SetSpeedRPC", RpcTarget.All, 0.5f); // 모든 클라이언트에 속도 변경
                Debug.Log("Player 2의 속도가 0.5로 설정되었습니다.");
            }
        }
    }

    //M2 gimmick

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
            Talk(objData.id, objData.isTomb,objData.SkullTrue);
            /*
            if (objData != null)
            {
                Talk(objData.id, objData.isTomb,objData.SkullTrue); // id를 전달하여 대화 처리
            }*/
        }

        talkPanel.SetActive(isAction);
    }

    //s3 m2 
    //퀴즈 출력 하면 될거 같은데
    public void Talk(int id, bool isTomb,bool skullTrue)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);
        if (isTomb)
        {
            talkText.text = talkData;
        }
        else if (skullTrue)
        {
            talkText.text = talkData;
        }
        else
        {
            talkText.text = talkData;
        }
        
    } 
}
