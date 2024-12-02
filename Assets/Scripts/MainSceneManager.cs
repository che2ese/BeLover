using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;

public class MainSceneManager : MonoBehaviour
{
    NetworkManager nm;
    public GameObject storyPanel;

    void Awake()
    {

        nm = FindObjectOfType<NetworkManager>();
    }

    public void SpawnChar()
    {
        storyPanel.SetActive(false);
        nm.Spawn();
    }
}
