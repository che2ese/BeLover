using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

//MonoBehaviour ����.
//Photon ���� ��� �÷��̾ �����̱� ���ٴ� ������ �����ϴ� �����̶� �ٸ� Ŭ������ ��ӹ޴´�

public class PhotonManager : MonoBehaviourPunCallbacks //��� Ŭ���� ����
{
    //���� ����
    private readonly string version = "1.0";//���� ���� üũ. ������ �ǵ帮�� ���ϰ� private, readonly
    private string userId = "Victor"; //�ƹ��ų� userId ����

    //���� ID�� �Է��� ��ǲ �ʵ�
    public TMP_InputField userInputField;
    //�� ID�� �Է��� ��ǲ �ʵ�
    public TMP_InputField roomInputField;

    //�α��� ��ư
    public Button loginButton;

    //�� ��Ͽ� ���� ������ ����
    Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    //�� ����� ǥ���� ������
    GameObject roomItemPrefab;
    //�� ����� ǥ�õ� scroll content
    public Transform scrollContent;

    //��Ʈ��ũ ������ Start()���� ���� ����Ǿ���Ѵ�. Awake() �Լ� ���
    private void Awake()
    {
        //�� ����ȭ. �� ó�� ������ ����� ������ �ȴ�.
        PhotonNetwork.AutomaticallySyncScene = true;
        //���� �Ҵ�. ���� string���� ������� version�� ����.
        PhotonNetwork.GameVersion = version;
        //App ID �Ҵ�. ���� userId�� ������� userId�� ����.
        PhotonNetwork.NickName = userId;
        //���� �������� ��� Ƚ���� �α׷� ���. �⺻�� : 30
        Debug.Log(PhotonNetwork.SendRate); //����� ����� �Ǿ��ٸ� 30�� ��µȴ�.

        //RoomItem ������ �ε� Resources �����κ���...
        roomItemPrefab = Resources.Load<GameObject>("RoomItem");

        //���� ������ ����
        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    //CallBack �Լ�
    public override void OnConnectedToMaster() //���������� ������ ������ ������ �Ǹ� ȣ��ȴ�.
    {
        //������ ������ ������ �Ǿ����� ����� �Ѵ�.
        Debug.Log("Connected to Master");
        Debug.Log($"In Lobby = {PhotonNetwork.InLobby}"); //�κ� ���� ������ True, �ƴϸ� False ��ȯ. Master �������� ���������� �κ񿡴� �ƴϹǷ� False ��ȯ�ȴ�.
        //�κ� ����
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() //�κ� ������ ����� �Ǿ��ٸ� �ش� �ݹ��Լ� ȣ��
    {
        Debug.Log($"In Lobby = {PhotonNetwork.InLobby}"); //�κ� ������ �Ǿ��ٸ� True�� ��ȯ �� ���̴�.
        //�� ���� ����� �� ����. 1.���� ��ġ����ŷ, 2.���õ� �� ����
        //PhotonNetwork.JoinRandomRoom();
    }

    //�� ������ ���� �ʾ����� ���� �ݹ� �Լ� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}: {message}");

        OnMakeRoomClick(); //���� ���� ���� �����ϱ� ���ؼ�.

        //�� �Ӽ� ����
        //RoomOptions roomOptions = new RoomOptions();
        //���� ������ �� �ִ� �ִ� ������ �� �ִ� ������ �س��� CCU�� ������ �� �ִ�.
        //roomOptions.MaxPlayers = 20;
        //�� ���� ����
        //roomOptions.IsOpen = true;
        //�κ񿡼� ���� ��Ͽ� �����ų�� ����. ������ ����
        //roomOptions.IsVisible = true;
        //�� ����
        //PhotonNetwork.CreateRoom("Room1", roomOptions); //�� �̸��� �� ����. �츮�� roomOptions�� ������ �̹� �س��Ҵ�.
    }

    //����� ���� �ִٸ� ������ �ݹ� �Լ��� ȣ���Ѵ�.
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name: {PhotonNetwork.CurrentRoom.Name}");
    }

