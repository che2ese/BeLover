using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0";
    private string userId;

    public TMP_InputField userInputField;
    public TMP_InputField roomInputField;

    public Button loginButton;
    public Button createRoomButton;
    public Button joinRoomButton; // **JoinRoom 버튼 추가**

    Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    GameObject roomItemPrefab;
    public Transform scrollContent;

    private bool isLoggedIn = false; // **로그인 상태 확인 변수 추가**

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;
        roomItemPrefab = Resources.Load<GameObject>("RoomItem");

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");

        // **마스터 서버에 연결 후 닉네임 설정**
        SetUserId();
        PhotonNetwork.NickName = userId;

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"In Lobby = {PhotonNetwork.InLobby}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}: {message}");
        OnMakeRoomClick();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name: {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"In Room = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"플레이어 닉네임: {player.Value.NickName}, 유저 고유값: {player.Value.ActorNumber}");
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MenuScene");
        }
    }

    private void Start()
    {
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1, 21):00}");
        userInputField.text = userId;

        DisableAllButtons(); // **모든 버튼 비활성화**
    }

    public void SetUserId()
    {
        if (string.IsNullOrEmpty(userInputField.text))
        {
            userId = $"USER_{Random.Range(1, 21):00}";
        }
        else
        {
            userId = userInputField.text;
        }

        PlayerPrefs.SetString("USER_ID", userId);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.NickName = userId; // **닉네임 설정**
        }
    }

    string SetRoomName()
    {
        if (string.IsNullOrEmpty(roomInputField.text))
        {
            roomInputField.text = $"{Random.Range(1, 101):000}번 방";
        }
        return roomInputField.text;
    }

    public void OnLoginClick()
    {
        SetUserId();
        DisableLoginButton();
        EnableAllButtons();
        userInputField.interactable = false;
        isLoggedIn = true;
    }

    private void DisableLoginButton()
    {
        loginButton.interactable = false;
    }

    private void EnableAllButtons()
    {
        EnableButton(createRoomButton);
        EnableButton(joinRoomButton);
        EnableRoomListButtons();
    }

    private void DisableAllButtons()
    {
        DisableButton(createRoomButton);
        DisableButton(joinRoomButton);
        DisableRoomListButtons();
    }

    private void EnableButton(Button button)
    {
        button.interactable = true;
    }

    private void DisableButton(Button button)
    {
        button.interactable = false;
    }

    private void DisableRoomListButtons()
    {
        foreach (var room in rooms.Values)
        {
            Button button = room.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = false;
            }
        }
    }

    private void EnableRoomListButtons()
    {
        foreach (var room in rooms.Values)
        {
            Button button = room.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = true;
            }
        }
    }

    public void JoinRoom()
    {
        if (!isLoggedIn)
        {
            Debug.LogWarning("로그인 후에 방에 입장할 수 있습니다.");
            return;
        }

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        if (!isLoggedIn)
        {
            Debug.LogWarning("로그인 후에 방을 생성할 수 있습니다.");
            return;
        }

        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 2;
        ro.IsOpen = true;
        ro.IsVisible = true;
        PhotonNetwork.CreateRoom(SetRoomName(), ro);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;
        foreach (var roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList == true)
            {
                rooms.TryGetValue(roomInfo.Name, out tempRoom);
                Destroy(tempRoom);
                rooms.Remove(roomInfo.Name);
            }
            else
            {
                if (!rooms.ContainsKey(roomInfo.Name))
                {
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;
                    Button button = roomPrefab.GetComponent<Button>();
                    if (button != null)
                    {
                        button.interactable = isLoggedIn;
                    }
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
            }
        }

        if (isLoggedIn)
        {
            EnableRoomListButtons();
        }
    }
}
