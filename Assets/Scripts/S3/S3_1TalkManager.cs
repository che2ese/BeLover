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
        //statue�� ��ȣ�ۿ�� ���� ����Ʈ �ȳ�
        talkdata.Add(100, new string[] { });

        // ���� ���� ������
        talkdata.Add(1001, new string[]
        {
        "���� ���������� �� ��°�� ������ �Ѵ�.",
        "5�� ����� ���Ǹ� ���Ѵ�."
        });

        talkdata.Add(1002, new string[]
        {
        "���� ù ��°�� ������ �Ѵ�.",
        "4�� ����� ������ �ڿ� �־�� �Ѵ�."
        });

        talkdata.Add(1003, new string[]
        {
        "���� 1�� ���񺸴� �ռ� ������ �Ѵ�.",
        "2�� ����� �������� �ϰ� �ִ�."
        });

        talkdata.Add(1004, new string[]
        {
        "���� 5�� ���� �ٷ� �տ� ������ �Ѵ�.",
        "3�� ����� ������ �ڿ� �־�� �Ѵ�."
        });

        talkdata.Add(1005, new string[]
        {
        "���� 2�� ���� �ٷ� �ڿ� ������ �Ѵ�.",
        "4�� ����� ���Ǹ� ���Ѵ�."
        });
    }
    //������ ��ȭ ������ ��ȯ�ϴ� �Լ� �ϳ� ����
    public string GetTalk(int id, int talkIndex) //talkindex
    {
        return talkdata[id][talkIndex];
    }
}
