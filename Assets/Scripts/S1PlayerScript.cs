using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class S1PlayerScript : MonoBehaviourPunCallbacks
{
    GameManager gm;
    public float speed;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    private S2SceneManager S2sm;

    // Player 1과 Player 2의 입력을 따로 관리 (Scene2)
    private float horizontalInputPlayer1 = 0f;
    private float verticalInputPlayer1 = 0f;

    private float horizontalInputPlayer2 = 0f;
    private float verticalInputPlayer2 = 0f;

    // Scene1의 입력을 관리 (Scene1)
    private float horizontalInput = 0f;
    private float verticalInput = 0f;

    bool isFalling = false;

    public GameObject blindPanel; // **블라인드 UI 패널 오브젝트 (검정색) 연결**
    private bool isBlindActive = false; // **블라인드 활성화 상태를 확인하는 변수 (false가 초기값)**

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        S2sm = FindObjectOfType<S2SceneManager>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Scene1")
        {
            // **자신의 CustomProperties 확인**
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Tag"))
            {
                string myTag = (string)PhotonNetwork.LocalPlayer.CustomProperties["Tag"];

                // **내가 player1을 선택한 경우에만 블라인드 활성화**
                if (myTag == "player1" && !isBlindActive)
                {
                    ActivateBlind(); // **블라인드 활성화**
                    isBlindActive = true; // **중복 활성화 방지**
                }
                // **내가 player2를 선택한 경우 블라인드 비활성화**
                else if (myTag == "player2" && isBlindActive)
                {
                    DeactivateBlind(); // **블라인드 비활성화**
                    isBlindActive = false;
                }
            }

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("Horizontal") &&
                    player.CustomProperties.ContainsKey("Vertical") &&
                    player.CustomProperties.ContainsKey("Tag"))
                {
                    float h = (float)player.CustomProperties["Horizontal"];
                    float v = (float)player.CustomProperties["Vertical"];
                    string tag = (string)player.CustomProperties["Tag"];

                    if (tag == "player1")
                    {
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
                    }
                }
            }
        }
        else if (SceneManager.GetActiveScene().name == "Scene2")
        {
            if (photonView.IsMine)
            {
                float localHorizontal = Input.GetAxisRaw("Horizontal");
                float localVertical = Input.GetAxisRaw("Vertical");

                // **동시 입력 방지 - 하나의 입력만 유지**
                if (localHorizontal != 0f && localVertical != 0f)
                {
                    // **수직 입력이 우선 적용됨**
                    localHorizontal = 0f;
                }

                Hashtable props = new Hashtable
                {
                    { "Horizontal", localHorizontal },
                    { "Vertical", localVertical }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey("Horizontal") &&
                    player.CustomProperties.ContainsKey("Vertical") &&
                    player.CustomProperties.ContainsKey("Tag"))
                {
                    float h = (float)player.CustomProperties["Horizontal"];
                    float v = (float)player.CustomProperties["Vertical"];
                    string tag = (string)player.CustomProperties["Tag"];

                    if (tag == "player1")
                    {
                        // **동시 입력 방지**
                        if (h != 0f && v != 0f)
                        {
                            // 수직 입력이 우선 적용됨
                            h = 0f;
                        }

                        switch (S2sm.mirrorCount)
                        {
                            case 0:
                                horizontalInputPlayer1 = 0f;
                                verticalInputPlayer1 = v;
                                break;
                            case 1:
                                horizontalInputPlayer1 = Mathf.Max(0f, h);
                                verticalInputPlayer1 = Mathf.Max(0f, v);
                                break;
                            case 2:
                                horizontalInputPlayer1 = Mathf.Max(0f, h);
                                verticalInputPlayer1 = Mathf.Min(0f, v);
                                break;
                            case 3:
                                horizontalInputPlayer1 = Mathf.Min(0f, h);
                                verticalInputPlayer1 = Mathf.Min(0f, v);
                                break;
                            case 4:
                                horizontalInputPlayer1 = Mathf.Min(0f, h);
                                verticalInputPlayer1 = Mathf.Max(0f, v);
                                break;
                            default:
                                horizontalInputPlayer1 = h;
                                verticalInputPlayer1 = 0f;
                                break;
                        }
                    }
                    else if (tag == "player2")
                    {
                        // **동시 입력 방지**
                        if (h != 0f && v != 0f)
                        {
                            // 수직 입력이 우선 적용됨
                            h = 0f;
                        }

                        switch (S2sm.mirrorCount)
                        {
                            case 0:
                                horizontalInputPlayer2 = h;
                                verticalInputPlayer2 = 0f;
                                break;
                            case 1:
                                horizontalInputPlayer2 = Mathf.Min(0f, h);
                                verticalInputPlayer2 = Mathf.Min(0f, v);
                                break;
                            case 2:
                                horizontalInputPlayer2 = Mathf.Min(0f, h);
                                verticalInputPlayer2 = Mathf.Max(0f, v);
                                break;
                            case 3:
                                horizontalInputPlayer2 = Mathf.Max(0f, h);
                                verticalInputPlayer2 = Mathf.Max(0f, v);
                                break;
                            case 4:
                                horizontalInputPlayer2 = Mathf.Max(0f, h);
                                verticalInputPlayer2 = Mathf.Min(0f, v);
                                break;
                            default:
                                horizontalInputPlayer2 = 0;
                                verticalInputPlayer2 = v;
                                break;
                        }
                    }
                }
            }
        }

    }
    private void ActivateBlind()
    {
        if (blindPanel != null && !isBlindActive)
        {
            blindPanel.SetActive(true); // **블라인드 패널 활성화**
            Debug.Log("블라인드 활성화 (내가 player1임)");
        }
        else if (blindPanel == null)
        {
            Debug.LogWarning("blindPanel이 연결되지 않았습니다.");
        }
    }

    private void DeactivateBlind()
    {
        if (blindPanel != null && isBlindActive)
        {
            blindPanel.SetActive(false); // **블라인드 패널 비활성화**
            Debug.Log("블라인드 비활성화 (내가 player2임)");
        }
        else if (blindPanel == null)
        {
            Debug.LogWarning("blindPanel이 연결되지 않았습니다.");
        }
    }
    void FixedUpdate()
    {
        if (isFalling) return;

        Vector2 moveVec;
        if (SceneManager.GetActiveScene().name == "Scene1")
        {
            moveVec = new Vector2(horizontalInput, verticalInput) * speed;
        }
        else if (SceneManager.GetActiveScene().name == "Scene2")
        {
            moveVec = new Vector2(horizontalInputPlayer1 + horizontalInputPlayer2, verticalInputPlayer1 + verticalInputPlayer2) * speed;
        }
        else
        {
            moveVec = Vector2.zero;
        }

        rigid.velocity = moveVec;

        anim.SetInteger("hAxisRaw", (int)moveVec.x);
        anim.SetInteger("vAxisRaw", (int)moveVec.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "DeathZone" && !isFalling)
        {
            StartCoroutine(FallAndRespawn());
        }

        if (other.gameObject.tag == "Goal")
        {
            gm.isMission1Clear = true;
            Debug.Log("Mission1 Clear");
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("MainScene");
            }
        }

        if (other.gameObject.tag == "MirrorPiece")
        {
            S2sm.mirrorCount++;
            Debug.Log("MirrorPiece 개수 : " + S2sm.mirrorCount);

            Destroy(other.gameObject);
        }

        if(other.gameObject.tag == "Mirror")
        {
            gm.isMission2Clear = true;
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("MainScene");
            }
        }
    }

    System.Collections.IEnumerator FallAndRespawn()
    {
        isFalling = true;
        rigid.velocity = Vector2.zero; 

        float fallDuration = 0.5f;
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;

        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(initialPosition, initialPosition + Vector3.down * 2, elapsedTime / fallDuration);
            transform.Rotate(Vector3.forward * 360 * Time.deltaTime);

            yield return null;
        }

        transform.position = Vector2.zero;
        transform.rotation = Quaternion.identity;

        isFalling = false;
    }

    new void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    new void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
