using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S3_1TalkManager : MonoBehaviour
{
    //Dictionary Key-Value
    Dictionary<int, string[]> talkdata;
    void Awake()
    {
        talkdata = new Dictionary<int, string[]>();
        GenerateData();
    }

    // Update is called once per frame
    void GenerateData()
    {
        //statue와 상호작용시 이제 퀘스트 안내
        talkdata.Add(100, new string[] { });

        // 묘비 퀴즈 데이터
        talkdata.Add(1001, new string[]
        {
        "나는 마지막에서 두 번째로 눌려야 한다.",
        "5번 묘비는 진실만 말한다."
        });

        talkdata.Add(1002, new string[]
        {
        "나는 첫 번째로 눌려야 한다.",
        "4번 묘비는 나보다 뒤에 있어야 한다."
        });

        talkdata.Add(1003, new string[]
        {
        "나는 1번 묘비보다 앞서 눌려야 한다.",
        "2번 묘비는 거짓말을 하고 있다."
        });

        talkdata.Add(1004, new string[]
        {
        "나는 5번 묘비 바로 앞에 눌려야 한다.",
        "3번 묘비는 나보다 뒤에 있어야 한다."
        });

        talkdata.Add(1005, new string[]
        {
        "나는 2번 묘비 바로 뒤에 눌려야 한다.",
        "4번 묘비는 진실만 말한다."
        });
    }
    //지정된 대화 문장을 반환하는 함수 하나 생성
    public string GetTalk(int id, int talkIndex) //talkindex
    {
        return talkdata[id][talkIndex];
    }
}
