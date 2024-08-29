using UnityEngine;
using UnityEngine.SceneManagement;
<<<<<<< HEAD
using Photon.Pun;

public class SceneLoader : MonoBehaviourPunCallbacks
{
    private NetworkManager NM;

=======

public class SceneLoader : MonoBehaviour
{
    private NetworkManager NM;
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
    void Awake()
    {
        NM = FindObjectOfType<NetworkManager>();
    }
<<<<<<< HEAD

=======
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
    void Update()
    {
        if (NM != null && NM.isGameStart)
        {
<<<<<<< HEAD
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("MainScene");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
=======
            SceneManager.LoadScene("MainScene");  // 전환할 씬 이름으로 변경
>>>>>>> d2af78701f8569556cde536fb3e01a4df1a74d41
        }
    }
}
