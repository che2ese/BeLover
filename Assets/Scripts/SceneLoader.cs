using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private NetworkManager NM;
    void Awake()
    {
        NM = FindObjectOfType<NetworkManager>();
    }
    void Update()
    {
        if (NM != null && NM.isGameStart)
        {
            SceneManager.LoadScene("MainScene");  // ��ȯ�� �� �̸����� ����
        }
    }
}
