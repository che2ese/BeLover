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
    public SpriteRenderer MaleSel;
    public SpriteRenderer FemaleSel;
    private bool isMaleSelected = false;
    private bool isFemaleSelected = false;

    private bool isCharSelectionLocked = false; // ĳ���� ���� ���

    [SerializeField]
    private byte maxPlayers = 2;

    private bool isMyCharSelected = false;
    private bool isOtherCharSelected = false;

    void Awake()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        PhotonNetwork.ConnectUsingSettings();
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
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
    }

    public void Connect()
    {
        if (string.IsNullOrEmpty(NickNameInput.text))
        {
            Debug.LogWarning("�г����� �Է��ϼ���.");
            NickError.text = "�г����� �Է��ϼ���.";
            Invoke("ClearText", 3f);
            return;
        }

        // �г��� �ߺ� Ȯ��
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == NickNameInput.text)
            {
                Debug.LogWarning("�̹� ��� ���� �г����Դϴ�.");
                NickError.text = "�̹� ��� ���� �г����Դϴ�.";
                Invoke("ClearText", 3f);
                return;
            }
        }

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
        if (!isMyCharSelected || !isOtherCharSelected)
        {
            Debug.LogWarning("�� ���� �÷��̾ ��� ĳ���͸� �����ؾ� �մϴ�.");
            if (StartError != null)
            {
                StartError.text = "�� ���� �÷��̾ ��� ĳ���͸� �����ؾ� �մϴ�.";
                Invoke("ClearText", 3f);
            }
            return;
        }

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
        Debug.Log("������ ���۵˴ϴ�.");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
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
        Debug.Log("Successfully joined the room.");
        UpdateOtherPlayerNick();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player entered: " + newPlayer.NickName);
        UpdateOtherPlayerNick();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player left: " + otherPlayer.NickName);
        UpdateOtherPlayerNick();
        ResetCharacterSelection();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        UpdateOtherPlayerNick();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        Debug.LogWarning($"Disconnected from Photon Server: {cause}");
    }

    void ClearText()
    {
        if (NickError != null)
            NickError.text = "";

        if (StartError != null)
            StartError.text = "";
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
                    return;
                }
            }
        }
        else
        {
            OtherNick.text = "Waiting for other player...";
        }
    }

    public void SelectChar(string character)
    {
        if (MaleSel == null || FemaleSel == null)
        {
            Debug.LogError("MaleSel �Ǵ� FemaleSel�� null�Դϴ�. Unity Editor���� �� �ʵ���� ����� ����Ǿ����� Ȯ���ϼ���.");
            return;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount < maxPlayers)
        {
            StartError.text = "�� ���� �÷��̾ ��� �����ؾ� ĳ���͸� ������ �� �ֽ��ϴ�.";
            Invoke("ClearText", 3f);
            return;
        }

        if (isGameStart || isCharSelectionLocked)
        {
            StartError.text = "����� �̹� �����Ͽ����ϴ�.";
            Invoke("ClearText", 3f);
            return;
        }

        // ���� ĳ���� ���� ���¿� ���� ó��
        if (character == "male")
        {
            if (!isMaleSelected || (isMaleSelected && !isOtherCharSelected))
            {
                isMyCharSelected = true;
                isMaleSelected = true;
                isFemaleSelected = false;
                isCharSelectionLocked = true;

                // ���� ĳ���� ����
                Color maleColor = MaleSel.color;
                maleColor.a = 1f;
                MaleSel.color = maleColor;

                Color femaleColor = FemaleSel.color;
                femaleColor.a = 0.4f;
                FemaleSel.color = femaleColor;

                // �±� ����
                gameObject.tag = "player1";

                PhotonView.Get(this).RPC("RPC_SelectChar", RpcTarget.Others, "male");
            }
            else
            {
                StartError.text = "�̹� ���õ� ĳ�����Դϴ�.";
                Invoke("ClearText", 3f);
            }
        }
        else if (character == "female")
        {
            if (!isFemaleSelected || (isFemaleSelected && !isOtherCharSelected))
            {
                isMyCharSelected = true;
                isFemaleSelected = true;
                isMaleSelected = false;
                isCharSelectionLocked = true;

                // ���� ĳ���� ����
                Color femaleColor = FemaleSel.color;
                femaleColor.a = 1f;
                FemaleSel.color = femaleColor;

                Color maleColor = MaleSel.color;
                maleColor.a = 0.4f;
                MaleSel.color = maleColor;

                // �±� ����
                gameObject.tag = "player2";

                PhotonView.Get(this).RPC("RPC_SelectChar", RpcTarget.Others, "female");
            }
            else
            {
                StartError.text = "�̹� ���õ� ĳ�����Դϴ�.";
                Invoke("ClearText", 3f);
            }
        }

        CheckIfBothSelected();
    }


    [PunRPC]
    void RPC_SelectChar(string character)
    {
        if (character == "male")
        {
            if (isMaleSelected && isOtherCharSelected)
            {
                StartError.text = "�̹� ���õ� ĳ�����Դϴ�.";
                Invoke("ClearText", 3f);
                return;
            }

            isOtherCharSelected = true;
            isMaleSelected = true;
            isFemaleSelected = false;

            Color maleColor = MaleSel.color;
            maleColor.a = 1f;
            MaleSel.color = maleColor;

            Color femaleColor = FemaleSel.color;
            femaleColor.a = 0.4f;
            FemaleSel.color = femaleColor;
        }
        else if (character == "female")
        {
            if (isFemaleSelected && isOtherCharSelected)
            {
                StartError.text = "�̹� ���õ� ĳ�����Դϴ�.";
                Invoke("ClearText", 3f);
                return;
            }

            isOtherCharSelected = true;
            isFemaleSelected = true;
            isMaleSelected = false;

            Color femaleColor = FemaleSel.color;
            femaleColor.a = 1f;
            FemaleSel.color = femaleColor;

            Color maleColor = MaleSel.color;
            maleColor.a = 0.4f;
            MaleSel.color = maleColor;
        }

        isCharSelectionLocked = false; // ���� �Ϸ� �� ��� ����

        CheckIfBothSelected();
    }

    void CheckIfBothSelected()
    {
        if (isMyCharSelected && isOtherCharSelected)
        {
            StartError.text = "��� ĳ���͸� �����߽��ϴ�. ������ ������ �� �ֽ��ϴ�.";

            // �� ĳ���� ��� �������ϰ� ����
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
        isCharSelectionLocked = false; // ���� ���� �ʱ�ȭ

        SetInitialCharacterColors();
    }
}
