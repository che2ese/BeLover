using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SceneLoader : MonoBehaviourPunCallbacks
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
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("MainScene");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
