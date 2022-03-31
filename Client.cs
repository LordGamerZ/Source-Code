using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using PacketModule;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//Written by Abdul Galeel Ali

public class Client : MonoBehaviour
{
    public static Client Instance;

    public bool IsHost;
    public string Username;

    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> PacketHandlers;

    //Connection
    public TcpClient Socket;

    private NetworkStream Stream;
    private Packet ReceivedData;
    private byte[] ReceiveBuffer;

    //Loading
    public GameObject LoadingScreen;
    public TextMeshProUGUI LoadingText;
    public ProgressBar Bar;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Application.runInBackground = true;
        Instance = this;
        LoadingScreen.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string scene)
    {
        LoadingText.text = "Loading";
        LoadingScreen.SetActive(true);
        StartCoroutine(SetSceneLoadProgress(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single)));
    }

    public IEnumerator SetSceneLoadProgress(AsyncOperation sceneLoading)
    {
        while (!sceneLoading.isDone)
        {
            Bar.ChangeFill(Mathf.RoundToInt(sceneLoading.progress));
            yield return null;
        }

        LoadingScreen.SetActive(false);
    }

    private void OnDestroy()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            LoadScene("MainMenu");
        }
    }  
}