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
        //statue
        talkdata.Add(3000, new string[]
        {
            "진실은 단 하나"
        });

        talkdata.Add(1001, new string[]
        {
            "핏빛 해골 6번째"
        });

        talkdata.Add(1002, new string[]
        {
            "잿빛 해골 2번째"
        });

        talkdata.Add(1003, new string[]
        {
            "다섯번째 묘지는 옳다"
        });

        talkdata.Add(1004, new string[]
        {
            "옳은 자는 첫번째 묘밖에 없다"
        });
        talkdata.Add(1005, new string[]
        { 
            "모랫빛 해골 1번째"
        });
        talkdata.Add(1006, new string[]
        {
            "세번째 묘는 진실을 이야기 한다"
        });
        talkdata.Add(2001, new string[]
        {
            "오답."
        });
        talkdata.Add(2002, new string[]
        {
            "정답."
        });
    }

    //지정된 대화 문장을 반환하는 함수 하나 생성
    public string GetTalk(int id, int talkIndex) //talkindex
    {
        return talkdata[id][talkIndex];
    }


}
