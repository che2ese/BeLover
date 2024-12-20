using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
    MainSceneManager Msm;

    public GameObject dialoguePanel;

    public TypeEffect CinemaP1;
    public TypeEffect CinemaP2;

    public bool isDialogueFinished = false;

    private string[] P1Text = {
        "어 여기가 어디지?",
        "누구세요?",
        "엥? 저는 잘 보이는데요?",
        "잘 모르겠는데 낭떠러지 같아요...\n\n그리고 무슨 로봇에 타있는데 조종이 안돼요...",
        "어? 앞으로 가고 있어요"
    };

    private string[] P2Text = {
        "거기 누구 있어요?",
        "저도 잘 모르겠어요.\n\n아무것도 기억이 나질 않아요...\n\n그리고 눈 앞이 깜깜해서 보이지가 않아요...",
        "그럼 여기가 어딘지 알아요?",
        "제 앞에 운전대가 있는거 같긴 한데 움직여볼게요",
        "저는 앞이 안 보이는데 말로 설명해주실 수 있어요?"
    };

    private int P1Index = 0;
    private int P2Index = 0;

    private bool isP1Talking = true; // P1부터 시작

    private void Awake()
    {
        Msm = FindObjectOfType<MainSceneManager>();
    }

    private void Start()
    {
        // 대사를 바로 시작하려면 Start()에서 호출하도록 수정
        if (Msm.isCinemaFinished)
        {
            dialoguePanel.SetActive(true);
            StartCoroutine(ShowDialogue());
        }
    }

    // Update에서 코루틴을 매번 호출하지 않도록 수정
    private void Update()
    {
        // dialoguePanel은 계속 켜놓고, 시네마틱이 끝났을 때만 대사를 시작
        if (Msm.isCinemaFinished && !dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(true);
            StartCoroutine(ShowDialogue());
        }
    }

    IEnumerator ShowDialogue()
    {
        // 시퀀스대로 대사 보여주기
        while (P1Index < P1Text.Length || P2Index < P2Text.Length)
        {
            if (isP1Talking && P1Index < P1Text.Length)
            {
                CinemaP1.SetMsg(P1Text[P1Index]);
                yield return new WaitForSeconds(P1Text[P1Index].Length / CinemaP1.CharPerSeconds); // 대사 길이만큼 기다림
                P1Index++;
                isP1Talking = false; // 다음은 P2의 차례
                yield return new WaitForSeconds(2f);
            }
            else if (!isP1Talking && P2Index < P2Text.Length)
            {
                CinemaP2.SetMsg(P2Text[P2Index]);
                yield return new WaitForSeconds(P2Text[P2Index].Length / CinemaP2.CharPerSeconds); // 대사 길이만큼 기다림
                P2Index++;
                isP1Talking = true; // 다음은 P1의 차례
                yield return new WaitForSeconds(2f);
            }
            yield return null;
        }
        isDialogueFinished = true;
        // 대사가 모두 끝나면 dialoguePanel을 비활성화
        dialoguePanel.SetActive(false);
    }
}
