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

    private bool isOtherMaleSelected = false;
    private bool isOtherFemaleSelected = false;
    private bool isCharSelectionLocked = false; // ĳ���� ���� ���

    Animator anim;
    [SerializeField]
    private RuntimeAnimatorController[] animatorControllers; // �ִϸ��̼� ��Ʈ�ѷ� �迭

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

        // ��� Ŭ���̾�Ʈ���� GameStart ���¸� ����ȭ�ϰ� �� ��ȯ ����
        photonView.RPC("RPC_StartGame", RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartGame()
    {
        isGameStart = true;
    }

    public void Spawn()
    {
        // ���� �÷��̾ ������ ĳ���Ϳ� ���� �±׿� �ִϸ��̼� ����
        string localTag = isMaleSelected ? "player1" : "player2";

        // �÷��̾ �����մϴ�.
        GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        Animator playerAnimator = player.GetComponent<Animator>();

        // ���� �÷��̾� ����
        player.tag = localTag;
        playerAnimator.runtimeAnimatorController = isMaleSelected ? animatorControllers[0] : animatorControllers[1];

        // ���濡�� ���� �÷��̾��� ���� ������ ����
        photonView.RPC("SetRemotePlayerAppearance", RpcTarget.OthersBuffered, player.GetComponent<PhotonView>().ViewID, isMaleSelected);
    }

    [PunRPC]
    void SetRemotePlayerAppearance(int viewID, bool isRemoteMale)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            // ������ ������ ĳ���͸� �״�� �ݿ�
            string remoteTag = isRemoteMale ? "player1" : "player2";
            targetView.gameObject.tag = remoteTag;

            Animator playerAnimator = targetView.gameObject.GetComponent<Animator>();
            playerAnimator.runtimeAnimatorController = isRemoteMale ? animatorControllers[0] : animatorControllers[1];
        }
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
            StartError.text = "�� ���� �÷��̾ ��� �����ؾ�\nĳ���͸� ������ �� �ֽ��ϴ�.";
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
            // �̹� ���� �Ǵ� ���ݿ��� ���� ĳ���Ͱ� ���õ� ���
            if (isMaleSelected || isOtherMaleSelected)
            {
                StartError.text = "�̹� ���õ� ĳ�����Դϴ�.";
                Invoke("ClearText", 3f);
                return;
            }

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

            PhotonView.Get(this).RPC("RPC_SelectChar", RpcTarget.OthersBuffered, "male");
        }
        else if (character == "female")
        {
            // �̹� ���� �Ǵ� ���ݿ��� ���� ĳ���Ͱ� ���õ� ���
            if (isFemaleSelected || isOtherFemaleSelected)
            {
                StartError.text = "�̹� ���õ� ĳ�����Դϴ�.";
                Invoke("ClearText", 3f);
                return;
            }

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

            PhotonView.Get(this).RPC("RPC_SelectChar", RpcTarget.OthersBuffered, "female");
        }

        CheckIfBothSelected();
    }

    [PunRPC]
    void RPC_SelectChar(string character)
    {
        if (character == "male")
        {
            // �̹� ���ݿ��� ���� ĳ���Ͱ� ���õ� ���
            if (isOtherMaleSelected || isMaleSelected)
            {
                StartError.text = "�̹� ���õ� ĳ�����Դϴ�.";
                Invoke("ClearText", 3f);
                return;
            }

            isOtherMaleSelected = true;
            isOtherFemaleSelected = false;
            isOtherCharSelected = true; // ���� ĳ���Ͱ� ���õǾ����� ǥ��

            Color maleColor = MaleSel.color;
            maleColor.a = 1f;
            MaleSel.color = maleColor;

            Color femaleColor = FemaleSel.color;
            femaleColor.a = 0.4f;
            FemaleSel.color = femaleColor;
        }
        else if (character == "female")
        {
            // �̹� ���ݿ��� ���� ĳ���Ͱ� ���õ� ���
            if (isOtherFemaleSelected || isFemaleSelected)
            {
                StartError.text = "�̹� ���õ� ĳ�����Դϴ�.";
                Invoke("ClearText", 3f);
                return;
            }

            isOtherFemaleSelected = true;
            isOtherMaleSelected = false;
            isOtherCharSelected = true; // ���� ĳ���Ͱ� ���õǾ����� ǥ��

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
