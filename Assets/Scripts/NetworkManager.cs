using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NickNameInput;
    public GameObject DisconnectPanel;
    public GameObject RoomPanel;
    public Text MyNick;
    public Text OtherNick;
    public Text NickError;
    public Text StartError;
    public bool isGameStart = false;
    public bool isConnect = false;
<<<<<<< HEAD
    public SpriteRenderer MaleSel;
    public SpriteRenderer FemaleSel;
    private bool isMaleSelected = false;
    private bool isFemaleSelected = false;

    private bool isOtherMaleSelected = false;
    private bool isOtherFemaleSelected = false;
    private bool isCharSelectionLocked = false; // 캐릭터 선택 잠금

    Animator anim;
    [SerializeField]
    private RuntimeAnimatorController[] animatorControllers; // 애니메이션 컨트롤러 배열
=======
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41

    [SerializeField]
    private byte maxPlayers = 2;

<<<<<<< HEAD
    private bool isMyCharSelected = false;
    private bool isOtherCharSelected = false;

    void Awake()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        PhotonNetwork.ConnectUsingSettings();
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        DontDestroyOnLoad(this.gameObject);
        anim = GetComponent<Animator>();
        RoomPanel.SetActive(false);
        SetInitialCharacterColors();
    }

    void SetInitialCharacterColors()
    {
        Color maleColor = MaleSel.color;
        maleColor.a = 100 / 255f;
        MaleSel.color = maleColor;

        Color femaleColor = FemaleSel.color;
        femaleColor.a = 100 / 255f;
        FemaleSel.color = femaleColor;
=======
    void Awake()
    {
        // Photon App ID 설정 및 서버 연결
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "47ced4ee-73fb-42d6-b169-d52aae1d7a91";
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        PhotonNetwork.ConnectUsingSettings();

        // 화면 해상도 및 네트워크 설정
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        // 초기 UI 설정
        RoomPanel.SetActive(false);
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
    }

    public void Connect()
    {
<<<<<<< HEAD
=======
        // 닉네임이 입력되지 않았다면 경고 표시
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
        if (string.IsNullOrEmpty(NickNameInput.text))
        {
            Debug.LogWarning("닉네임을 입력하세요.");
            NickError.text = "닉네임을 입력하세요.";
            Invoke("ClearText", 3f);
            return;
        }

<<<<<<< HEAD
        // 닉네임 중복 확인
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == NickNameInput.text)
            {
                Debug.LogWarning("이미 사용 중인 닉네임입니다.");
                NickError.text = "이미 사용 중인 닉네임입니다.";
                Invoke("ClearText", 3f);
                return;
            }
        }

=======
        // 닉네임 설정 및 UI 업데이트
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        DisconnectPanel.SetActive(false);
        RoomPanel.SetActive(true);

        MyNick.text = PhotonNetwork.LocalPlayer.NickName;
        MyNick.color = Color.green;
        OtherNick.color = Color.red;

        UpdateOtherPlayerNick();
    }

    public void GameStart()
    {
<<<<<<< HEAD
        if (!isMyCharSelected || !isOtherCharSelected)
        {
            Debug.LogWarning("두 명의 플레이어가 모두 캐릭터를 선택해야 합니다.");
            if (StartError != null)
            {
                StartError.text = "두 명의 플레이어가 모두 캐릭터를 선택해야 합니다.";
                Invoke("ClearText", 3f);
            }
            return;
        }

=======
        // 방에 두 명의 플레이어가 모두 준비되었는지 확인
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.LogWarning("2명의 플레이어가 준비가 돼야 합니다.");
            if (StartError != null)
            {
                StartError.text = "2명의 플레이어가 준비가 돼야 합니다.";
                Invoke("ClearText", 3f);
            }
            return;
        }
