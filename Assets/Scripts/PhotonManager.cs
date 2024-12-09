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
    private string userId = "Victor";

    public TMP_InputField userInputField;
    public TMP_InputField roomInputField;

    public Button loginButton;
    public Button createRoomButton;
    public Button joinRoomButton; // **JoinRoom ��ư �߰�**

    Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    GameObject roomItemPrefab;
    public Transform scrollContent;

    private bool isLoggedIn = false; // **�α��� ���� Ȯ�� ���� �߰�**

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = userId;
        roomItemPrefab = Resources.Load<GameObject>("RoomItem");

        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
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
            Debug.Log($"�÷��̾� �г���: {player.Value.NickName}, ���� ������: {player.Value.ActorNumber}");
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MenuScene");
        }
    }

    private void Start()
    {
        userId = PlayerPrefs.GetString("����� ID", $"USER_{Random.Range(1, 21):00}");
        userInputField.text = userId;
        PhotonNetwork.NickName = userId;

        DisableAllButtons(); // **��� ��ư ��Ȱ��ȭ**
    }

    public void SetUserId()
    {
        if (string.IsNullOrEmpty(userInputField.text))
        {
            userId = $"����� {Random.Range(1, 21):00}";
        }
        else
        {
            userId = userInputField.text;
        }

        PlayerPrefs.SetString("�����", userId);
        PhotonNetwork.NickName = userId;
    }

    string SetRoomName()
    {
        if (string.IsNullOrEmpty(roomInputField.text))
        {
            roomInputField.text = $"{Random.Range(1, 101):000}�� ��";
        }
        return roomInputField.text;
    }

    public void OnLoginClick()
    {
        SetUserId();
        DisableLoginButton();
        EnableAllButtons(); // **��� ��ư Ȱ��ȭ**
        userInputField.interactable = false;
        isLoggedIn = true; // **�α��� ���¸� true�� ����**
    }

    private void DisableLoginButton()
    {
        loginButton.interactable = false;
        Image buttonImage = loginButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = Color.gray;
        }
    }

    private void EnableAllButtons()
    {
        EnableButton(createRoomButton);
        EnableButton(joinRoomButton);
        EnableRoomListButtons(); // **RoomList ��ư Ȱ��ȭ**
    }

    private void DisableAllButtons()
    {
        DisableButton(createRoomButton);
        DisableButton(joinRoomButton);
        DisableRoomListButtons(); // **RoomList ��ư ��Ȱ��ȭ**
    }

    private void EnableButton(Button button)
    {
        button.interactable = true;
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = Color.white;
        }
    }

    private void DisableButton(Button button)
    {
        button.interactable = false;
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = Color.gray;
        }
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
            Debug.LogWarning("�α��� �Ŀ� �濡 ������ �� �ֽ��ϴ�.");
            return;
        }

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        if (!isLoggedIn)
        {
            Debug.LogWarning("�α��� �Ŀ� ���� ������ �� �ֽ��ϴ�.");
            return;
        }

        SetUserId();
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
                if (rooms.ContainsKey(roomInfo.Name) == false)
                {
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;
                    Button button = roomPrefab.GetComponent<Button>();
                    if (button != null)
                    {
                        button.interactable = isLoggedIn; // **�α��� ���¿� ���� ��ư Ȱ��ȭ**
                    }
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
            }
        }

        if (isLoggedIn)
        {
            EnableRoomListButtons(); // **�α��� ������ �� �� ����Ʈ ��ư Ȱ��ȭ**
        }
    }
}
