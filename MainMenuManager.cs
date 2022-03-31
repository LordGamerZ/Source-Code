using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

//Written by Abdul Galeel Ali

public class MainMenuManager : MonoBehaviour
{
    public GameObject StartMenu;
    public GameObject GuestPanel;
    public GameObject ConnectingPanel;
    public TMP_InputField GuestUsernameInput;

    public float ChangeTime;
    private float Timer;

    public Material RainbowMat;

    private void Start()
    {
        StartMenu.SetActive(true);
        GuestPanel.SetActive(false);
        ConnectingPanel.SetActive(false);
        Timer = 0;
        RainbowMat.color = new Color(Random.value, Random.value, Random.value, 255);
    }

    private void Update()
    {
        Timer += Time.deltaTime;
        if(Timer > ChangeTime)
        {
            RainbowMat.color = new Color(Random.value, Random.value, Random.value, 255);

            Timer = 0;
        }
    }

    public void GuestConnect()
    {
        if (GuestUsernameInput.text != "")
        {
            StartMenu.SetActive(false);
            GuestPanel.SetActive(false);
            ConnectingPanel.SetActive(true);

            Client.Instance.Username = GuestUsernameInput.text;
            Client.Instance.Connect();
        }
    }

    public void OpenGuestPanel()
    {
        GuestUsernameInput.text = "";
        GuestPanel.SetActive(true);
        StartMenu.SetActive(false);
    }

    public void CloseGuestPanel()
    {
        GuestUsernameInput.text = "";
        GuestPanel.SetActive(false);
        StartMenu.SetActive(true);
    }

    public void CancelConnect()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}