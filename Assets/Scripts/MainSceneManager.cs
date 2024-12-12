using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    GameManager gm;
    NetworkManager nm;
    public GameObject storyPanel;
    public Button StartBtn;

    public GameObject[] mainMission;

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        nm = FindObjectOfType<NetworkManager>();

        // ���� ����� �� �׻� storyPanel�� Ȱ��ȭ
        storyPanel.SetActive(true);

        // �ʿ��ϸ� PlayerPrefs �ʱ�ȭ
        PlayerPrefs.SetInt("StoryPanelHidden", 0);
    }
    void Update()
    {
        if(gm.mainSceneEnterCount >= 2)
        {
            storyPanel.SetActive(false);
            if(gm.mainSceneEnterCount == 2 && gm.isMission1Clear == true && gm.isMission2Clear == false && gm.isMission3Clear == false)
            {
                mainMission[0].tag = "MainMission";
            }
            else
                mainMission[0].tag = "Untagged";

            if (gm.mainSceneEnterCount == 3 && gm.isMission1Clear == true && gm.isMission2Clear == true && gm.isMission3Clear == false)
            {
                mainMission[1].tag = "MainMission";
            }
            else
                mainMission[1].tag = "Untagged";

            if(gm.mainSceneEnterCount == 4 && gm.isMission1Clear == true && gm.isMission2Clear == true && gm.isMission3Clear == true)
            {
                mainMission[2].tag = "MainMission";
            }
            else
                mainMission[2].tag = "Untagged";
        }
    }
    public void SpawnChar()
    {
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
}
