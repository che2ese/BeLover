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

    [SerializeField]
    private byte maxPlayers = 2;

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
    }

    public void Connect()
    {
        // 닉네임이 입력되지 않았다면 경고 표시
        if (string.IsNullOrEmpty(NickNameInput.text))
        {
            Debug.LogWarning("닉네임을 입력하세요.");
            NickError.text = "닉네임을 입력하세요.";
            Invoke("ClearText", 3f);
            return;
        }

        // 닉네임 설정 및 UI 업데이트
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
        // 방에 두 명의 플레이어가 모두 준비되었는지 확인
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
        RoomPanel.SetActive(false);
        isGameStart = true;
    }

    public override void OnConnectedToMaster()
    {
        // 마스터 서버에 연결된 후 로비에 참가
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        // 로비에 참가한 후 방을 생성하거나 참가
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
        // 방에 성공적으로 참가한 경우
        Debug.Log("Successfully joined the room.");
        UpdateOtherPlayerNick();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 새로운 플레이어가 방에 들어왔을 때
        Debug.Log("Player entered: " + newPlayer.NickName);
        UpdateOtherPlayerNick();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 플레이어가 방에서 나갔을 때
        Debug.Log("Player left: " + otherPlayer.NickName);
        UpdateOtherPlayerNick();
    }

    void Update()
    {
        // ESC 키를 눌렀을 때 Photon 연결 해제
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        UpdateOtherPlayerNick();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // Photon 연결이 끊어졌을 때 UI 업데이트
        DisconnectPanel.SetActive(true);
        Debug.LogWarning($"Disconnected from Photon Server: {cause}");
    }

    void ClearText()
    {
        // 경고 메시지 초기화
        if (NickError != null)
            NickError.text = "";

        if (StartError != null)
            StartError.text = "";
    }

    void UpdateOtherPlayerNick()
    {
        // 방에 다른 플레이어가 있다면 닉네임 업데이트
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
}
