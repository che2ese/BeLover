using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

//MonoBehaviour 수정.
//Photon 같은 경우 플레이어를 움직이기 보다는 서버에 접속하는 역할이라 다른 클래스를 상속받는다

public class PhotonManager : MonoBehaviourPunCallbacks //상속 클래스 변경
{
    //변수 선언
    private readonly string version = "1.0";//게임 버전 체크. 유저가 건드리지 못하게 private, readonly
    private string userId = "Victor"; //아무거나 userId 생성

    //유저 ID를 입력할 인풋 필드
    public TMP_InputField userInputField;
    //룸 ID를 입력할 인풋 필드
    public TMP_InputField roomInputField;

    //로그인 버튼
    public Button loginButton;

    //룸 목록에 대한 데이터 저장
    Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    //룸 목록을 표시할 프리팹
    GameObject roomItemPrefab;
    //룸 목록이 표시될 scroll content
    public Transform scrollContent;

    //네트워크 접속은 Start()보다 먼저 실행되어야한다. Awake() 함수 사용
    private void Awake()
    {
        //씬 동기화. 맨 처음 접속한 사람이 방장이 된다.
        PhotonNetwork.AutomaticallySyncScene = true;
        //버전 할당. 위에 string으로 만들었던 version을 쓴다.
        PhotonNetwork.GameVersion = version;
        //App ID 할당. 위에 userId로 만들었던 userId를 쓴다.
        PhotonNetwork.NickName = userId;
        //포톤 서버와의 통신 횟수를 로그로 찍기. 기본값 : 30
        Debug.Log(PhotonNetwork.SendRate); //제대로 통신이 되었다면 30이 출력된다.

        //RoomItem 프리팹 로드 Resources 폴더로부터...
        roomItemPrefab = Resources.Load<GameObject>("RoomItem");

        //포톤 서버에 접속
        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    //CallBack 함수
    public override void OnConnectedToMaster() //정상적으로 마스터 서버에 접속이 되면 호출된다.
    {
        //마스터 서버에 접속이 되었는지 디버깅 한다.
        Debug.Log("Connected to Master");
        Debug.Log($"In Lobby = {PhotonNetwork.InLobby}"); //로비에 들어와 있으면 True, 아니면 False 반환. Master 서버에는 접속했지만 로비에는 아니므로 False 반환된다.
        //로비 접속
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() //로비에 접속이 제대로 되었다면 해당 콜백함수 호출
    {
        Debug.Log($"In Lobby = {PhotonNetwork.InLobby}"); //로비에 접속이 되었다면 True가 반환 될 것이다.
        //방 접속 방법은 두 가지. 1.랜덤 매치메이킹, 2.선택된 방 접속
        //PhotonNetwork.JoinRandomRoom();
    }

    //방 생성이 되지 않았으면 오류 콜백 함수 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}: {message}");

        OnMakeRoomClick(); //오류 나는 것을 방지하기 위해서.

        //룸 속성 설정
        //RoomOptions roomOptions = new RoomOptions();
        //룸의 접속할 수 있는 최대 접속자 수 최대 제한을 해놔야 CCU를 제한할 수 있다.
        //roomOptions.MaxPlayers = 20;
        //룸 오픈 여부
        //roomOptions.IsOpen = true;
        //로비에서 룸의 목록에 노출시킬지 여부. 공개방 생성
        //roomOptions.IsVisible = true;
        //룸 생성
        //PhotonNetwork.CreateRoom("Room1", roomOptions); //룸 이름과 룸 설정. 우리는 roomOptions에 설정을 이미 해놓았다.
    }

    //제대로 룸이 있다면 다음의 콜백 함수를 호출한다.
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name: {PhotonNetwork.CurrentRoom.Name}");
    }

