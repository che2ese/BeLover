using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class TalkManager : MonoBehaviour
{
    MainSceneManager Msm;

    public GameObject dialoguePanel;

    public TypeEffect CinemaP1;
    public TypeEffect CinemaP2;

    public bool isDialogueFinished = false;

    private string[] P1Text = {
        "�� ���Ⱑ �����?",
        "��������?",
        "��? ���� �� ���̴µ���?",
        "�� �𸣰ڴµ� �������� ���ƿ�...\n\n�׸��� ���� �κ��� Ÿ�ִµ� ������ �ȵſ�...",
        "��? ������ ���� �־��"
    };

    private string[] P2Text = {
        "�ű� ���� �־��?",
        "���� �� �𸣰ھ��.\n\n�ƹ��͵� ����� ���� �ʾƿ�...\n\n�׸��� �� ���� �����ؼ� �������� �ʾƿ�...",
        "�׷� ���Ⱑ ����� �˾ƿ�?",
        "�� �տ� �����밡 �ִ°� ���� �ѵ� ���������Կ�",
        "���� ���� �� ���̴µ� ���� �������ֽ� �� �־��?"
    };

    private int P1Index = 0;
    private int P2Index = 0;

    private bool isP1Talking = true; // P1���� ����

    public string[] S2Text = {
        "�ܸ�Ʈ������ ���� �ſ��� ���� ���ϰ�,\n\n��Ʈ������ �޾� ���� ���� ����",
        "���ƾƾƾƾ� �ſ��� �ʹ� �Ⱦ� �� �ڻ쳻�����ž�!!!",
        "�μ��� �ſ� ������ ã�� �ſ��� �ϼ��غ���.\n\n�� �� �÷��̾ ������ �� �ִ� 2���� ����Ű�� �ٸ���,\n\n������ ���� ������ ����Ű�� �������� �ٲ��.",
        "��... �츮�� �ſ� ������ ��ƿԾ�.",
        "����! ���� ġ��!",
        "�ƴϾ�, �ſ��� ��. �װ� �Ʊ�ʹ� �ٸ��� ����.",
        "�׷�...? �Ȱ����� ������...",
        "���� ������ �����ڸ� ��.",
        "�׷�! �ſﺸ�� �� ��¦�Ÿ��°�? ���� ���ڴ�.",
        "���� �׷��� ������...?",
        "�׷�. �ʴ� � �� ����?",
        "����...�׷� �� ����...",
        "���� Ǯ���༭ ����, ���߿� �� �ʿ��� ������ �ٰ�.\n\n���� �Ծ��� �ſ� ���� 5���� �ſ� 5���� �����ٰ�."
    };
    public string[] S3Text =
    {
        "�ܸ�Ʈ������ ���� ������� �������ϰ�,\n\n��ȸ�� �и��Ǿ� ��Ȳ��ְ� �� ����",
        "���� �� ����...�ٽ� �������� ���ư� �� ����...",
        "�÷��̾���� ���� �ٸ� �������� ������ Ǯ��\n\n���ο��� ������ �ְ� Ż���غ���.\n\nó���� ������ �ٴ� Ÿ�Ͽ� �����ϴ� ��ư�� ���ڰ� ���� �ذ��ؾ��Ѵ�.",
        "���� �� ���� ���� �ھƿ� ��Ƹ����ž�...\n\n���� ������ ������ ���� �� ����.",
        "����...? �츮�� ���鼭 �ô� �ذ���� ���ϴ°ž�?",
        "�¾�...�и� ���� ������ �����ϰž�...",
        "�� ����غ�. ���� �ʰ� ������ ������ �ƴϾ�.",
        "�ʸ� ��Ƹ������� ���� �ھ��� ���̾�.",
        "�� ã�ƿ��� ���...?",
        "�츮�� ���� �ھư� ���� ���ϰ� �к��� �� ���Ҿ�.",
        "���� �� �ڽ��� �ȹٷ� �ٶ� �� �����ž�.",
        "����...���� �� �� ������ ����.",
        "���� Ǯ���༭ ����, ���߿� �� �ʿ��� ������ �ٰ�.\n\n���� ������ Ƚ����ŭ 3���� �� ���̸� �ٰ�."
    };
    private void Awake()
    {
        Msm = FindObjectOfType<MainSceneManager>();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // ��縦 �ٷ� �����Ϸ��� Start()���� ȣ���ϵ��� ����
        if (Msm != null && Msm.isCinemaFinished)
        {
            dialoguePanel.SetActive(true);
            StartCoroutine(ShowDialogue());
        }
    }

    // Update���� �ڷ�ƾ�� �Ź� ȣ������ �ʵ��� ����
    private void Update()
    {
        // dialoguePanel�� ��� �ѳ���, �ó׸�ƽ�� ������ ���� ��縦 ����
        if (Msm != null && Msm.isCinemaFinished && !dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(true);
            StartCoroutine(ShowDialogue());
        }
    }

    IEnumerator ShowDialogue()
    {
        // ��������� ��� �����ֱ�
        while (P1Index < P1Text.Length || P2Index < P2Text.Length)
        {
            if (isP1Talking && P1Index < P1Text.Length)
            {
                CinemaP1.SetMsg(P1Text[P1Index]);
                yield return new WaitForSeconds(P1Text[P1Index].Length / CinemaP1.CharPerSeconds); // ��� ���̸�ŭ ��ٸ�
                P1Index++;
                isP1Talking = false; // ������ P2�� ����
                yield return new WaitForSeconds(2f);
            }
            else if (!isP1Talking && P2Index < P2Text.Length)
            {
                CinemaP2.SetMsg(P2Text[P2Index]);
                yield return new WaitForSeconds(P2Text[P2Index].Length / CinemaP2.CharPerSeconds); // ��� ���̸�ŭ ��ٸ�
                P2Index++;
                isP1Talking = true; // ������ P1�� ����
                yield return new WaitForSeconds(2f);
            }
            yield return null;
        }
        isDialogueFinished = true;
        // ��簡 ��� ������ dialoguePanel�� ��Ȱ��ȭ
        dialoguePanel.SetActive(false);
    }

    public IEnumerator ImagePadeOut(Image image, Image image1)
    {
        RectTransform rectTransform = image.GetComponent<RectTransform>(); // RectTransform ��������
        CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>(); // Alpha ���� �����ϱ� ���� CanvasGroup ���
        if (canvasGroup == null)
        {
            canvasGroup = image.gameObject.AddComponent<CanvasGroup>(); // ������ �߰�
        }

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0); // �ʱ� Y ��ġ ����
        canvasGroup.alpha = 1f; // �ʱ� Alpha �� ����

        while (rectTransform.anchoredPosition.y < 1200)
        {
            rectTransform.anchoredPosition += new Vector2(0, 200) * Time.deltaTime; // Y ��ġ ����
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, new Vector3(0.2f, 0.2f, 0.2f), Time.deltaTime); // ũ�� ���� ���̱�
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime); // ���� ���� ����
            yield return null; // ���� �����ӱ��� ���
        }

        if (rectTransform.anchoredPosition.y > 1200)
        {
            image1.gameObject.SetActive(true);
            yield return new WaitForSeconds(5f);

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("MainScene");
            }
        }
    }
}
