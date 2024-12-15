using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private float speed;
    public S3SceneManager S3sm; //s3
    float h;
    float v;

    bool isHorizonMove;
    bool isVerticalMove;

    string playerTag;

    GameManager gm;
    Rigidbody2D rd;
    Animator anim;
    public PhotonView pv;
    public Text nicknameText;

    //s3 raycast
    Vector3 dirVec;//바라보고 있는 방향
    GameObject scanObject;
    GameObject portalObject;
    private GameObject currentRoad; // Player2의 현재 Road 상태

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        rd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        DontDestroyOnLoad(this.gameObject);

        nicknameText.text = pv.IsMine ? PhotonNetwork.NickName : pv.Owner.NickName;
        nicknameText.color = pv.IsMine ? Color.green : Color.red;
    }

    void Update()
    {
        if (pv.IsMine) // 자신의 플레이어만 동작
        {
            // 입력값 처리
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            bool hDown = Input.GetButtonDown("Horizontal");
            bool vDown = Input.GetButtonDown("Vertical");
            bool hUp = Input.GetButtonUp("Horizontal");
            bool vUp = Input.GetButtonUp("Vertical");

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

            if (hDown)
                isHorizonMove = true;
            else if (vDown)
                isHorizonMove = false;
            else if (vUp || hUp)
                isHorizonMove = h != 0;

            // 애니메이션 상태 처리
            if (anim.GetInteger("hAxisRaw") != h)
            {
                anim.SetBool("isChange", true);
                anim.SetInteger("hAxisRaw", (int)h);
            }
            else if (anim.GetInteger("vAxisRaw") != v)
            {
                anim.SetBool("isChange", true);
                anim.SetInteger("vAxisRaw", (int)v);
            }
            else
                anim.SetBool("isChange", false);
        }
    }

    void FixedUpdate()
    {
        if (pv.IsMine)
        {
            Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
            rd.velocity = moveVec * speed;
            if (Input.GetButtonDown("Jump"))
            {
                // Ray 시작 위치를 더 오른쪽으로 이동
                Vector2 rayStartPos = rd.position + new Vector2(0.2f, -0.1f);

                // 디버그 선 추가
                Debug.DrawRay(rayStartPos, dirVec * 0.5f, Color.red);

                // Ray를 발사
                RaycastHit2D rayHit = Physics2D.Raycast(
                    rayStartPos, // 시작 위치
                    dirVec,      // 방향
                    1f,          // 길이 (0.35f -> 1f로 변경)
                    LayerMask.GetMask("Object") // Object 레이어만 탐색
                );

                if (rayHit.collider != null)
                {
                    Debug.Log("Ray Hit Object: " + rayHit.collider.gameObject.name);
                    Debug.Log("Ray Hit Object: " + rayHit.collider.gameObject.tag);
                    scanObject = rayHit.collider.gameObject;
                }
                else
                {
                    scanObject = null;
                }
            }
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Photon 상태 동기화용 (필요 시 구현)
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal"))
        {
            // Portal과의 충돌 로직 추가
            S3Portal portal = collision.GetComponent<S3Portal>();
            if (portal != null)
            {
                portal.OnPlayerEnter(gameObject);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        string objectName = collision.gameObject.name;
        // Player 1과 Button 상호작용
        if (CompareTag("player1") && objectName.StartsWith("Button"))
        {
            Debug.Log($"Player 1이 {objectName}과 지속적으로 상호작용 중.");
            S3SceneManager.Instance.SetP1ObjectName(objectName);
        }
        // Player 2와 Road 상호작용
        else if (CompareTag("player2") && objectName.StartsWith("Road"))
        {
            // Road 충돌 상태를 계속 업데이트
            if (currentRoad != collision.gameObject)
            {
                currentRoad = collision.gameObject; // 새로운 Road로 업데이트
                Debug.Log($"Player2가 새로운 Road({currentRoad.name})에 충돌했습니다.");
                S3SceneManager.Instance.SetP1ObjectName(objectName);
                S3SceneManager.Instance.SetP2ObjectName(objectName);
            }

            // 버튼과 상호작용하지 않은 상태에서 속도 감소
            if (!S3sm.IsButtonInteracted())
            {
                SetSpeedRPC(0.5f);
            }
            Debug.Log($"Player 2가 {objectName}과 지속적으로 상호작용 중.");
            S3SceneManager.Instance.SetP2ObjectName(objectName);
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
            SetPosition(0f, 0f, 0f);
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