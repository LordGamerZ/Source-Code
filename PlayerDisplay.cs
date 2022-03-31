using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Written by Abdul Galeel Ali

public class PlayerDisplay : MonoBehaviour
{
    public Image ProfileIcon;
    public int PlayerID;

    public TextMeshProUGUI Username;

    public TextMeshProUGUI TeamNum;
    public TMP_Dropdown MyTeamNum;

    public void SetUpDisplay(int userID, string username, int teamNum = 1, Sprite icon = null)
    {
        PlayerID = userID;
        ProfileIcon.sprite = icon;
        Username.text = username;

        if (Client.Instance.MyID == PlayerID)
        {
            MyTeamNum.gameObject.SetActive(true);
            TeamNum.gameObject.SetActive(false);
        }
        else
        {
            MyTeamNum.gameObject.SetActive(false);
            TeamNum.gameObject.SetActive(true);
            TeamNum.text = teamNum.ToString();
        }

        GameObject players = GameObject.Find("Players");
        transform.SetParent(players.transform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(players.GetComponent<RectTransform>());
    }

    public int GetTeamNum()
    {
        if(Client.Instance.MyID == PlayerID)
        {
            return int.Parse(MyTeamNum.options[MyTeamNum.value].text);
        }

        return int.Parse(TeamNum.text);
    }

    public void ChangeMyTeamNum()
    {
        ClientSend.ChangePlayerDisplay(int.Parse(MyTeamNum.options[MyTeamNum.value].text.ToString()));
    }
}
