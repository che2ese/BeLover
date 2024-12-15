using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S4Manager : MonoBehaviour
{
    private int currentButtonIndex = 1; // 플레이어가 밟아야 할 버튼의 순서
    public GameObject door;
    public GameObject house1;
    public GameObject house2;
    public GameObject blind;
    public GameObject mapLight;
    public float lightMoveSpeed = 5f;
    public Vector3[] lightPositions;
    
    private ResetStatue resetStatue;
    private Btn[] Btns;
    

    private void Start()
    {
        // 모든 버튼 스크립트 가져오기
        Btns = FindObjectsOfType<Btn>();
        resetStatue = FindObjectOfType<ResetStatue>();

        mapLight.transform.position = lightPositions[1];
    }

    public bool OnButtonPressed(int buttonID)
    {
        if (buttonID == currentButtonIndex) // 올바른 순서로 밟았을 때
        {
            Debug.Log($"Button {buttonID} Correct!");

            currentButtonIndex++;
            MoveLightToNextPosition();

            if (currentButtonIndex > 20) // 모든 버튼을 밟았을 때
            {
                Debug.Log("Door Opened!");
                door.SetActive(false);
                house1.SetActive(true);
                house2.SetActive(true);
                blind.SetActive(false);
            }
            return true;
        }
        else if (buttonID > currentButtonIndex)
        {
            Debug.Log($"Button {buttonID} Incorrect!");
            currentButtonIndex = 0; // 버튼 순서
            MoveLightToNextPosition();
            return false;
        }
        return true;
    }

    public void ResetButtons()
    {
        Debug.Log("Resetting Buttons!");
        currentButtonIndex = 1;
        MoveLightToNextPosition();
        foreach (Btn button in Btns)
        {
            button.ResetButton();
        }
        resetStatue.TriggerResetEffect();
    }

    private void MoveLightToNextPosition()
    {
        Debug.Log($"now : {currentButtonIndex}");
        Vector3 startPosition = mapLight.transform.position;
        Vector3 targetPosition = lightPositions[currentButtonIndex];
        StartCoroutine(MoveLightBezier(mapLight.transform, startPosition, targetPosition));
    }

    private IEnumerator MoveLightBezier(Transform lightTransform, Vector3 startPosition, Vector3 targetPosition)
    {
        float t = 0; // 곡선 진행도 (0~1)

        // 곡선 제어점 계산: 높이를 제한
        float maxHeight = 1f; // 곡선의 최대 높이
        Vector3 controlPoint = (startPosition + targetPosition) / 2 + Vector3.up * maxHeight;

        while (t < 1)
        {
            t += Time.deltaTime * lightMoveSpeed; // 진행 속도
            t = Mathf.Clamp01(t); // 0~1 사이로 제한

            // Bezier 곡선 계산
            Vector3 pointOnCurve = Mathf.Pow(1 - t, 2) * startPosition +
                                   2 * (1 - t) * t * controlPoint +
                                   Mathf.Pow(t, 2) * targetPosition;

            lightTransform.position = pointOnCurve;

            yield return null;
        }

        // 정확히 목표 위치로 설정
        lightTransform.position = targetPosition;
    }




}