    //�뿡 ������ �� �ݹ� �Լ�
    public override void OnJoinedRoom()
    {
        Debug.Log($"In Room = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        //������ ����� �г��� Ȯ��
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            //�÷��̾� �г���, ������ ������ ��������
            Debug.Log($"�÷��̾� �г���: {player.Value.NickName}, ���� ������: {player.Value.ActorNumber}");
        }

        //�÷��̾� ���� ����Ʈ �׷� �迭�� �޾ƿ���. ����Ʈ �׷��� �ڽ� ������Ʈ�� Transform �޾ƿ���.
        //Transform[] points = GameObject.Find("PointGroup").GetComponentsInChildren<Transform>();
        //1���� �迭�� ���̱����� ���� �� Random�� ���� ����
        //int idx = Random.Range(1, points.Length);
        //�÷��̾� �������� ������ idx ��ġ�� ȸ�� ���� ����. ��Ʈ��ũ�� ���ؼ�.
        //PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);

        //������ Ŭ���̾�Ʈ�� ��� ���� �� �ε�
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MenuScene"); //�� �̸����� �ҷ�����
        }
    }

    private void Start()
    {
        //���� ID ���� ����
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1, 21):00}"); //20����� �ۿ� �������Ƿ� 1~21 ����. :00�� �� �ڸ��� �� �ڸ��� ������ַ���.
        userInputField.text = userId;
        //���� �г��� ��Ʈ��ũ ���
        PhotonNetwork.NickName = userId;
    }

    //�������� �����ϴ� ����
    public void SetUserId()
    {
        //��ǲ �ʵ尡 ��������� ������ ��, �׷��� ������ ������ ������ ��. �ٽ� �������� �� �г��� ����
        if (string.IsNullOrEmpty(userInputField.text))
        {
            userId = $"USER_{Random.Range(1, 21):00}";
        }
        else
        {
            userId = userInputField.text;
        }
        //������ ����. �κ񿡼� ���� ��ü�� ���ο����� �� �� �ִ�.
        PlayerPrefs.SetString("USER_ID", userId);
        PhotonNetwork.NickName = userId; //��Ʈ��ũ���� �ݿ�
    }

    string SetRoomName()
    {
        //��������� ������ �� �̸�. �׷��� ������ ����������.
        if (string.IsNullOrEmpty(roomInputField.text))
        {
            roomInputField.text = $"ROOM_{Random.Range(1, 101):000}";
        }
        return roomInputField.text;
    }

    public void OnLoginClick() //�α��� ��ư ���� �Լ�
    {
        SetUserId();
        // �α��� �Ϸ� �� ��ư ��Ȱ��ȭ
        DisableLoginButton();
        userInputField.interactable = false;
    }

    private void DisableLoginButton()
    {
        // ��ư ��Ȱ��ȭ
        loginButton.interactable = false;

        // ��ư ���� ����
        Image buttonImage = loginButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = Color.gray; // ���ϴ� �������� ����
        }
    }

    public void JoinRoom()
    {
        //������ ������ ����
        PhotonNetwork.JoinRandomRoom();
    }
    public void OnMakeRoomClick() //�� ���� ��ư ���� �Լ�
    {
        //���� ID ����
        SetUserId();
        //�� �Ӽ� ����
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 2;
        ro.IsOpen = true;
        //������ ����
        ro.IsVisible = true;
        //�� ����
        PhotonNetwork.CreateRoom(SetRoomName(), ro); //������ ���� �ƴ϶� ������ Ÿ������ ���� �޾ƿ´�.
    }
    //�� ����Ʈ�� �����ϴ� �ݹ� �Լ� ����
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ������ RoomItem �������� ������ �ӽú���
        GameObject tempRoom = null;
        foreach (var roomInfo in roomList)
        {
            // ���� ������ ���
            if (roomInfo.RemovedFromList == true)
            {
                // ��ųʸ����� �� �̸����� �˻��� ����� RoomItem �������� ����
                rooms.TryGetValue(roomInfo.Name, out tempRoom);
                // RoomItem ������ ����
                Destroy(tempRoom);
                // ��ųʸ����� �ش� �� �̸��� �����͸� ����
                rooms.Remove(roomInfo.Name);

            }
            else // �� ������ ����� ���
            {
                // �� �̸��� ��ųʸ��� ���� ��� ���� �߰�
                if (rooms.ContainsKey(roomInfo.Name) == false)
                {
                    // RoomInfo �������� scrollContent ������ ����
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
                    // �� ������ ǥ���ϱ� ���� RoomInfo ���� ����
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;
                    // ��ųʸ� �ڷ����� ������ �߰�
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else  // �� �̸��� ��ųʸ��� ���� ��쿡 �� ������ ����
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
            }
            Debug.Log($"Room={roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})");
        }
    }
}
