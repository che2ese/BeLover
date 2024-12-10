using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image backgroundImage; // ��� �̹����� �����մϴ�.
    public float moveDuration = 2f; // �¿�� �����̴� �� �ɸ��� �ð�
    public float moveSpeed = 0.1f; // ����� �̵� �ӵ�
    public float scaleDuration = 2f; // Ȯ�� �� ��ҿ� �ɸ��� �ð�
    public Vector2 minMaxScale = new Vector2(0.8f, 1.2f); // �ּ�/�ִ� ������ ����

    [Range(0f, 1f)]
    public float minOffsetX = 0f; // �ؽ�ó �̵��� �ּ� x ���� (0 ~ 1)
    [Range(0f, 1f)]
    public float maxOffsetX = 0.2f; // �ؽ�ó �̵��� �ִ� x ���� (0 ~ 1)

    void Start()
    {
        if (backgroundImage == null)
        {
            Debug.LogError("Background Image�� �����ؾ� �մϴ�.");
            return;
        }

        // Image�� Material�� �ν��Ͻ�ȭ�Ͽ� ���������� ������ �� �ֵ��� ��
        backgroundImage.material = new Material(backgroundImage.material);

        // �ݺ� �ִϸ��̼� ����
        StartCoroutine(AnimateBackground());
    }

    IEnumerator AnimateBackground()
    {
        while (true)
        {
            // 1. Ȯ��
            yield return StartCoroutine(ScaleImage(minMaxScale.y, scaleDuration * 2));

            // 2. �·� �̵�
            yield return StartCoroutine(MoveBackground(Vector2.left, moveDuration));

            // 3. ��� �̵�
            yield return StartCoroutine(MoveBackground(Vector2.right, moveDuration * 3)); // �� �� �ð����� ������ �̵�

            // 4. �·� �ٽ� �̵� (���� ��ġ��)
            yield return StartCoroutine(MoveBackground(Vector2.left, moveDuration * 3));

            // 5. ���� ũ��� ���� (1.0���� ����)
            yield return StartCoroutine(ScaleImage(1.0f, scaleDuration * 2));
        }
    }

    IEnumerator ScaleImage(float targetScale, float duration)
    {
        float time = 0f;
        Vector3 initialScale = backgroundImage.rectTransform.localScale;
        Vector3 targetScaleVector = new Vector3(targetScale, targetScale, 1f);

        while (time < duration)
        {
            time += Time.deltaTime;
            backgroundImage.rectTransform.localScale = Vector3.Lerp(initialScale, targetScaleVector, time / duration);
            yield return null;
        }

        backgroundImage.rectTransform.localScale = targetScaleVector; // ��Ȯ�� ���߱� ���ؼ� �������� ����
    }

    IEnumerator MoveBackground(Vector2 direction, float duration)
    {
        float time = 0f;
        Vector2 initialOffset = backgroundImage.material.mainTextureOffset;

        while (time < duration)
        {
            time += Time.deltaTime;
            // mainTextureOffset�� �����Ͽ� �̹����� �ҽ� �κ��� �̵���ŵ�ϴ�.
            Vector2 newOffset = initialOffset + direction * (moveSpeed * time / duration);

            // ���ο� x �������� ���� ���� �����մϴ�.
            newOffset.x = Mathf.Clamp(newOffset.x, minOffsetX, maxOffsetX);

            // ������ ����
            backgroundImage.material.mainTextureOffset = newOffset;
            yield return null;
        }

        // �������� ���������� �ٽ� Ŭ�����Ͽ� ��Ȯ�� ����
        Vector2 finalOffset = initialOffset + direction * moveSpeed;
        finalOffset.x = Mathf.Clamp(finalOffset.x, minOffsetX, maxOffsetX);
        backgroundImage.material.mainTextureOffset = finalOffset;
    }
}
