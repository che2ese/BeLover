using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S3ObjectData : MonoBehaviour
{
    public int id; // ���� ID
    public bool isTomb; // ���� ����
    public bool SkullTrue;
    public bool isActivated = false; // ��ȣ�ۿ� ���θ� ���
    public GameObject linkedCandleFire; // ����� CandleFire ������Ʈ
}
