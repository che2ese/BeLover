using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S1PlayerScript : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    // 플레이어 이동속도
    public float speed;

    bool isFalling = false;

    float h;
    float v;

    bool isHorizonMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isFalling) return;

        bool hDown = Input.GetButtonDown("Horizontal");
        bool vDown = Input.GetButtonDown("Vertical");
        bool hUp = Input.GetButtonUp("Horizontal");
        bool vUp = Input.GetButtonUp("Vertical");

        if (hDown || vUp)
        {
            isHorizonMove = true;
        }
        else if (vDown || hUp)
        {
            isHorizonMove = false;
        }

        // 애니메이션 설정
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

    void FixedUpdate()
    {
        if (isFalling) return;

        // 상하좌우 이동 
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
        rigid.velocity = moveVec * speed;
    }

    // DeathZone 감지 및 원위치 이동
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "DeathZone" && !isFalling)
        {
            StartCoroutine(FallAndRespawn());
        }

    }

    IEnumerator FallAndRespawn()
    {
        isFalling = true;
        rigid.velocity = Vector2.zero;  // 이동 중지

        // 초기 설정
        float fallDuration = 0.5f;  // 떨어지는 시간
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position; // 원래 위치 저장

        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(initialPosition, initialPosition + Vector3.down * 2, elapsedTime / fallDuration);
            transform.Rotate(Vector3.forward * 360 * Time.deltaTime);

            yield return null;
        }

        // 원위치로 복귀
        transform.position = Vector2.zero;
        transform.rotation = Quaternion.identity;

        isFalling = false;
    }
}

