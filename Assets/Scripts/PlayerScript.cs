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
    GameObject scanObject;
    GameObject portalObject;

    //s4
    private bool canReset = false;

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
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            bool hDown = Input.GetButtonDown("Horizontal");
            bool vDown = Input.GetButtonDown("Vertical");
            bool hUp = Input.GetButtonUp("Horizontal");
            bool vUp = Input.GetButtonUp("Vertical");

            playerTag = gameObject.CompareTag("player1") ? "player1" : "player2";
            ExitGames.Client.Photon.Hashtable playerInput = new ExitGames.Client.Photon.Hashtable
            {
                { "Horizontal", h },
                { "Vertical", v },
                { "Tag", playerTag }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerInput);

            if (SceneManager.GetActiveScene().name == "Scene1" || SceneManager.GetActiveScene().name == "Scene2")
            {
                transform.position = new Vector3(10000f, 10000f, 10000f);
            }
            else if (SceneManager.GetActiveScene().name == "Scene3-1" || SceneManager.GetActiveScene().name == "MainScene")
            {
                SetDirection(hDown, vDown);
                if (Input.GetButtonDown("Jump") && scanObject != null)
                {
                    if(SceneManager.GetActiveScene().name == "Scene3-1")
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
                Vector2 rayStartPos = rd.position + new Vector2(0.2f, -0.1f);

                Debug.DrawRay(rayStartPos, dirVec * 0.5f, Color.red);

                RaycastHit2D rayHit = Physics2D.Raycast(
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal"))
        {
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
        if (CompareTag("player1") && objectName.StartsWith("Button"))
        {
            S3SceneManager.Instance.SetP1ObjectName(objectName);
        }
        else if (CompareTag("player2") && objectName.StartsWith("Road"))
        {
            if (currentRoad != collision.gameObject)
            {
                S3SceneManager.Instance.SetP1ObjectName(objectName);
                S3SceneManager.Instance.SetP2ObjectName(objectName);
            }

            if (!S3sm.IsButtonInteracted())
            {
                SetSpeedRPC(0.5f);
            }
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
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (pv.IsMine)
        {
        }
    }

    new void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pv.IsMine)
        {
        }
        if (scene.name == "MainScene")
        {
            SetPosition(0f, 0f, 0f);
        }
        else if (scene.name == "Scene3-1")
        {
            S3sm = FindObjectOfType<S3SceneManager>();
            if (S3sm != null)
            {
            }
            else
            {
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
            }
        }
        else if (scene.name == "Scene4")
        {
            if(playerTag == "player1")
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
            S3sm = null;
        }
    }
    private void SetPosition(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }
    private void AttachCameraToPlayer()
    {
        S3Camera mainCamera = Camera.main.GetComponent<S3Camera>();
        if (mainCamera != null)
        {
            mainCamera.SetTarget(transform);
        }
        else
        {
        }
    }



    [PunRPC]
    public void SetSpeedRPC(float newSpeed)
    {

        {
            speed = newSpeed;
        }
        else
        {
        }
    }
}