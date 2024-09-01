using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S1PlayerScript : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    // �÷��̾� �̵��ӵ�
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

        // �ִϸ��̼� ����
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

        // �����¿� �̵� 
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
        rigid.velocity = moveVec * speed;
    }

    // DeathZone ���� �� ����ġ �̵�
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
        rigid.velocity = Vector2.zero;  // �̵� ����

        // �ʱ� ����
        float fallDuration = 0.5f;  // �������� �ð�
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position; // ���� ��ġ ����

        while (elapsedTime < fallDuration)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(initialPosition, initialPosition + Vector3.down * 2, elapsedTime / fallDuration);
            transform.Rotate(Vector3.forward * 360 * Time.deltaTime);

            yield return null;
        }

        // ����ġ�� ����
        transform.position = Vector2.zero;
        transform.rotation = Quaternion.identity;

        isFalling = false;
    }
}

