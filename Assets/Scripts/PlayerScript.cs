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

    float h;
    float v;

    bool isHorizonMove;
    bool isVerticalMove;

    bool hasSpawned = false; // �¾ ��ġ�� �����Ǿ����� Ȯ��

    string playerTag;

    Rigidbody2D rd;
    Animator anim;
    public PhotonView pv;
    public Text nicknameText;

    void Awake()
    {
        rd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        DontDestroyOnLoad(this.gameObject);

        nicknameText.text = pv.IsMine ? PhotonNetwork.NickName : pv.Owner.NickName;
        nicknameText.color = pv.IsMine ? Color.green : Color.red;
    }

    void Update()
    {
        if (pv.IsMine) // �ڽ��� �÷��̾ ����
        {
            // Scene1�� ��
            if (SceneManager.GetActiveScene().name == "Scene1")
            {
                // �÷��̾ ȭ�� ������ �̵����Ѽ� ������ �ʰ� ��
                transform.position = new Vector3(10000f, 10000f, 10000f);
            }

            // MainScene�� ��
            else if (SceneManager.GetActiveScene().name == "MainScene" && !hasSpawned)
            {
                transform.position = new Vector3(0f, 0f, 0f); // �¾�� ��ġ
                hasSpawned = true; // �ʱ� ��ġ ���� �Ϸ�
            }

            // �±� ����
            playerTag = gameObject.CompareTag("player1") ? "player1" : "player2";
            ExitGames.Client.Photon.Hashtable playerInput = new ExitGames.Client.Photon.Hashtable
            {
                { "Horizontal", h },
                { "Vertical", v },
                { "Tag", playerTag }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerInput);

            // �Է°� ó��
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            bool hDown = Input.GetButtonDown("Horizontal");
            bool vDown = Input.GetButtonDown("Vertical");
            bool hUp = Input.GetButtonUp("Horizontal");
            bool vUp = Input.GetButtonUp("Vertical");

            if (hDown)
                isHorizonMove = true;
            else if (vDown)
                isHorizonMove = false;
            else if (vUp || hUp)
                isHorizonMove = h != 0;

            // �ִϸ��̼� ���� ó��
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
        if (pv.IsMine) // �ڽ��� �÷��̾ ���� ������Ʈ
        {
            Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
            rd.velocity = moveVec * speed;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Photon ���� ����ȭ�� (�ʿ� �� ����)
    }
}