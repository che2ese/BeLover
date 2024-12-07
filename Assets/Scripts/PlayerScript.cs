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

            // Scene1일 때
            if (SceneManager.GetActiveScene().name == "Scene1" || SceneManager.GetActiveScene().name == "Scene2")
            {
                // 플레이어를 화면 밖으로 이동시켜서 보이지 않게 함
                transform.position = new Vector3(10000f, 10000f, 10000f);
            }
            else if(SceneManager.GetActiveScene().name == "Scene3-1")
            {
                transform.position = new Vector3(-0.5f, -2f, 0f);
                SetDirection(hDown, vDown);
                if (Input.GetButtonDown("Jump") && scanObject != null)
                {
                    Debug.Log(scanObject.name);
                    S3sm.Action(scanObject);
                }
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
        if (pv.IsMine) // 자신의 플레이어만 물리 업데이트
        {
            Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
            rd.velocity = moveVec * speed;

            //s3 ray
            Debug.DrawRay(rd.position, dirVec * 0.7f, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rd.position, dirVec, 0.35f,
                LayerMask.GetMask("Object")); //Layer에서 object 인것을 조사할 수 잇도록 한다.
            if (rayHit.collider != null)
            {
                scanObject = rayHit.collider.gameObject;
            }
            else
            {
                scanObject = null;
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
    private void SetDirection(bool hDown, bool vDown)
    {
        if (vDown && v == 1) dirVec = Vector3.up;
        else if (vDown && v == -1) dirVec = Vector3.down;
        else if (hDown && h == -1) dirVec = Vector3.left;
        else if (hDown && h == 1) dirVec = Vector3.right;
    }

}