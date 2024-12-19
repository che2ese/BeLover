using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOnOff : MonoBehaviour
{
    public int buttonID; // ��ư ���� ID (1���� 20����)
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color pressedColor = new Color(0.5f, 0.5f, 0.5f);

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // ���� �� ����
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player1")) // �÷��̾ ����� ��
        {
            Debug.Log($"Player stepped on Button {buttonID}");
            spriteRenderer.color = pressedColor;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("player1"))
        {
            spriteRenderer.color = originalColor; // ���� ������ ����
        }
    }
}
