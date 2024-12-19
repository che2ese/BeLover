using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class MainSceneManager : MonoBehaviour
{
    NetworkManager nm;
    TalkManager tm;

    public GameObject storyPanel;
    public Button StartBtn;
    public Image CinemaImage1;
    public Image CinemaImage2;

    public GameObject StoryPanel;
    public TextMeshProUGUI StoryText;
    public TypeEffect CinemaText;

    public GameObject[] mainMission;
    public Sprite[] GhostImage;
    public Image ghostImage;

    public bool isCinemaFinished = false;
    public bool isStart = false;

    void Awake()
    {
        nm = FindObjectOfType<NetworkManager>();
        tm = FindObjectOfType<TalkManager>();

        // ���� ����� �� �׻� storyPanel�� Ȱ��ȭ
        storyPanel.SetActive(true);
        StartBtn.gameObject.SetActive(false);

        // �ʿ��ϸ� PlayerPrefs �ʱ�ȭ
        PlayerPrefs.SetInt("StoryPanelHidden", 0);
    }
    private void Start()
    {
        // ���� ���۵Ǹ� �ó׸�ƽ �ִϸ��̼� ����
        StartCoroutine(CinemaSequence());
    }
    void Update()
    {
        if (tm.isDialogueFinished && isStart == false)
        {
            StartBtn.gameObject.SetActive(true);
        }

        if (GameManager.Instance.mainSceneEnterCount >= 2)
        {
            storyPanel.SetActive(false);
            if (GameManager.Instance.mainSceneEnterCount == 2 && GameManager.Instance.isMission1Clear == true && GameManager.Instance.isMission2Clear == false && GameManager.Instance.isMission3Clear == false)
            {
                mainMission[0].tag = "MainMission";
            }
            else
                mainMission[0].tag = "Untagged";

            if (GameManager.Instance.mainSceneEnterCount == 3 && GameManager.Instance.isMission1Clear == true && GameManager.Instance.isMission2Clear == true && GameManager.Instance.isMission3Clear == false)
            {
                mainMission[1].tag = "MainMission";
            }
            else
                mainMission[1].tag = "Untagged";

            if (GameManager.Instance.mainSceneEnterCount == 4 && GameManager.Instance.isMission1Clear == true && GameManager.Instance.isMission2Clear == true && GameManager.Instance.isMission3Clear == true)
            {
                mainMission[2].tag = "MainMission";
            }
            else
                mainMission[2].tag = "Untagged";
        }
    }

    public void SpawnChar()
    {
        isStart = true;
        // ��ư�� ��Ȱ��ȭ�Ͽ� �� �̻� ������ �ʰ� ����ϴ�.
        StartBtn.gameObject.SetActive(false);

        // storyPanel ���� ���� ����
        PlayerPrefs.SetInt("StoryPanelHidden", 1);

        // Spawn ���� ViewID ����
        nm.Spawn();

        // ��ư�� ���� �÷��̾� ���� ������Ʈ
        UpdatePlayerReadyStatus();

        // �� �÷��̾ �غ� �������� Ȯ��
        StartCoroutine(CheckBothPlayersReady());
    }

    void UpdatePlayerReadyStatus()
    {
        // ���� �÷��̾ �غ� �������� ����
        var props = new ExitGames.Client.Photon.Hashtable
        {
            { "IsReady", true }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    IEnumerator CheckBothPlayersReady()
    {
        while (true)
        {
            // ��� �÷��̾��� CustomProperties Ȯ��
            bool allReady = true;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("IsReady"))
                {
                    allReady &= (bool)player.CustomProperties["IsReady"];
                }
                else
                {
                    allReady = false;
                }
            }

            // �� �÷��̾ ��� �غ� ���¸� Scene1���� �̵�
            if (allReady)
            {
                yield return new WaitForSeconds(3f); // 3�� ���
                nm.StartScene1(); // Scene1���� �̵�
                yield break; // Coroutine ����
            }

            yield return null; // ���� �����ӱ��� ���
        }
    }

    // =========================
    //   �ó׸�ƽ �ִϸ��̼� �κ�
    // =========================

    IEnumerator CinemaSequence()
    {
        CinemaText.SetMsg("��ο� �� �� ���ڰ� ���ο��ϰ� �ִ�");

        yield return new WaitForSeconds(3f); // 3�� ��� �� ���̵� �ƿ� ����

        // CinemaImage1 ���̵� �ƿ��� CinemaImage2 ���̵� ���� ���ÿ� ����
        StartCoroutine(FadeOutImage(CinemaImage1, 3f)); // 3�� ���� ���̵� �ƿ�

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FadeInImage(CinemaImage2, 5f)); // 7�� ���� ���̵� ��

        CinemaText.SetMsg("������ ���ο��ϴٰ� �ῡ �����");
        // ���̵� �� �Ϸ� �� 5�� ���� CinemaImage2�� ũ�⸦ 3��� Ȯ��
        yield return StartCoroutine(ScaleImage(CinemaImage2, 2.2f, 5f)); // 5�� ���� 3�� Ȯ��

        // Ȯ�밡 ���� �� 3�� ���� ���
        yield return new WaitForSeconds(3f);

        // Ȯ�밡 ���� �� 3�� ���� CinemaImage2�� ���������� ����
        StartCoroutine(FadeToBlackImage(CinemaImage2, 3f)); // �̹��� RGB ����
        yield return StartCoroutine(FadeToBlackText(CinemaText, 3f)); // �ؽ�Ʈ ���� ����

        // �ó׸�ƽ ���� ǥ��
        isCinemaFinished = true;
    }

    IEnumerator FadeOutImage(Image image, float duration)
    {
        float elapsedTime = 0f;
        Color color = image.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration); // ���� �� 1���� 0����
            image.color = color;
            yield return null;
        }

        color.a = 0f; // Ȯ���� 0���� ����
        image.color = color;
    }

    IEnumerator FadeInImage(Image image, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // ���� �� 0���� 1��
            Color color = image.color;
            color.a = Mathf.Lerp(0f, 1f, t);

            // RGB ���� ���� (255, 255, 255)�� ����
            color.r = Mathf.Lerp(image.color.r, 1f, t);
            color.g = Mathf.Lerp(image.color.g, 1f, t);
            color.b = Mathf.Lerp(image.color.b, 1f, t);

            image.color = color;
            yield return null;
        }

        // ���� ������ (255, 255, 255)�� ����
        Color finalColor = image.color;
        finalColor.a = 1f;
        finalColor.r = 1f;
        finalColor.g = 1f;
        finalColor.b = 1f;
        image.color = finalColor;
    }

    IEnumerator ScaleImage(Image image, float targetScale, float duration)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = image.rectTransform.localScale;
        Vector3 target = new Vector3(targetScale, targetScale, targetScale); // ��ǥ ũ�� (3��)

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.rectTransform.localScale = Vector3.Lerp(initialScale, target, elapsedTime / duration); // ũ�� ����
            yield return null;
        }

        image.rectTransform.localScale = target; // ���� ũ�� ����
    }
    IEnumerator FadeToBlackImage(Image image, float duration)
    {
        float elapsedTime = 0f;
        Color initialColor = image.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // RGB�� (255,255,255)���� (0,0,0)���� ����
            Color color = image.color;
            color.r = Mathf.Lerp(initialColor.r, 0f, t);
            color.g = Mathf.Lerp(initialColor.g, 0f, t);
            color.b = Mathf.Lerp(initialColor.b, 0f, t);
            image.color = color;

            yield return null;
        }

        // ���� ������ ������(0,0,0)���� ����
        Color finalColor = image.color;
        finalColor.r = 0f;
        finalColor.g = 0f;
        finalColor.b = 0f;
        image.color = finalColor;
    }
    IEnumerator FadeToBlackText(TypeEffect textEffect, float duration)
    {
        if (textEffect.MsgText == null)
        {
            Debug.LogError("TypeEffect.MsgText is null!");
            yield break;
        }

        TextMeshProUGUI text = textEffect.MsgText; // TypeEffect���� TextMeshProUGUI ��������
        float elapsedTime = 0f;
        Color initialColor = text.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // RGB�� �ʱⰪ���� (0,0,0)���� ����
            Color color = text.color;
            color.r = Mathf.Lerp(initialColor.r, 0f, t);
            color.g = Mathf.Lerp(initialColor.g, 0f, t);
            color.b = Mathf.Lerp(initialColor.b, 0f, t);
            text.color = color;

            yield return null;
        }

        // ���� ������ ������(0,0,0)���� ����
        Color finalColor = text.color;
        finalColor.r = 0f;
        finalColor.g = 0f;
        finalColor.b = 0f;
        text.color = finalColor;
    }
}
