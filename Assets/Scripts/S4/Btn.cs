using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Btn : MonoBehaviour
{
    public int buttonID; // 버튼 고유 ID (1부터 20까지)
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color pressedColor = new Color(0.5f, 0.5f, 0.5f);
    public Color incorrectColor = new Color(0.6f, 0.2f, 0.2f);

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // 원래 색 저장
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player1")) // 플레이어가 밟았을 때
        {
            Debug.Log($"Player stepped on Button {buttonID}");
            S4Manager manager = FindObjectOfType<S4Manager>();

            if (manager.OnButtonPressed(buttonID))
            {
                spriteRenderer.color = pressedColor;
            }
            else
            {
                spriteRenderer.color = incorrectColor;
            }
        }
    }

    public void ResetButton()
    {
        spriteRenderer.color = originalColor; // 원래 색으로 복원
    }
}
