using System.Collections;
using UnityEngine;
using UnityEngine.UI; // **Image ������Ʈ�� ����ϱ� ���� �߰�**
using UnityEngine.SceneManagement; // **�� ��ȯ�� ���� �߰�**

public class StartManager : MonoBehaviour
{
    public Image panelImage; // **�г��� Image ������Ʈ ����**
    public Button lobbyButton; // **LobbyScene���� �̵��ϴ� ��ư ����**
    public float duration = 5f; // **���� ��ȭ�� �ɸ��� �ð�**
    public float fadeDuration = 3f; // **LobbyScene���� ��ȯ�� ���� ���̵� �ƿ� �ð�**

    private bool isColorChanging = false; // **���� ���� ������ Ȯ���ϴ� �÷���**

    private void Start()
    {
        // **�ڷ�ƾ ����**
        StartCoroutine(ChangePanelColor(new Color(0f, 0f, 0f), new Color(200f / 255f, 200f / 255f, 200f / 255f), duration));
    }

    /// <summary>
    /// **�г��� ������ ������ �����ϴ� �ڷ�ƾ**
    /// </summary>
    /// <param name="startColor">���� ����</param>
    /// <param name="endColor">�� ����</param>
    /// <param name="duration">���濡 �ɸ��� �ð�(��)</param>
    /// <returns></returns>
    private IEnumerator ChangePanelColor(Color startColor, Color endColor, float duration)
    {
        isColorChanging = true; // **���� ���� ������ �˸�**
        lobbyButton.interactable = false; // **��ư ��Ȱ��ȭ**

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // **���� ������ Lerp�� ���**
            panelImage.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            yield return null; // **���� �����ӱ��� ���**
        }

        // **���� ���� ����**
        panelImage.color = endColor;

        isColorChanging = false; // **���� ���� �Ϸ�**
        lobbyButton.interactable = true; // **��ư Ȱ��ȭ**
    }

    /// <summary>
    /// **LobbyScene���� �̵��ϴ� �Լ�**
    /// </summary>
    public void GoToLobbyScene()
    {
        // **���� ���� ���̶�� ��ư Ŭ���� ����**
        if (isColorChanging)
        {
            Debug.Log("���� ���� �߿��� ��ư�� ���� �� �����ϴ�.");
            return;
        }

        StartCoroutine(LoadLobbySceneWithFade());
    }

    /// <summary>
    /// **���̵� �ƿ� �� LobbyScene���� ��ȯ**
    /// </summary>
    private IEnumerator LoadLobbySceneWithFade()
    {
        float elapsedTime = 0f;

        // **���̵� �ƿ� (������ ��Ӱ�)**
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panelImage.color = Color.Lerp(new Color(200f / 255f, 200f / 255f, 200f / 255f), new Color(0f, 0f, 0f), elapsedTime / fadeDuration);
            yield return null;
        }

        // **�� ��ȯ (LobbyScene)**
        SceneManager.LoadScene("LobbyScene");
    }
}