    //룸에 들어왔을 때 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"In Room = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        //접속한 사용자 닉네임 확인
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            //플레이어 닉네임, 유저의 고유값 가져오기
            Debug.Log($"플레이어 닉네임: {player.Value.NickName}, 유저 고유값: {player.Value.ActorNumber}");
        }

        //플레이어 생성 포인트 그룹 배열을 받아오기. 포인트 그룹의 자식 오브젝트의 Transform 받아오기.
        //Transform[] points = GameObject.Find("PointGroup").GetComponentsInChildren<Transform>();
        //1부터 배열의 길이까지의 숫자 중 Random한 값을 추출
        //int idx = Random.Range(1, points.Length);
        //플레이어 프리팹을 추출한 idx 위치와 회전 값에 생성. 네트워크를 통해서.
        //PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);

        //마스터 클라이언트인 경우 게임 씬 로딩
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MenuScene"); //씬 이름으로 불러오기
        }
    }

    private void Start()
    {
        //유저 ID 랜덤 설정
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1, 21):00}"); //20명까지 밖에 못들어오므로 1~21 설정. :00은 한 자리도 두 자리로 만들어주려고.
        userInputField.text = userId;
        //접속 닉네임 네트워크 등록
        PhotonNetwork.NickName = userId;
    }

    //유저명을 설정하는 로직
    public void SetUserId()
    {
        //인풋 필드가 비어있으면 랜덤한 값, 그렇지 않으면 유저가 생성한 값. 다시 접속했을 때 닉네임 보존
        if (string.IsNullOrEmpty(userInputField.text))
        {
            userId = $"USER_{Random.Range(1, 21):00}";
        }
        else
        {
            userId = userInputField.text;
        }
        //유저명 저장. 로비에서 만든 개체를 메인에서도 쓸 수 있다.
        PlayerPrefs.SetString("USER_ID", userId);
        PhotonNetwork.NickName = userId; //네트워크에도 반영
    }

    string SetRoomName()
    {
        //비어있으면 랜덤한 룸 이름. 그렇지 않으면 가져오도록.
        if (string.IsNullOrEmpty(roomInputField.text))
        {
            roomInputField.text = $"ROOM_{Random.Range(1, 101):000}";
        }
        return roomInputField.text;
    }

    public void OnLoginClick() //로그인 버튼 매핑 함수
    {
        SetUserId();
        // 로그인 완료 후 버튼 비활성화
        DisableLoginButton();
        userInputField.interactable = false;
    }

    private void DisableLoginButton()
    {
        // 버튼 비활성화
        loginButton.interactable = false;

        // 버튼 색상 변경
        Image buttonImage = loginButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = Color.gray; // 원하는 색상으로 설정
        }
    }

    public void JoinRoom()
    {
        //무작위 룸으로 입장
        PhotonNetwork.JoinRandomRoom();
    }
    public void OnMakeRoomClick() //방 생성 버튼 매핑 함수
    {
        //유저 ID 저장
        SetUserId();
        //룸 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 2;
        ro.IsOpen = true;
        //공개방 설정
        ro.IsVisible = true;
        //룸 생성
        PhotonNetwork.CreateRoom(SetRoomName(), ro); //고정된 값이 아니라 유저가 타이핑한 값을 받아온다.
    }
    //방 리스트를 수신하는 콜백 함수 생성
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 삭제된 RoomItem 프리팹을 저장할 임시변수
        GameObject tempRoom = null;
        foreach (var roomInfo in roomList)
        {
            // 룸이 삭제된 경우
            if (roomInfo.RemovedFromList == true)
            {
                // 딕셔너리에서 룸 이름으로 검색해 저장된 RoomItem 프리팹을 추출
                rooms.TryGetValue(roomInfo.Name, out tempRoom);
                // RoomItem 프리팹 삭제
                Destroy(tempRoom);
                // 딕셔너리에서 해당 룸 이름의 데이터를 삭제
                rooms.Remove(roomInfo.Name);

            }
            else // 룸 정보가 변경된 경우
            {
                // 룸 이름이 딕셔너리에 없는 경우 새로 추가
                if (rooms.ContainsKey(roomInfo.Name) == false)
                {
                    // RoomInfo 프리팹을 scrollContent 하위에 생성
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
                    // 룸 정보를 표시하기 위해 RoomInfo 정보 전달
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;
                    // 딕셔너리 자료형에 데이터 추가
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else  // 룸 이름이 딕셔너리에 없는 경우에 룸 정보를 갱신
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
            }
            Debug.Log($"Room={roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})");
        }
    }
}
