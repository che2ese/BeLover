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
        // Photon App ID ���� �� ���� ����
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "47ced4ee-73fb-42d6-b169-d52aae1d7a91";
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        PhotonNetwork.ConnectUsingSettings();

        // ȭ�� �ػ� �� ��Ʈ��ũ ����
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        // �ʱ� UI ����
        RoomPanel.SetActive(false);
    }

    public void Connect()
    {
        // �г����� �Էµ��� �ʾҴٸ� ��� ǥ��
        if (string.IsNullOrEmpty(NickNameInput.text))
        {
            Debug.LogWarning("�г����� �Է��ϼ���.");
            NickError.text = "�г����� �Է��ϼ���.";
            Invoke("ClearText", 3f);
            return;
        }

        // �г��� ���� �� UI ������Ʈ
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
        // �濡 �� ���� �÷��̾ ��� �غ�Ǿ����� Ȯ��
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.LogWarning("2���� �÷��̾ �غ� �ž� �մϴ�.");
            if (StartError != null)
            {
                StartError.text = "2���� �÷��̾ �غ� �ž� �մϴ�.";
                Invoke("ClearText", 3f);
            }
            return;
        }
        RoomPanel.SetActive(false);
        isGameStart = true;
    }

    public override void OnConnectedToMaster()
    {
        // ������ ������ ����� �� �κ� ����
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        // �κ� ������ �� ���� �����ϰų� ����
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
        // �濡 ���������� ������ ���
        Debug.Log("Successfully joined the room.");
        UpdateOtherPlayerNick();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // ���ο� �÷��̾ �濡 ������ ��
        Debug.Log("Player entered: " + newPlayer.NickName);
        UpdateOtherPlayerNick();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // �÷��̾ �濡�� ������ ��
        Debug.Log("Player left: " + otherPlayer.NickName);
        UpdateOtherPlayerNick();
    }

    void Update()
    {
        // ESC Ű�� ������ �� Photon ���� ����
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        UpdateOtherPlayerNick();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // Photon ������ �������� �� UI ������Ʈ
        DisconnectPanel.SetActive(true);
        Debug.LogWarning($"Disconnected from Photon Server: {cause}");
    }

    void ClearText()
    {
        // ��� �޽��� �ʱ�ȭ
        if (NickError != null)
            NickError.text = "";

        if (StartError != null)
            StartError.text = "";
    }

    void UpdateOtherPlayerNick()
    {
        // �濡 �ٸ� �÷��̾ �ִٸ� �г��� ������Ʈ
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
