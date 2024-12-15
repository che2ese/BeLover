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
            "������ �� �ϳ�"
        });

        talkdata.Add(1001, new string[]
        {
            "�ͺ� �ذ� 6��°"
        });

        talkdata.Add(1002, new string[]
        {
            "��� �ذ� 2��°"
        });

        talkdata.Add(1003, new string[]
        {
            "�ټ���° ������ �Ǵ�"
        });

        talkdata.Add(1004, new string[]
        {
            "���� �ڴ� ù��° ���ۿ� ����"
        });
        talkdata.Add(1005, new string[]
        { 
            "�𷧺� �ذ� 1��°"
        });
        talkdata.Add(1006, new string[]
        {
            "����° ���� ������ �̾߱� �Ѵ�"
        });
        talkdata.Add(2001, new string[]
        {
            "����."
        });
        talkdata.Add(2002, new string[]
        {
            "����."
        });
    }

    //������ ��ȭ ������ ��ȯ�ϴ� �Լ� �ϳ� ����
    public string GetTalk(int id, int talkIndex) //talkindex
    {
        return talkdata[id][talkIndex];
    }


}
