using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S3GameManager : MonoBehaviour
{
    public GameObject talkPanel;
    //public Text talkText;
    public GameObject scanObject;
    public bool isAction;

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
            //talkText.text = "�̰��� �̸��� " + scanObject.name + "�̶�� �Ѵ�.";
        }

        talkPanel.SetActive(isAction);

    }
}
