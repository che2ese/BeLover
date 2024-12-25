using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // �̱��� �ν��Ͻ�

    // �� ���� Ƚ���� ����ϴ� ����
    public int mainSceneEnterCount = 0;
    public bool isMission1Clear = false;
    public bool isMission2Clear = false;
    public bool isMission3Clear = false;

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� GameManager ����
        }
    }

    // �� �ε�� �� ȣ��Ǵ� Unity�� ���� �޼���
    private void OnEnable()
    {
        // �� �ε� �̺�Ʈ�� �ڵ鷯 ���
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // �� �ε� �̺�Ʈ �ڵ鷯 ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ���� �ε�Ǿ��� �� ȣ��Ǵ� �Լ�
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            IncrementMainSceneEnterCount();
            //s3sm ����
            S3SceneManager s3Manager = FindObjectOfType<S3SceneManager>();
            if (s3Manager != null)
            {
                Destroy(s3Manager.gameObject); // MainScene������ S3SceneManager ����
                Debug.Log("MainScene���� S3SceneManager�� �����߽��ϴ�.");
            }
        }
    }

    // MainScene ���� Ƚ���� ������Ű�� �Լ�
    private void IncrementMainSceneEnterCount()
    {
        mainSceneEnterCount++;
        Debug.Log("MainScene�� ������ Ƚ��: " + mainSceneEnterCount);
    }

    // ���� MainScene ���� Ƚ���� ��ȯ�ϴ� �Լ�
    public int GetMainSceneEnterCount()
    {
        return mainSceneEnterCount;
    }
}
