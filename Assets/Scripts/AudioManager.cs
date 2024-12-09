using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // **�̱��� �ν��Ͻ�**
    private AudioSource audioSource; // **AudioSource ������Ʈ**
    private Coroutine fadeCoroutine; // **���̵� ��/�ƿ� �ڷ�ƾ ����**

    [Header("Scene-Specific BGM Clips")]
    public AudioClip StartSceneBGM;
    public AudioClip mainSceneBGM;
    public AudioClip scene1BGM;
    public AudioClip scene2BGM;
    public AudioClip scene3_1BGM;
    public AudioClip scene3BGM;
    public AudioClip scene4BGM;
    public AudioClip scene5BGM;

    private int mirrorCount = 0; // **mirrorCount ����**
    private int previousMirrorCount = 0; // **���� mirrorCount ��**

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;
        audioSource.mute = false; // ���Ұ� ����

        // **���� ���� BGM�� ���**
        Debug.Log($"���� ��: {SceneManager.GetActiveScene().name}");
        PlayBGMForScene(SceneManager.GetActiveScene().name); // **���� ���� �´� BGM ���**
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene Loaded: {scene.name}");
        PlayBGMForScene(scene.name);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Scene2")
        {
            S2SceneManager s2 = FindObjectOfType<S2SceneManager>();
            if (s2 != null)
            {
                mirrorCount = s2.mirrorCount;

                if (mirrorCount > previousMirrorCount)
                {
                    Debug.Log($"mirrorCount�� {previousMirrorCount}���� {mirrorCount}�� �����߽��ϴ�.");
                    IncreaseBGMVolume(0.1f); // **BGM �Ҹ� 1.2�� ����**
                    previousMirrorCount = mirrorCount; // **���� �� ����**
                }
            }
        }
    }

    private void PlayBGMForScene(string sceneName)
    {
        Debug.Log($"BGM �÷��� ����: {sceneName}"); // **����� �α� �߰�**
        AudioClip clipToPlay = null;

        switch (sceneName)
        {
            case "StartScene": // **LobbyScene �߰�**
                clipToPlay = StartSceneBGM; // **LobbyScene�� BGM���� StartSceneBGM ���**
                break;
            case "LobbyScene": // **LobbyScene �߰�**
                clipToPlay = StartSceneBGM; // **LobbyScene�� BGM���� StartSceneBGM ���**
                break;
            case "MenuScene":
                clipToPlay = StartSceneBGM;
                break;
            case "MainScene":
                clipToPlay = mainSceneBGM;
                break;
            case "Scene1":
                clipToPlay = scene1BGM;
                break;
            case "Scene2":
                clipToPlay = scene2BGM;
                break;
            case "Scene3-1":
                clipToPlay = scene3_1BGM;
                break;
            case "Scene3":
                clipToPlay = scene3BGM;
                break;
            case "Scene4":
                clipToPlay = scene4BGM;
                break;
            case "Scene5":
                clipToPlay = scene5BGM;
                break;
            default:
                Debug.LogWarning($"BGM not assigned for scene: {sceneName}");
                return;
        }

        if (clipToPlay != null)
        {
            PlayBGMWithFade(clipToPlay, 1f);
        }
    }


    public void PlayBGM(AudioClip clip)
    {
        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayBGMWithFade(AudioClip clip, float fadeDuration = 1f)
    {
        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInBGM(clip, fadeDuration));
    }

    private IEnumerator FadeInBGM(AudioClip newClip, float duration)
    {
        if (audioSource.isPlaying)
        {
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(0.5f, 0f, t / duration);
                yield return null;
            }
            audioSource.Stop();
        }

        audioSource.clip = newClip;
        audioSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, 0.5f, t / duration);
            yield return null;
        }
    }

    public void PauseBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void ResumeBGM()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }

    public void IncreaseBGMVolume(float add)
    {
        float newVolume = audioSource.volume + add;
        audioSource.volume = Mathf.Clamp(newVolume, 0f, 1f);
        Debug.Log($"New BGM Volume: {audioSource.volume}");
    }
}
