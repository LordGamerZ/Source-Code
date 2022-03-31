using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Written by Abdul Galeel Ali

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;
    public int MaxPlayers = 4;

    //Player Display Variables
    public Transform PlayersPanel;
    public GameObject PlayerDisplayPrefab;

    //Chat Variables
    public GameObject ChatBox;
    public GameObject ChatPrefab;

    public InputField ChatInput;

    //Launch Match Variables
    public Button LaunchButton;

    public TMP_Dropdown BoardSizes;
    public int[] Sizes;

    public TMP_Dropdown TurnTimes;
    public int[] Times;

    public CreateGrid GridCreator;

    private void Awake()
    {
        if (Client.Instance.IsHost)
        {
            Sizes = new int[4] { 25, 50, 75, 100 };
            List<TMP_Dropdown.OptionData> boardOptions = new List<TMP_Dropdown.OptionData>
            {
                new TMP_Dropdown.OptionData("Small (25 x 25)"),
                new TMP_Dropdown.OptionData("Medium (50 x 50)"),
                new TMP_Dropdown.OptionData("Large (75 x 75"),
                new TMP_Dropdown.OptionData("Too Big (100 x 100")
            };
            BoardSizes.AddOptions(boardOptions);
            BoardSizes.value = 1;

            Times = new int[4] { 30, 60, 90, 120 };
            List<TMP_Dropdown.OptionData> timeOptions = new List<TMP_Dropdown.OptionData>
            {
                new TMP_Dropdown.OptionData("30 seconds"),
                new TMP_Dropdown.OptionData("60 seconds"),
                new TMP_Dropdown.OptionData("90 seconds"),
                new TMP_Dropdown.OptionData("120 seconds"),
            };
            TurnTimes.AddOptions(timeOptions);
            BoardSizes.value = 1;
        }
        else
        {
            LaunchButton.gameObject.SetActive(false);
            BoardSizes.gameObject.SetActive(false);
            TurnTimes.gameObject.SetActive(false);

            ClientSend.OtherDisplays();
        }

        ClientSend.CreatePlayerDisplay();

        Instance = this;
    }

    public void BeginMatch()
    {
        if (PlayersPanel.transform.childCount > 0)
        {
            int[] ids = new int[PlayersPanel.transform.childCount];
            int[] teamNums = new int[PlayersPanel.transform.childCount];

            for (int i = 0; i < PlayersPanel.transform.childCount; i++)
            {
                PlayerDisplay playerDisplay = PlayersPanel.GetChild(i).GetComponent<PlayerDisplay>();

                ids[i] = playerDisplay.PlayerID;
                teamNums[i] = playerDisplay.GetTeamNum();
            }

            ClientSend.StartMatch(Sizes[BoardSizes.value], ids, teamNums, Times[TurnTimes.value]);
        }
    }

    public void CreateChat(string username, string msg)
    {
        Instantiate(ChatPrefab, ChatBox.transform).GetComponent<TextMeshProUGUI>().text = username + ": " + msg;
        LayoutRebuilder.ForceRebuildLayoutImmediate(ChatBox.GetComponent<RectTransform>());
    }

    public void SendChat()
    {
        if (ChatInput.text != "")
        {
            ClientSend.SendChat(ChatInput.text);
            ChatInput.text = "";
        }
    }

    public void CreatePlayerDisplay(int clientID, string username, int teamNum = 1)
    {
        PlayerDisplay playerDisplay = Instantiate(PlayerDisplayPrefab, PlayersPanel.transform).GetComponent<PlayerDisplay>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(PlayersPanel.GetComponent<RectTransform>());

        playerDisplay.SetUpDisplay(clientID, username, teamNum);
    }

    public void ChangePlayerDisplay(int clientID, int teamNum)
    {
        foreach (Transform playerDisplay in PlayersPanel.transform)
        {
            if (clientID == playerDisplay.GetComponent<PlayerDisplay>().PlayerID)
            {
                playerDisplay.GetComponent<PlayerDisplay>().TeamNum.text = teamNum.ToString();
                break;
            }
        }
    }

    public void DeletePlayerDisplay(int clientID)
    {
        foreach (Transform playerDisplay in PlayersPanel.transform)
        {
            if (clientID == playerDisplay.GetComponent<PlayerDisplay>().PlayerID)
            {
                Destroy(playerDisplay.gameObject);
            }
        }
    }

    public void LeaveMatch()
    {
        Destroy(PlayerInteraction.Instance.gameObject);
        Destroy(UIControl.Instance.gameObject);
        ClientSend.LeaveLobby();

        Client.Instance.LoadScene("CreateJoinLobby");
    }
}