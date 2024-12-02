using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S3PlayerScript : MonoBehaviour
{
    [SerializeField]
    private float speed;

    float h;
    float v;

    bool isHorizonMove;
    bool isVerticalMove;

    Rigidbody2D rd;
    Animator anim;
    public PhotonView pv;
    //public Text nicknameText;

    void Awake()
    {
        rd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        //nicknameText.text = pv.IsMine ? PhotonNetwork.NickName : pv.Owner.NickName;
        //nicknameText.color = pv.IsMine ? Color.green : Color.red;
    }

    void Update()
    {


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

    void FixedUpdate()
    {
    
        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
        rd.velocity = moveVec * speed;
        //Ray
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}