using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // **UI �̹��� ó���� ���� �߰�**
using TMPro;

public class S2SceneManager : MonoBehaviour
{
    public Camera mainCamera; // ����� ī�޶� (Camera.main ��� ���)
    public TextMeshProUGUI warningText; // **TMP ��� �޽��� �ؽ�Ʈ ����**
    public Image displayImage; // **ǥ���� �̹��� ����**
    public Sprite[] imageList; // **�̹��� ����Ʈ (5�� ��������Ʈ �Է�)**
    public GameObject prefabToShow; // **Ȱ��ȭ�� Prefab**
    public int mirrorCount = 0; // �̷� ī��Ʈ
    private int previousMirrorCount = 0; // ���� �̷� ī��Ʈ�� ����
    private bool isShaking = false; // ��鸲 ������ ���� Ȯ��

    void Start()
    {
        if (mainCamera == null)
        {
            Debug.LogError("[S2SceneManager] mainCamera�� ������� �ʾҽ��ϴ�. ī�޶� �����ϼ���.");
        }

        if (warningText == null)
        {
            Debug.LogError("[S2SceneManager] warningText�� ������� �ʾҽ��ϴ�. TMP ������Ʈ�� �����ϼ���.");
        }
        else
        {
            warningText.gameObject.SetActive(false); // ������ �� ��� �޽����� ����
        }

        if (displayImage == null)
        {
            Debug.LogError("[S2SceneManager] displayImage�� ������� �ʾҽ��ϴ�. UI Image ������Ʈ�� �����ϼ���.");
        }
        else
        {
            displayImage.preserveAspect = false; // **Preserve Aspect ��Ȱ��ȭ**
        }

        if (imageList == null || imageList.Length == 0)
        {
            Debug.LogError("[S2SceneManager] imageList�� �̹����� �������� �ʾҽ��ϴ�.");
        }

        if (prefabToShow == null)
        {
            Debug.LogError("[S2SceneManager] prefabToShow�� ������� �ʾҽ��ϴ�. Prefab ������Ʈ�� �����ϼ���.");
        }
        else
        {
            prefabToShow.SetActive(false); // **Prefab�� ��Ȱ��ȭ�� ���·� ����**
        }

        if (imageList.Length > 0)
        {
            displayImage.sprite = imageList[0];
            displayImage.rectTransform.sizeDelta = new Vector2(160, 180); // **�̹��� ũ�� ����**
        }
    }

    void Update()
    {
        if (mirrorCount > previousMirrorCount)
        {
            Debug.Log($"[S2SceneManager] mirrorCount�� {previousMirrorCount}���� {mirrorCount}�� �����߽��ϴ�.");

            // **mirrorCount�� 5�� �ƴ� ���� ī�޶� ��鸲 �߻�**
            if (mirrorCount != 5)
            {
                StartCoroutine(CameraShake(3f));
            }

            StartCoroutine(ShowWarningMessage("�÷��̾��� ����Ű�� �������� �缳���˴ϴ�"));
            ChangeImage(mirrorCount);

            if (mirrorCount == 5)
            {
                // **Prefab Ȱ��ȭ �� ī�޶� �̵�**
                StartCoroutine(ShowPrefabAndSwitchCamera());
            }

            previousMirrorCount = mirrorCount;
        }
    }

    IEnumerator CameraShake(float duration)
    {
        if (isShaking) yield break;
        isShaking = true;

        if (mainCamera == null)
        {
            Debug.LogError("[S2SceneManager] mainCamera�� ������� �ʾҽ��ϴ�. ��鸲�� �ߴ��մϴ�.");
            yield break;
        }

        float elapsed = 0f;
        Matrix4x4 originalProjectionMat = mainCamera.projectionMatrix;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Matrix4x4 pMat = originalProjectionMat;

            pMat.m01 += Mathf.Sin(Time.time * 25f) * 0.2f;
            pMat.m10 += Mathf.Sin(Time.time * 30f) * 0.2f;

            pMat.m00 += Mathf.Sin(Time.time * 20f) * 0.2f;
            pMat.m11 += Mathf.Sin(Time.time * 20f) * 0.2f;

            mainCamera.projectionMatrix = pMat;

            yield return null;
        }

        mainCamera.projectionMatrix = originalProjectionMat;
        isShaking = false;
    }

    IEnumerator ShowWarningMessage(string message)
    {
        if (warningText == null)
        {
            Debug.LogError("[S2SceneManager] warningText�� ������� �ʾҽ��ϴ�.");
            yield break;
        }

        warningText.text = message;
        warningText.color = Color.red;

        for (int i = 0; i < 3; i++)
        {
            warningText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            warningText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }

        warningText.gameObject.SetActive(false);
    }

    void ChangeImage(int index)
    {
        if (imageList == null || imageList.Length == 0) return;

        if (index < imageList.Length)
        {
            displayImage.sprite = imageList[index];
            displayImage.rectTransform.sizeDelta = new Vector2(160, 180);
        }
    }

    IEnumerator ShowPrefabAndSwitchCamera()
    {
        if (prefabToShow == null || mainCamera == null)
        {
            Debug.LogError("[S2SceneManager] prefabToShow �Ǵ� mainCamera�� ������� �ʾҽ��ϴ�.");
            yield break;
        }

        Debug.Log("[S2SceneManager] Prefab Ȱ��ȭ �� ī�޶� ��ȯ");

        // **Prefab Ȱ��ȭ**
        prefabToShow.SetActive(true);

        // **���� ī�޶��� ��ġ�� ȸ�� ����**
        Vector3 originalCameraPosition = mainCamera.transform.position;
        Quaternion originalCameraRotation = mainCamera.transform.rotation;

        // **Prefab ��ġ�� ī�޶� �̵�**
        Vector3 targetPosition = prefabToShow.transform.position + new Vector3(0, 0, -10);
        float transitionTime = 2f; // **ī�޶� ��ȯ�� �ɸ��� �ð�**
        float elapsedTime = 0f;

        // **ī�޶� Prefab ��ġ�� �̵� (2�� ���� �ε巴�� ��ȯ)**
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            mainCamera.transform.position = Vector3.Lerp(originalCameraPosition, targetPosition, elapsedTime / transitionTime);
            mainCamera.transform.rotation = Quaternion.Slerp(originalCameraRotation, Quaternion.LookRotation(prefabToShow.transform.position - mainCamera.transform.position), elapsedTime / transitionTime);
            yield return null;
        }

        // **Prefab�� ���� �� ���� ��ġ�� ����**
        elapsedTime = -2f;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            mainCamera.transform.position = Vector3.Lerp(targetPosition, originalCameraPosition, elapsedTime / transitionTime);
            mainCamera.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(prefabToShow.transform.position - mainCamera.transform.position), originalCameraRotation, elapsedTime / transitionTime);
            yield return null;
        }

        Debug.Log("[S2SceneManager] Prefab ��Ȱ��ȭ �� ī�޶� ���� �Ϸ�");
    }

}