<<<<<<< HEAD

        // 모든 클라이언트에서 GameStart 상태를 동기화하고 씬 전환 유도
        photonView.RPC("RPC_StartGame", RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartGame()
    {
        isGameStart = true;
    }

    public void Spawn()
    {
        // 로컬 플레이어가 선택한 캐릭터에 따라 태그와 애니메이션 설정
        string localTag = isMaleSelected ? "player1" : "player2";

        // 플레이어를 생성합니다.
        GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        Animator playerAnimator = player.GetComponent<Animator>();

        // 로컬 플레이어 설정
        player.tag = localTag;
        playerAnimator.runtimeAnimatorController = isMaleSelected ? animatorControllers[0] : animatorControllers[1];

        // 상대방에게 로컬 플레이어의 선택 정보를 전송
        photonView.RPC("SetRemotePlayerAppearance", RpcTarget.OthersBuffered, player.GetComponent<PhotonView>().ViewID, isMaleSelected);
    }

    [PunRPC]
    void SetRemotePlayerAppearance(int viewID, bool isRemoteMale)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            // 상대방이 선택한 캐릭터를 그대로 반영
            string remoteTag = isRemoteMale ? "player1" : "player2";
            targetView.gameObject.tag = remoteTag;

            Animator playerAnimator = targetView.gameObject.GetComponent<Animator>();
            playerAnimator.runtimeAnimatorController = isRemoteMale ? animatorControllers[0] : animatorControllers[1];
        }
    }



    public override void OnConnectedToMaster()
    {
=======
        RoomPanel.SetActive(false);
        isGameStart = true;
    }

    public override void OnConnectedToMaster()
    {
        // 마스터 서버에 연결된 후 로비에 참가
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
<<<<<<< HEAD
=======
        // 로비에 참가한 후 방을 생성하거나 참가
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
        Debug.Log("Joined lobby, now creating or joining a room...");
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.JoinOrCreateRoom("Room", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join or create room: {message}");
    }

    public override void OnJoinedRoom()
    {
<<<<<<< HEAD
=======
        // 방에 성공적으로 참가한 경우
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
        Debug.Log("Successfully joined the room.");
        UpdateOtherPlayerNick();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
<<<<<<< HEAD
=======
        // 새로운 플레이어가 방에 들어왔을 때
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
        Debug.Log("Player entered: " + newPlayer.NickName);
        UpdateOtherPlayerNick();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
<<<<<<< HEAD
        Debug.Log("Player left: " + otherPlayer.NickName);
        UpdateOtherPlayerNick();
        ResetCharacterSelection();
=======
        // 플레이어가 방에서 나갔을 때
        Debug.Log("Player left: " + otherPlayer.NickName);
        UpdateOtherPlayerNick();
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
    }

    void Update()
    {
<<<<<<< HEAD
=======
        // ESC 키를 눌렀을 때 Photon 연결 해제
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        UpdateOtherPlayerNick();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
<<<<<<< HEAD
=======
        // Photon 연결이 끊어졌을 때 UI 업데이트
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
        DisconnectPanel.SetActive(true);
        Debug.LogWarning($"Disconnected from Photon Server: {cause}");
    }

    void ClearText()
    {
<<<<<<< HEAD
=======
        // 경고 메시지 초기화
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
        if (NickError != null)
            NickError.text = "";

        if (StartError != null)
            StartError.text = "";
    }

    void UpdateOtherPlayerNick()
    {
<<<<<<< HEAD
=======
        // 방에 다른 플레이어가 있다면 닉네임 업데이트
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player != PhotonNetwork.LocalPlayer)
                {
                    OtherNick.text = player.NickName;
                    return;
                }
            }
        }
        else
        {
            OtherNick.text = "Waiting for other player...";
        }
    }
