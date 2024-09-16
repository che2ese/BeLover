using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S1PlayerScript : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    // ???????? ????????
    public float speed;

    bool isFalling = false;


    float h;
    float v;
    Vector2 moveVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isFalling) return;

        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        Vector2 inputVec = new Vector2(h, v);

        if (inputVec.magnitude > 1)
            return;

        moveVec.x = inputVec.x;
        moveVec.y = inputVec.y;

        bool hButton = Input.GetButton("Horizontal");
        bool vButton = Input.GetButton("Vertical");

    }

    void FixedUpdate()
    {
        if (isFalling) return;

        rigid.MovePosition(rigid.position + moveVec * Time.deltaTime * speed);
    }

    void LateUpdate()
    {
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

    IEnumerator FallAndRespawn()
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

