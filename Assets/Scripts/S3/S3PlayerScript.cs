using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S3PlayerScript : MonoBehaviour
{
    [SerializeField]
    private float speed;
    public S3_1GameManager manager;

    float h;
    float v;

    bool isHorizonMove;
    bool isVerticalMove;

    Rigidbody2D rd;
    Animator anim;
    public PhotonView pv;
    //public Text nicknameText;

    Vector3 dirVec;//바라보고 있는 방향
    GameObject scanObject;
    GameObject portalObject;

    void Awake()
    {
        rd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //nicknameText.text = pv.IsMine ? PhotonNetwork.NickName : pv.Owner.NickName;
        //nicknameText.color = pv.IsMine ? Color.green : Color.red;
    }

    void Update()
    {


        h = manager.isAction ? 0 : Input.GetAxisRaw("Horizontal");
        v = manager.isAction ? 0 : Input.GetAxisRaw("Vertical");

        //Check Button Up and Down
        bool hDown = manager.isAction ? false : Input.GetButtonDown("Horizontal");
        bool vDown = manager.isAction ? false : Input.GetButtonDown("Vertical");
        bool hUp = manager.isAction ? false : Input.GetButtonUp("Horizontal");
        bool vUp = manager.isAction ? false : Input.GetButtonUp("Vertical");

        if (hDown)
            isHorizonMove = true;
        else if (vDown)
            isHorizonMove = false;
        else if (vUp || hUp)
            isHorizonMove = h != 0;

        UpdateAnimation(h, v);
        SetDirection(hDown, vDown);
        //scan
        /*
        if (Input.GetKeyDown("Jump") && portalObject != null)
        {
            Debug.Log("Portal activated! Switching scenes...");
            SceneManager.LoadScene("scene3-1"); // 전환할 씬 이름
        }*/

        if (Input.GetButtonDown("Jump") && scanObject != null)
        {
            Debug.Log(scanObject.name);
            manager.Action(scanObject);
        }
    }

    void FixedUpdate()
    {

        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
        rd.velocity = moveVec * speed;
        //Ray
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        // Portal 오브젝트와 충돌 시 씬 전환
        if (collision.CompareTag("Portal"))
        {
            Debug.Log("Portal entered! Switching scenes...");
            SceneManager.LoadScene("Scene3-1"); // 전환할 씬 이름
        }*/
        // S3PlayerScript의 로직 유지
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
    /*
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Portal 오브젝트에서 벗어나면 참조 해제
        if (collision.CompareTag("Portal"))
        {
            portalObject = null; // Portal 오브젝트 참조 제거
            Debug.Log("Player left portal");
        }
    }*/
    private void UpdateAnimation(float h, float v)
    {
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
        {
            anim.SetBool("isChange", false);
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