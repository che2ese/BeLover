using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // �̱��� �ν��Ͻ�

    // �� ���� Ƚ���� ����ϴ� ����
    public int mainSceneEnterCount = 0;
    public bool isMission1Clear = false;
    public bool isMission2Clear = false;
    public bool isMission3Clear = false;

    private Button settingButton; // SettingBtn ����
    private GameObject settingUI; // SettingUI ����
    private Button resumeBtn;
    private Button gamesettingBtn;
    private Button finishBtn;

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
        }

        // ���� �ε�� �� SettingBtn�� SettingUI Ž��
        FindSettingComponents();

        // SettingBtn Ŭ�� �̺�Ʈ ���
        if (settingButton != null)
        {
            settingButton.onClick.RemoveAllListeners(); // ���� ������ ����
            settingButton.onClick.AddListener(ToggleSettingUI);
        }
        if (resumeBtn != null)
        {
            resumeBtn.onClick.RemoveAllListeners(); // ���� ������ ����
            resumeBtn.onClick.AddListener(GameResume);
        }
        if (gamesettingBtn != null)
        {
            gamesettingBtn.onClick.RemoveAllListeners(); // ���� ������ ����
            gamesettingBtn.onClick.AddListener(OpenSettingsMenu);
        }
        if (finishBtn != null)
        {
            finishBtn.onClick.RemoveAllListeners(); // ���� ������ ����
            finishBtn.onClick.AddListener(FinishGame);
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

    // Setting ��ư�� UI�� ã�� �޼���
    private void FindSettingComponents()
    {
        // ���� ������ SettingBtn�� SettingUI ã��
        settingButton = GameObject.Find("SettingBtn")?.GetComponent<Button>();
        settingUI = GameObject.Find("SettingUI");
        resumeBtn = GameObject.Find("ResumeBtn")?.GetComponent<Button>();
        gamesettingBtn = GameObject.Find("GameSetBtn")?.GetComponent<Button>();
        finishBtn = GameObject.Find("FinishBtn")?.GetComponent<Button>();

        if (settingUI != null)
        {
            // SettingUI�� ó������ ��Ȱ��ȭ
            settingUI.SetActive(false);
        }
    }

    // SettingUI�� ����ϴ� �޼���
    private void ToggleSettingUI()
    {
        if (settingUI != null)
        {
            settingUI.SetActive(!settingUI.activeSelf);
        }
    }
    private void GameResume()
    {
        settingUI.SetActive(false);
    }
    private void OpenSettingsMenu()
    {
        Debug.Log("Settings �޴� ����");
    }

    private void FinishGame()
    {
        Debug.Log("���� ����");
        Application.Quit(); // ���� ����
    }
}
