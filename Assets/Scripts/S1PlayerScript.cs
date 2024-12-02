using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

public class S1PlayerScript : MonoBehaviourPunCallbacks
{

    public float speed;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    private float horizontalInput = 0f; // 수평 입력값
    private float verticalInput = 0f;   // 수직 입력값
    bool isFalling = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {

            float h = (float)player.CustomProperties["Horizontal"];
            float v = (float)player.CustomProperties["Vertical"];
            string tag = (string)player.CustomProperties["Tag"];

            // 플레이어 값 전송 확인
            // Debug.Log($"{tag} : Input - Horizontal: {h}, Vertical: {v}");

            if (tag == "player1")
            {
                if (h != 0f && v == 0f)  // 수평 입력만 있을 때
                {
                    horizontalInput = h;
                    verticalInput = 0f;  // 수직 입력을 0으로
                }
                else if (v != 0f && h == 0f)  // 수직 입력만 있을 때
                {
                    verticalInput = v;
                    horizontalInput = 0f;  // 수평 입력을 0으로
                }
                else if (h == 0f && v == 0f)  // 아무 입력도 없을 때 (가만히 있을 때)
                {
                    horizontalInput = 0f;
                    verticalInput = 0f;  // 가만히 있을 때는 입력을 0으로
                }
            }
        }

    }

    void FixedUpdate()
    {
        if (isFalling) return;

        Vector2 moveVec = new Vector2(horizontalInput, verticalInput) * speed;
        rigid.velocity = moveVec;

        anim.SetInteger("hAxisRaw", (int)moveVec.x);
        anim.SetInteger("vAxisRaw", (int)moveVec.y);
    }


    // DeathZone ???? ?? ?????? ????
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "DeathZone" && !isFalling)
        {
            StartCoroutine(FallAndRespawn());
        }

    }

    System.Collections.IEnumerator FallAndRespawn()
    {
        isFalling = true;
        rigid.velocity = Vector2.zero;  // ???? ????

        // ???? ????
        float fallDuration = 0.5f;  // ???????? ????
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position; // ???? ???? ????

        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(initialPosition, initialPosition + Vector3.down * 2, elapsedTime / fallDuration);
            transform.Rotate(Vector3.forward * 360 * Time.deltaTime);

            yield return null;
        }

        // ???????? ????
        transform.position = Vector2.zero;
        transform.rotation = Quaternion.identity;

        isFalling = false;
    }
}

