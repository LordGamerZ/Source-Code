using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

//Written by Abdul Galeel Ali

public class CreateJoinLobby : MonoBehaviour
{
    public static CreateJoinLobby Instance;

    public GameObject JoinMenu;

    public TMP_InputField CreateTitle;
    public TMP_InputField CreatePassword;

    public TMP_InputField JoinPassword;
    public int JoinLobbyNum;

    public Transform TitleColumn;
    public Transform HostNameColumn;
    public Transform NumPlayersColumn;
    public Transform HasPasswordColumn;
    public Transform JoinButtonColumn;

    public GameObject TextPrefab;
    public GameObject TogglePrefab;
    public GameObject ButtonPrefab;

    public TextMeshProUGUI MessageBox;

    private void Start()
    {
        Instance = this;

        DisplayLobbies();
    }

    public void DisplayMessage(string message)
    {
        MessageBox.text = message;
        MessageBox.GetComponent<MessageReset>().enabled = true;
    }

    public void DisplayLobbies()
    {
        for (int i = TitleColumn.transform.childCount - 1; i > -1; i--)
        {
            Destroy(TitleColumn.transform.GetChild(i).gameObject);
        }

        for (int i = HostNameColumn.transform.childCount - 1; i > -1; i--)
        {
            Destroy(HostNameColumn.transform.GetChild(i).gameObject);
        }


        for (int i = NumPlayersColumn.transform.childCount - 1; i > -1; i--)
        {
            Destroy(NumPlayersColumn.transform.GetChild(i).gameObject);
        }


        for (int i = HasPasswordColumn.transform.childCount - 1; i > -1; i--)
        {
            Destroy(HasPasswordColumn.transform.GetChild(i).gameObject);
        }


        for (int i = JoinButtonColumn.transform.childCount - 1; i > -1; i--)
        {
            Destroy(JoinButtonColumn.transform.GetChild(i).gameObject);
        }

        ClientSend.DisplayLobbies();
    }

    public void CreateLobbyDisplay(int lobbyNum, string title, string hostName, string numPlayers, bool hasPassword)
    {
        Instantiate(TextPrefab, TitleColumn).GetComponent<TextMeshProUGUI>().text = title;
        Instantiate(TextPrefab, HostNameColumn).GetComponent<TextMeshProUGUI>().text = hostName;
        Instantiate(TextPrefab, NumPlayersColumn).GetComponent<TextMeshProUGUI>().text = numPlayers;
        Instantiate(TogglePrefab, HasPasswordColumn).GetComponent<Toggle>().isOn = hasPassword;
        GameObject button = Instantiate(ButtonPrefab, JoinButtonColumn);
        button.GetComponent<Button>().onClick.AddListener(delegate { AttemptJoinLobby(lobbyNum, button.transform.GetSiblingIndex()); });

        LayoutRebuilder.ForceRebuildLayoutImmediate(TitleColumn.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(HostNameColumn.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(NumPlayersColumn.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(HasPasswordColumn.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(JoinButtonColumn.GetComponent<RectTransform>());
    }

    public void CreateJobby()
    {
        if (CreateTitle.text != "")
        {
            JoinMenu.SetActive(false);
            ClientSend.CreateLobby(CreateTitle.text, CreatePassword.text);
        }
    }

    public void AttemptJoinLobby(int lobbyNum, int whichTickBox)
    {
        if (HasPasswordColumn.GetChild(whichTickBox).GetComponent<Toggle>().isOn)
        {
            DisplayJoinPassword();
            JoinLobbyNum = lobbyNum;
        }
        else
        {
            JoinLobby(lobbyNum);
        }
    }

    public void DisplayJoinPassword()
    {
        JoinPassword.text = "";
        JoinMenu.SetActive(true);
    }

    public void CloseJoinPassword()
    {
        JoinPassword.text = "";
        JoinMenu.SetActive(false);
    }

    public void EnterPassword()
    {
        JoinLobby(JoinLobbyNum);
        CloseJoinPassword();
    }

    public void JoinLobby(int lobbyNum)
    {
        JoinMenu.SetActive(false);
        ClientSend.JoinLobby(lobbyNum, JoinPassword.text);
    }

    public void ToMainMenu()
    {
        JoinMenu.SetActive(false);
        Client.Instance.LoadScene("MainMenu");
    }
}
