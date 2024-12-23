using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviourPunCallbacks
{
    public MainSceneManager Msm;

    [SerializeField]
    private float speed;
    public S3SceneManager S3sm; //s3

    [SerializeField] private int minSortingOrder = 10; // Y 값이 -500일 때의 최소 레이어
    [SerializeField] private int maxSortingOrder = 1000000; // Y 값이 500일 때의 최대 레이어
    [SerializeField] private float offsetMultiplier = 5f;
    private float minY = -500f; // Y 좌표 최소값
    private float maxY = 500f;  // Y 좌표 최대값

    float h;
    float v;

    bool isHorizonMove;
    bool isVerticalMove;

    string playerTag;

    Rigidbody2D rd;
    Animator anim;
    SpriteRenderer spriteRenderer;
    public PhotonView pv;
    public Text nicknameText;

    private float horizontalInput = 0f;
    private float verticalInput = 0f;

    //s3 raycast
    Vector3 dirVec;//바라보고 있는 방향
    GameObject scanObject;
    GameObject portalObject;
    private GameObject currentRoad; // Player2의 현재 Road 상태
    
    //s3 gimmick
    public int candlecount;

    void Awake()
    {
        rd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        S3sm = FindObjectOfType<S3SceneManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        DontDestroyOnLoad(this.gameObject);

        nicknameText.text = pv.IsMine ? PhotonNetwork.NickName : pv.Owner.NickName;
        nicknameText.color = pv.IsMine ? Color.green : Color.red;
    }

    void Update()
    {
        if (pv.IsMine) // 자신의 플레이어만 동작
        {
            if (Msm != null && Msm.StoryPanel.activeSelf)
            {
                if (Input.GetButtonDown("Jump"))
                {   
                    Msm.StoryPanel.SetActive(false);
                }
                //  플레이어의 이동을 정지시킴
                rd.velocity = Vector2.zero; // 움직임 정지
                return; //  더 이상 코드 실행 중지
            }
            if (Msm != null && Msm.StoryPanel1.activeSelf)
            {
                //  플레이어의 이동을 정지시킴
                rd.velocity = Vector2.zero; // 움직임 정지
                anim.SetBool("isChange", false); // 애니메이션 정지
                return; //  더 이상 코드 실행 중지
            }
            
            // 입력값 처리
            h =  Input.GetAxisRaw("Horizontal");
            v =  Input.GetAxisRaw("Vertical");

            bool hDown = Input.GetButtonDown("Horizontal");
            bool vDown = Input.GetButtonDown("Vertical");
            bool hUp =  Input.GetButtonUp("Horizontal");
            bool vUp =  Input.GetButtonUp("Vertical");

            if (h != 0f && v == 0f)
            {
                horizontalInput = h;
                verticalInput = 0f;
            }
            else if (v != 0f && h == 0f)
            {
                verticalInput = v;
                horizontalInput = 0f;
            }
            else if (h == 0f && v == 0f)
            {
                horizontalInput = 0f;
                verticalInput = 0f;
            }


            // 태그 설정
            playerTag = gameObject.CompareTag("player1") ? "player1" : "player2";
            ExitGames.Client.Photon.Hashtable playerInput = new ExitGames.Client.Photon.Hashtable
            {
                { "Horizontal", h },
                { "Vertical", v },
                { "Tag", playerTag }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerInput);

            // Scene1일 때
            if (SceneManager.GetActiveScene().name == "Scene1" || SceneManager.GetActiveScene().name == "Scene2")
            {
                // 플레이어를 화면 밖으로 이동시켜서 보이지 않게 함
                transform.position = new Vector3(10000f, 10000f, 10000f);
            }
            else if (SceneManager.GetActiveScene().name == "Scene3-1" || SceneManager.GetActiveScene().name == "MainScene")
            {
                SetDirection(hDown, vDown);
                if (Input.GetButtonDown("Jump") && scanObject != null)
                {
                    if (SceneManager.GetActiveScene().name == "Scene3-1")
                    {
                        Debug.Log(scanObject.name);
                        S3sm.Action(scanObject);
                        if (scanObject != null && scanObject.name == "Skull_True")
                        {
                            Debug.Log("Player 2가 Skull_True 오브젝트와 상호작용했습니다. 클리어 화면을 모든 클라이언트에 표시합니다.");
                            S3sm.photonView.RPC("ShowClearUI_RPC", RpcTarget.All, 2); // 조건 2번
                        }
                        if (scanObject != null && scanObject.CompareTag("Candle"))
                        {
                            S3ObjectData objData = scanObject.GetComponent<S3ObjectData>();
                            if (objData != null && objData.isActivated)
                            {
                                Debug.Log($"이미 활성화된 {scanObject.name}과는 더 이상 상호작용할 수 없습니다.");
                                return;
                            }

                            if (objData != null && objData.linkedCandleFire != null)
                            {
                                objData.linkedCandleFire.SetActive(true);
                                Debug.Log($"{scanObject.name}의 CandleFire 활성화 완료");
                            }
                            else
                            {
                                Debug.LogWarning($"CandleFire가 {scanObject.name}와 연결되지 않았습니다.");
                                return;
                            }

                            objData.isActivated = true;
                            candlecount++;
                            Debug.Log($"캔들 카운트 증가: {candlecount}");
                            S3sm.CheckStageClear(candlecount);
                        }

                    }

                }
                if (Input.GetButtonDown("Jump") && scanObject != null)
                {
                    NetworkManager nm = FindAnyObjectByType<NetworkManager>();
                    Debug.Log(scanObject.name);

                    if (scanObject.tag == "MainMission")
                    {
                        HandleMissionSelection(scanObject.name, nm);
                    }
                    else
                    {
                        HandleMissionClearCheck(scanObject.name);
                    }
                }
            }
            else if (SceneManager.GetActiveScene().name == "Scene4")
            {
                SetDirection(hDown, vDown);
                if (CompareTag("player2"))
                {
                    GameObject blindObject = GameObject.FindGameObjectWithTag("Blind");
                    if (blindObject != null)
                    {
                        blindObject.SetActive(false);
                    }
                }
                if (Input.GetButtonDown("Jump") && scanObject != null)
                {
                    Debug.Log(scanObject.name);
                    if (scanObject.name == "ResetStatue")
                    {
                        S4Manager s4manager = FindObjectOfType<S4Manager>();
                        s4manager.ResetButtons();
                    }
                }
            }
        }
    }

    void LateUpdate()
    {

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            // Y 좌표를 sortingOrder 범위로 매핑
            float normalizedY = Mathf.InverseLerp(minY, maxY, transform.position.y);
            float lerpedOrder = Mathf.Lerp(maxSortingOrder, minSortingOrder, normalizedY) * offsetMultiplier;
            int calculatedOrder = Mathf.RoundToInt(lerpedOrder);

            // 정수로 변환하여 설정
            spriteRenderer.sortingOrder = Mathf.RoundToInt(calculatedOrder);
        }
        else
        {
            spriteRenderer.sortingOrder = 10;
        }
    }


    void FixedUpdate()
    {
        if (pv.IsMine)
        {
            Vector2 moveVec = new Vector2(horizontalInput, verticalInput);
            rd.velocity = moveVec * speed;

            anim.SetInteger("hAxisRaw", (int)moveVec.x);
            anim.SetInteger("vAxisRaw", (int)moveVec.y);

            pv.RPC("SyncAnimation", RpcTarget.Others, (int)horizontalInput, (int)verticalInput);

            // Ray 시작 위치를 더 오른쪽으로 이동
            Vector2 rayStartPos = rd.position + new Vector2(0.2f, -0.3f);

            // 디버그 선 추가
            Debug.DrawRay(rayStartPos, dirVec * 0.5f, Color.red);

            // Ray를 발사
            RaycastHit2D rayHit = Physics2D.Raycast(
                rayStartPos, // 시작 위치
                dirVec,      // 방향
                0.5f,        // 길이
                LayerMask.GetMask("Object") // Object 레이어만 탐색
            );

            if (rayHit.collider != null)
            {
                // 새로운 오브젝트를 스캔
                scanObject = rayHit.collider.gameObject;
            }
            else
            {
                // Raycast가 아무것도 감지하지 못한 경우
                scanObject = null;
            }
        }
    }

    [PunRPC]
    void SyncAnimation(int hInput, int vInput)
    {
        anim.SetInteger("hAxisRaw", hInput);
        anim.SetInteger("vAxisRaw", vInput);
    }

    void HandleMissionSelection(string missionName, NetworkManager nm)
    {
        // UI 패널에 메시지 표시
        if (missionName == "MainMission1")
        {
            Msm.StoryPanel.SetActive(true);
            Msm.StoryText.text = "거울의 방";
            Msm.ghostImage.sprite = Msm.GhostImage[0];
            Msm.ghostImage.color = new Color(1f, 1f, 1f);
        }
        else if (missionName == "MainMission2")
        {
            Msm.StoryPanel.SetActive(true);
            Msm.StoryText.text = "분리의 방";
            Msm.ghostImage.sprite = Msm.GhostImage[1];
            Msm.ghostImage.color = new Color(1f, 1f, 1f);
        }
        else if (missionName == "MainMission3")
        {
            Msm.StoryPanel.SetActive(true);
            Msm.StoryText.text = "어둠의 방";
            Msm.ghostImage.sprite = Msm.GhostImage[2];
            Msm.ghostImage.color = new Color(1f, 1f, 1f);
        }

        // 선택한 미션 정보를 Photon CustomProperties에 저장
        ExitGames.Client.Photon.Hashtable playerSelection = new ExitGames.Client.Photon.Hashtable
    {
        { "SelectedMission", missionName }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelection);

        // 모든 플레이어의 선택 정보 확인 (코루틴으로 확인)
        StartCoroutine(CheckBothPlayersMissionSelection(missionName, nm));
    }

    void HandleMissionClearCheck(string missionName)
    {
        if (missionName == "MainMission1" && GameManager.Instance.isMission1Clear == true)
        {
            Msm.StoryPanel.SetActive(true);
            Msm.ghostImage.color = new Color(1f, 1f, 1f);
            Msm.ghostImage.sprite = Msm.GhostImage[0];
            Msm.StoryText.text = "이미 성불한 방이다";
        }

        if (missionName == "MainMission2" && GameManager.Instance.isMission2Clear == true)
        {
            Msm.StoryPanel.SetActive(true);
            Msm.ghostImage.color = new Color(1f, 1f, 1f);
            Msm.ghostImage.sprite = Msm.GhostImage[1];
            Msm.StoryText.text = "이미 성불한 방이다";
        }

        if (GameManager.Instance.isMission2Clear == false)
        {
            if (missionName == "MainMission2" || missionName == "MainMission3")
            {
                Msm.StoryPanel.SetActive(true);
                Msm.StoryText.text = "거울의 방부터 성불해라";
                if(missionName == "MainMission2")
                {
                    Msm.ghostImage.sprite = Msm.GhostImage[1];
                    Msm.ghostImage.color = new Color(0f, 0f, 0f);
                }
                else if(missionName == "MainMission3")
                {
                    Msm.ghostImage.sprite = Msm.GhostImage[2];
                    Msm.ghostImage.color = new Color(0f, 0f, 0f);
                }
            }
        }

        if (GameManager.Instance.isMission3Clear == false && missionName == "MainMission3")
        {
            Msm.StoryPanel.SetActive(true);
            Msm.StoryText.text = "분리의 방부터 성불해라";
            Msm.ghostImage.sprite = Msm.GhostImage[2];
            Msm.ghostImage.color = new Color(0f, 0f, 0f);
        }
    }

    IEnumerator CheckBothPlayersMissionSelection(string missionName, NetworkManager nm)
    {
        float elapsedTime = 0f;
        bool missionMatched = false;

        while (elapsedTime < 3f) // 3초 동안 매 0.5초마다 확인
        {
            yield return new WaitForSeconds(1.5f);
            elapsedTime += 1.5f;

            // 모든 플레이어의 선택한 미션 정보를 가져옴
            string player1Mission = "";
            string player2Mission = "";

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("SelectedMission"))
                {
                    string selectedMission = player.CustomProperties["SelectedMission"].ToString();
                    if (player.IsLocal)
                        player1Mission = selectedMission;
                    else
                        player2Mission = selectedMission;
                }
            }

            // 두 플레이어의 선택이 같은지 확인
            if (!string.IsNullOrEmpty(player1Mission) && player1Mission == player2Mission)
            {
                Debug.Log($"두 플레이어가 같은 미션을 선택했습니다: {missionName}");

                // 카운트다운 시작
                StartCoroutine(StartCountdownAndChangeScene(missionName, nm));
                missionMatched = true;
                break; // 코루틴 종료
            }
        }

        if (!missionMatched)
        {
            Debug.LogWarning("두 플레이어의 선택이 일치하지 않았습니다.");
        }
    }

    IEnumerator StartCountdownAndChangeScene(string missionName, NetworkManager nm)
    {
        string mapName = "알 수 없는 방";

        if (missionName == "MainMission1")
        {
            mapName = "거울의 방";
        }
        else if (missionName == "MainMission2")
        {
            mapName = "분리의 방";
        }
        else if (missionName == "MainMission3")
        {
            mapName = "어둠의 방";
        }
        int countdown = 3; // 카운트다운 시작 숫자

        while (countdown > 0)
        {
            Msm.countDown.text = mapName +"에 입장합니다\n\n"+ countdown.ToString(); // 텍스트 갱신
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        // 씬 변경
        if (missionName == "MainMission1")
        {
            nm.photonView.RPC("RPC_StartScene2", RpcTarget.All);
        }
        else if (missionName == "MainMission2")
        {
            nm.photonView.RPC("RPC_StartScene3", RpcTarget.All);
        }
        else if (missionName == "MainMission3")
        {
            nm.photonView.RPC("RPC_StartScene4", RpcTarget.All);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("MainScene");
            }
        }

        if (collision.CompareTag("Portal"))
        {
            // Portal과의 충돌 로직 추가
            S3Portal portal = collision.GetComponent<S3Portal>();
            if (portal != null)
            {
                portal.OnPlayerEnter(gameObject);
            }
            // **M1_2Portal과 충돌했을 때만 클리어 화면을 표시**
            if (collision.gameObject.name == "M1_2Portal" && CompareTag("player2"))
            {
                Debug.Log("Player 2가 M1_2Portal에 접촉했습니다. 클리어 화면을 모든 클라이언트에 표시합니다.");
                S3sm.photonView.RPC("ShowClearUI_RPC", RpcTarget.All, 1); // 조건 1번
            }

        }

        string objectName = collision.gameObject.name;
        if (CompareTag("player2") && objectName.StartsWith("Road"))
        {
            Debug.Log($"Player 22 {objectName}과 상호작용 시작.");

            ExitGames.Client.Photon.Hashtable player2Properties = new ExitGames.Client.Photon.Hashtable
            {
                { "Player2Road", objectName }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(player2Properties);

            S3SceneManager.Instance.CompareButtonAndRoad();
        }


        if (collision.CompareTag("Bridge"))
        {
            Debug.Log("지나갈 수 없어 보인다...");
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        string objectName = collision.gameObject.name;
        // Player 1과 Button 상호작용
        if (CompareTag("player1") && objectName.StartsWith("Button"))
        {
            Debug.Log($"Player 1이 {objectName}과 지속적으로 상호작용 중.");

            ExitGames.Client.Photon.Hashtable player1Properties = new ExitGames.Client.Photon.Hashtable
            {
                { "Player1Button", objectName }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(player1Properties);
        }
        // Player 2와 Road 상호작용
        if (CompareTag("player2") && objectName.StartsWith("Road"))
        {
            Debug.Log($"Player 22 {objectName}과 지속적으로 상호작용 중.");

            ExitGames.Client.Photon.Hashtable player2Properties = new ExitGames.Client.Photon.Hashtable
            {
                { "Player2Road", objectName }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(player2Properties);

            S3SceneManager.Instance.CompareButtonAndRoad();
        }


    }

    private void SetDirection(bool hDown, bool vDown)
    {
        if (vDown && v == 1) dirVec = Vector3.up;
        else if (vDown && v == -1) dirVec = Vector3.down;
        else if (hDown && h == -1) dirVec = Vector3.left;
        else if (hDown && h == 1) dirVec = Vector3.right;
    }
    new void OnEnable()
    {
        // SceneManager의 sceneLoaded에 OnSceneLoaded 연결
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (pv.IsMine)
        {
            AttachCameraToPlayer(); // 씬이 전환되었을 때 카메라 타겟 다시 연결
        }
    }

    new void OnDisable()
    {
        // 장면이 전환될 때 메서드 호출을 중지하기 위해 콜백 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 특정 장면이 로드될 때 호출할 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pv.IsMine)
        {
            AttachCameraToPlayer(); // 씬이 전환되었을 때 카메라 타겟 다시 연결
        }
        if (scene.name == "MainScene")
        {
            Msm = FindObjectOfType<MainSceneManager>(); // **씬이 로드될 때 Msm 다시 할당**
            if (Msm != null)
            {
                Debug.Log("MainSceneManager 연결 성공");
            }
            else
            {
                Debug.LogWarning("MainSceneManager를 찾을 수 없습니다.");
            }
            if (playerTag == "player1")
            {
                SetPosition(3.5f, -1f, 0f);
            }
            else if (playerTag == "player2")
            {
                SetPosition(2.8f, -1f, 0f);
            }
        }
        else if (scene.name == "Scene3-1")
        {
            // Scene3-1에서 S3SceneManager 찾기
            S3sm = FindObjectOfType<S3SceneManager>();
            if (S3sm != null)
            {
                Debug.Log("S3SceneManager가 Scene3-1에서 연결되었습니다.");
            }
            else
            {
                Debug.LogWarning("Scene3-1에 S3SceneManager 프리팹이 없습니다.");
            }

            if (playerTag == "player1")
            {
                SetPosition(-0.5f, 2f, 0f);
            }
            else if (playerTag == "player2")
            {
                SetPosition(3f, -2.5f, 0f);
            }

            if (playerTag == "player2")
            {
                SetSpeedRPC(0.5f); // Player2 속도를 느리게 설정
            }
        }
        else if (scene.name == "Scene4")
        {
            if (playerTag == "player1")
            {
                SetPosition(-0.4f, 3.7f, 0f);
            }
            else if (playerTag == "player2")
            {
                SetPosition(-0.35f, -28.8f, 0f);
            }
        }
        else
        {
            // 다른 씬에서는 S3SceneManager 사용하지 않음
            S3sm = null;
        }
    }
    private void SetPosition(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }
    private void AttachCameraToPlayer()
    {
        // MainCamera의 S3Camera 스크립트에 이 플레이어의 Transform을 연결
        S3Camera mainCamera = Camera.main.GetComponent<S3Camera>();
        if (mainCamera != null)
        {
            mainCamera.SetTarget(transform);
            Debug.Log("카메라가 플레이어에 연결되었습니다: " + transform.name);
        }
        else
        {
            Debug.LogWarning("S3Camera를 찾을 수 없습니다.");
        }
    }
    [PunRPC]
    public void SetSpeedRPC(float newSpeed)
    {
        Debug.Log($"SetSpeedRPC 호출됨: {tag}, 속도: {newSpeed}");

        if (CompareTag("player2")) // Player 2만 속도 변경
        {
            speed = newSpeed;
            Debug.Log($"Player 2의 속도가 {newSpeed}로 설정되었습니다.");
        }
        else
        {
            Debug.Log($"SetSpeedRPC 호출됨: Player 1이므로 속도 변경 없음.");
        }
    }

}