using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeEffect : MonoBehaviour
{
    public float CharPerSeconds;
    string targetMsg;
    TextMeshProUGUI msgText;

    int index;

    public TextMeshProUGUI MsgText => msgText; // �ܺο��� ������ �� �ִ� ������Ƽ �߰�


    // Start is called before the first frame update
    private void Awake()
    {
        msgText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void SetMsg(string msg)
    {
        targetMsg = msg;
        EffectStart();
    }
    void EffectStart()
    {
        msgText.text = "";
        index = 0;

        Invoke("Effecting", 1 / CharPerSeconds);
    }
    void Effecting()
    {
        if(msgText.text == targetMsg)
        {
            EffectEnd();
            return;
        }
        msgText.text += targetMsg[index];
        index++;

        Invoke("Effecting", 1 / CharPerSeconds);
    }
    void EffectEnd()
    {

    }
}
