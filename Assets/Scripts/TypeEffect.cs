using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeEffect : MonoBehaviour
{
    public float CharPerSeconds; // Ÿ���� �ӵ�
    public AudioClip typingSound; // Ÿ���� �Ҹ� ����
    private AudioSource audioSource; // ����� �ҽ��� ������ ����

    string targetMsg; // ����� ��ü �޽���
    TextMeshProUGUI msgText; // �ؽ�Ʈ ��¿� TextMeshProUGUI

    int index; // ���� ��� ���� ���� �ε���

    public TextMeshProUGUI MsgText => msgText; // �ܺο��� ������ �� �ִ� ������Ƽ �߰�

    private void Awake()
    {
        msgText = GetComponent<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource�� �Ҵ���� �ʾҽ��ϴ�. TypeEffect ������Ʈ�� AudioSource ������Ʈ�� �߰��ϼ���.");
        }

        if (typingSound != null)
        {
            audioSource.clip = typingSound;
        }
    }

    public void SetMsg(string msg)
    {
        targetMsg = msg;
        EffectStart();
    }

    void EffectStart()
    {
        msgText.text = ""; // �ؽ�Ʈ �ʱ�ȭ
        index = 0;

        if (audioSource != null && typingSound != null)
        {
            audioSource.clip = typingSound; // ����� Ŭ�� �Ҵ�
            audioSource.loop = true; // �ݺ� ���
            audioSource.Play(); // �Ҹ� ���
        }

        Invoke("Effecting", 1 / CharPerSeconds);
    }

    void Effecting()
    {
        if (msgText.text == targetMsg)
        {
            EffectEnd();
            return;
        }

        msgText.text += targetMsg[index]; // �� ���� �߰�
        index++;

        Invoke("Effecting", 1 / CharPerSeconds); // ���� ���� ���
    }

    void EffectEnd()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.loop = false; // ���� ����
            audioSource.Stop(); // �Ҹ� ����
        }
    }
}
