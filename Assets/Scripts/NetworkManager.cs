using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject RoomPanel;
    public Text MyNick;
    public Text OtherNick;
    public Text StartError;
    public SpriteRenderer MaleSel;
    public SpriteRenderer FemaleSel;

    private bool isMaleSelected = false;
    private bool isFemaleSelected = false;
    private bool isOtherMaleSelected = false;
    private bool isOtherFemaleSelected = false;
    private bool isCharSelectionLocked = false;
    private bool isMyCharSelected = false;
    private bool isOtherCharSelected = false;
    public bool isGameStart = false;

    [SerializeField]
    private RuntimeAnimatorController[] animatorControllers;

    void Awake()
    {
        // Remove DontDestroyOnLoad if not necessary for the NetworkManager.
        if (FindObjectsOfType<NetworkManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }

        RoomPanel.SetActive(false);
        SetInitialCharacterColors();
    }


    void OnDestroy()
    {
        if (PhotonNetwork.IsConnected)
        {
            // Properly clean up Photon objects when NetworkManager is destroyed.
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        }
    }


    void Start()
    {
        string savedNickName = PlayerPrefs.GetString("USER_ID", "Unknown");
        PhotonNetwork.LocalPlayer.NickName = savedNickName;
        MyNick.text = savedNickName;
        MyNick.color = Color.green;

        RoomPanel.SetActive(true);
        UpdateOtherPlayerNick();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            photonView.RPC("RPC_StartScene1", RpcTarget.All);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            photonView.RPC("RPC_StartScene2", RpcTarget.All);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            photonView.RPC("RPC_StartScene3", RpcTarget.All);
        }
    }

    public bool GetIsGameStart()
    {
        return isGameStart;
    }

    void SetInitialCharacterColors()
    {
        SetCharacterColor(MaleSel, 0.4f);
        SetCharacterColor(FemaleSel, 0.4f);
    }

    public void GameStart()
    {
        if (!isMyCharSelected || !isOtherCharSelected)
        {
            StartError.text = "두 명의 플레이어가 모두 캐릭터를 선택해야 합니다.";
            Invoke("ClearText", 3f);
            return;
        }

        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            StartError.text = "2명의 플레이어가 준비가 돼야 합니다.";
            Invoke("ClearText", 3f);
            return;
        }

        isGameStart = true;
        photonView.RPC("RPC_StartGame", RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartGame()
    {
        Debug.Log("Game Start Triggered");
    }

    public void StartScene1()
    {
        photonView.RPC("RPC_StartScene1", RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartScene1()
    {
        PhotonNetwork.LoadLevel("Scene1");
    }
    public void StartScene2()
    {
        photonView.RPC("RPC_StartScene2", RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartScene2()
    {
        // PhotonNetwork를 통해 씬 전환
        PhotonNetwork.LoadLevel("Scene2");

        // NetworkManager를 수동으로 유지
        DontDestroyOnLoad(this.gameObject);
    }
    public void StartScene3()
    {
        photonView.RPC("RPC_StartScene3", RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartScene3()
    {
        PhotonNetwork.LoadLevel("Scene3-1");
        DontDestroyOnLoad(this.gameObject);
    }
    public void Spawn()
    {
        string localTag = isMaleSelected ? "player1" : "player2";

        // PhotonNetwork.Instantiate는 네트워크에 객체를 자동으로 동기화
        GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        PhotonView playerPhotonView = player.GetComponent<PhotonView>();

        // 객체가 중복 생성되는 문제를 방지하기 위해 추가적인 처리가 필요 없다
        // PhotonNetwork.Instantiate가 객체를 네트워크에 고유하게 관리

        // Animator와 태그 설정
        Animator playerAnimator = player.GetComponent<Animator>();
        player.tag = localTag;
        playerAnimator.runtimeAnimatorController = isMaleSelected ? animatorControllers[0] : animatorControllers[1];

        // 원격 플레이어에게 캐릭터 정보를 전송
        photonView.RPC("SetRemotePlayerAppearance", RpcTarget.OthersBuffered, playerPhotonView.ViewID, isMaleSelected);
    }



    [PunRPC]
    void SetRemotePlayerAppearance(int viewID, bool isRemoteMale)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            string remoteTag = isRemoteMale ? "player1" : "player2";
            targetView.gameObject.tag = remoteTag;

            Animator playerAnimator = targetView.gameObject.GetComponent<Animator>();
            playerAnimator.runtimeAnimatorController = isRemoteMale ? animatorControllers[0] : animatorControllers[1];
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player entered: {newPlayer.NickName}");

        // 새로운 플레이어에게 현재 클라이언트의 닉네임을 알려줌
        photonView.RPC("RPC_UpdateOtherPlayerNick", newPlayer, PhotonNetwork.LocalPlayer.NickName);

        // 새 플레이어의 닉네임 업데이트
        UpdateOtherPlayerNick();
    }

    [PunRPC]
    void RPC_UpdateOtherPlayerNick(string otherPlayerNick)
    {
        OtherNick.text = otherPlayerNick;
        OtherNick.color = Color.red;
    }

    public void SelectChar(string character)
    {
        if (isGameStart || isCharSelectionLocked)
        {
            StartError.text = "캐릭터를 이미 선택했습니다.";

            Invoke("ClearText", 3f);
            return;
        }

        if (character == "male")
        {
            if (isMaleSelected || isOtherMaleSelected)
            {
                StartError.text = "???? ?????? ????????????.";
                Invoke("ClearText", 3f);
                return;
            }

            isMyCharSelected = true;
            isMaleSelected = true;
            isCharSelectionLocked = true;

            SetCharacterColor(MaleSel, 1f);
            SetCharacterColor(FemaleSel, 0.4f);

            photonView.RPC("RPC_SelectChar", RpcTarget.OthersBuffered, "male");
        }
        else if (character == "female")
        {
            if (isFemaleSelected || isOtherFemaleSelected)
            {
                StartError.text = "???? ?????? ????????????.";
                Invoke("ClearText", 3f);
                return;
            }

            isMyCharSelected = true;
            isFemaleSelected = true;
            isCharSelectionLocked = true;

            SetCharacterColor(FemaleSel, 1f);
            SetCharacterColor(MaleSel, 0.4f);

            photonView.RPC("RPC_SelectChar", RpcTarget.OthersBuffered, "female");
        }

        CheckIfBothSelected();
    }

    [PunRPC]
    void RPC_SelectChar(string character)
    {
        if (character == "male")
        {
            isOtherMaleSelected = true;
            SetCharacterColor(MaleSel, 1f);
            SetCharacterColor(FemaleSel, 0.4f);
        }
        else if (character == "female")
        {
            isOtherFemaleSelected = true;
            SetCharacterColor(FemaleSel, 1f);
            SetCharacterColor(MaleSel, 0.4f);
        }

        isOtherCharSelected = true;
        CheckIfBothSelected();
    }

    void CheckIfBothSelected()
    {
        if (isMyCharSelected && isOtherCharSelected)
        {
            StartError.text = "캐릭터 선택 완료! 게임을 시작하세요.";

            SetCharacterColor(MaleSel, 1f);
            SetCharacterColor(FemaleSel, 1f);
        }
    }

    void UpdateOtherPlayerNick()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player != PhotonNetwork.LocalPlayer)
                {
                    OtherNick.text = player.NickName;
                    OtherNick.color = Color.red;
                    return;
                }
            }
        }
        else
        {
            OtherNick.text = "Waiting for other player...";
        }
    }

    void SetCharacterColor(SpriteRenderer sprite, float alpha)
    {
        Color color = sprite.color;
        color.a = alpha;
        sprite.color = color;
    }

    void ClearText()
    {
        StartError.text = "";
    }

    void ResetCharacterSelection()
    {
        isMyCharSelected = false;
        isOtherCharSelected = false;
        isMaleSelected = false;
        isFemaleSelected = false;
        isCharSelectionLocked = false;

        SetInitialCharacterColors();
    }
}