<<<<<<< HEAD

    public void SelectChar(string character)
    {
        if (MaleSel == null || FemaleSel == null)
        {
            Debug.LogError("MaleSel 또는 FemaleSel이 null입니다. Unity Editor에서 이 필드들이 제대로 연결되었는지 확인하세요.");
            return;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount < maxPlayers)
        {
            StartError.text = "두 명의 플레이어가 모두 입장해야\n캐릭터를 선택할 수 있습니다.";
            Invoke("ClearText", 3f);
            return;
        }

        if (isGameStart || isCharSelectionLocked)
        {
            StartError.text = "당신은 이미 선택하였습니다.";
            Invoke("ClearText", 3f);
            return;
        }

        // 현재 캐릭터 선택 상태에 따라 처리
        if (character == "male")
        {
            // 이미 로컬 또는 원격에서 남성 캐릭터가 선택된 경우
            if (isMaleSelected || isOtherMaleSelected)
            {
                StartError.text = "이미 선택된 캐릭터입니다.";
                Invoke("ClearText", 3f);
                return;
            }

            isMyCharSelected = true;
            isMaleSelected = true;
            isFemaleSelected = false;
            isCharSelectionLocked = true;

            // 남성 캐릭터 선택
            Color maleColor = MaleSel.color;
            maleColor.a = 1f;
            MaleSel.color = maleColor;

            Color femaleColor = FemaleSel.color;
            femaleColor.a = 0.4f;
            FemaleSel.color = femaleColor;

            PhotonView.Get(this).RPC("RPC_SelectChar", RpcTarget.OthersBuffered, "male");
        }
        else if (character == "female")
        {
            // 이미 로컬 또는 원격에서 여성 캐릭터가 선택된 경우
            if (isFemaleSelected || isOtherFemaleSelected)
            {
                StartError.text = "이미 선택된 캐릭터입니다.";
                Invoke("ClearText", 3f);
                return;
            }

            isMyCharSelected = true;
            isFemaleSelected = true;
            isMaleSelected = false;
            isCharSelectionLocked = true;

            // 여성 캐릭터 선택
            Color femaleColor = FemaleSel.color;
            femaleColor.a = 1f;
            FemaleSel.color = femaleColor;

            Color maleColor = MaleSel.color;
            maleColor.a = 0.4f;
            MaleSel.color = maleColor;

            PhotonView.Get(this).RPC("RPC_SelectChar", RpcTarget.OthersBuffered, "female");
        }

        CheckIfBothSelected();
    }

    [PunRPC]
    void RPC_SelectChar(string character)
    {
        if (character == "male")
        {
            // 이미 원격에서 남성 캐릭터가 선택된 경우
            if (isOtherMaleSelected || isMaleSelected)
            {
                StartError.text = "이미 선택된 캐릭터입니다.";
                Invoke("ClearText", 3f);
                return;
            }

            isOtherMaleSelected = true;
            isOtherFemaleSelected = false;
            isOtherCharSelected = true; // 상대방 캐릭터가 선택되었음을 표시

            Color maleColor = MaleSel.color;
            maleColor.a = 1f;
            MaleSel.color = maleColor;

            Color femaleColor = FemaleSel.color;
            femaleColor.a = 0.4f;
            FemaleSel.color = femaleColor;
        }
        else if (character == "female")
        {
            // 이미 원격에서 여성 캐릭터가 선택된 경우
            if (isOtherFemaleSelected || isFemaleSelected)
            {
                StartError.text = "이미 선택된 캐릭터입니다.";
                Invoke("ClearText", 3f);
                return;
            }

            isOtherFemaleSelected = true;
            isOtherMaleSelected = false;
            isOtherCharSelected = true; // 상대방 캐릭터가 선택되었음을 표시

            Color femaleColor = FemaleSel.color;
            femaleColor.a = 1f;
            FemaleSel.color = femaleColor;

            Color maleColor = MaleSel.color;
            maleColor.a = 0.4f;
            MaleSel.color = maleColor;
        }

        isCharSelectionLocked = false; // 선택 완료 후 잠금 해제

        CheckIfBothSelected();
    }


    void CheckIfBothSelected()
    {
        if (isMyCharSelected && isOtherCharSelected)
        {
            StartError.text = "모두 캐릭터를 선택했습니다. 게임을 시작할 수 있습니다.";

            // 두 캐릭터 모두 불투명하게 설정
            Color maleColor = MaleSel.color;
            maleColor.a = 1f;
            MaleSel.color = maleColor;

            Color femaleColor = FemaleSel.color;
            femaleColor.a = 1f;
            FemaleSel.color = femaleColor;
        }
    }


    void ResetCharacterSelection()
    {
        isMyCharSelected = false;
        isOtherCharSelected = false;
        isMaleSelected = false;
        isFemaleSelected = false;
        isCharSelectionLocked = false; // 선택 상태 초기화

        SetInitialCharacterColors();
    }
=======
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
}
