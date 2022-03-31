using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PacketModule;

//Written by Abdul Galeel Ali

public class Lobby : MonoBehaviour
{
    public Server MainServer;
    public int LobbyNum;

    public Dictionary<int, PlayerInfo> PlayerIDs;

    public string Title;
    public int HostID;
    public bool Joinable;
    public bool InGame;
    public string Password;

    public int TurnNum;
    public int CurrentTurn;
    public float TurnTimer;
    public float TurnTime;

    public int MaxPlayers = 4;

    public int MaxViewIDs = 20000;
    public ViewTypes[] ServerObjects;

    private void Awake()
    {
        ResetLobby();
    }

    private void Update()
    {
        if(InGame)
        {
            TurnTimer += Time.deltaTime;

            if (TurnTimer > TurnTime)
            {
                TurnTimer = 0;
                NextTurn();
            }
        }
    }

    public void ResetLobby()
    {
        TurnTime = 0;
        TurnTimer = 0;
        TurnNum = -1;
        CurrentTurn = -1;
        InGame = false;
        Joinable = false;
        Title = "";
        Password = "";
        HostID = 0;
        ServerObjects = null;
        PlayerIDs = null;
    }

    public void UserLeave(int clientID)
    {
        if (PlayerIDs.ContainsKey(clientID))
        {
            PlayerIDs.Remove(clientID);

            if (InGame && PlayerIDs.Count > 0)
            {
                Packet packet = new((int)ClientPackets.userLeave);
                packet.Write(clientID);

                MainServer.Sender.SendTCPDataAll(clientID, packet, LobbyNum);

                if(PlayerIDs.Count == 1)
                {
                    Packet winPacket = new((int)ClientPackets.win);

                    MainServer.Sender.SendTCPData(new List<int>(PlayerIDs.Keys)[0], winPacket);
                    ResetLobby();
                }
            }
            else if (!InGame && PlayerIDs.Count > 0)
            {
                Packet packet = new((int)ClientPackets.deleteDisplay);
                packet.Write(clientID);

                MainServer.Sender.SendTCPDataAll(clientID, packet, LobbyNum);

                if(!PlayerIDs.ContainsKey(HostID))
                {
                    Packet hostPacket = new((int)ClientPackets.setHost);
                    HostID = new List<int>(PlayerIDs.Keys)[0];

                    MainServer.Sender.SendTCPData(new List<int>(PlayerIDs.Keys)[0], hostPacket);
                }
            }
            else
            {
                ResetLobby();
            }
        }
    }

    public void StartLobby(Server mainServer, string title, int lobbyNum, int maxPlayers, int hostID)
    {
        PlayerIDs = new Dictionary<int, PlayerInfo>();

        Title = title;
        MainServer = mainServer;
        LobbyNum = lobbyNum;
        HostID = hostID;
        ServerObjects = new ViewTypes[MaxViewIDs];

        MaxPlayers = maxPlayers;

        AddPlayer(HostID);
    }

    public bool AddPlayer(int playerID)
    {
        if (PlayerIDs.Count <= MaxPlayers)
        {
            PlayerIDs.Add(playerID, new PlayerInfo());
            MainServer.Clients[playerID].LobbyNum = LobbyNum;
            return true;
        }

        return false;
    }

    public void StartMatch(int boardLength, int[] ids, int[] teamNums, int turnTime)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            PlayerIDs[ids[i]].TeamNum = teamNums[i];
        }

        for (int i = 0; i < boardLength * boardLength; i++)
        {
            ServerObjects[i] = ViewTypes.gridPos;
        }

        TurnTime = turnTime;
        InGame = true;
        NextTurn();
    }

    public void UpdateResources(int clientID, float wood, float stone, float gold)
    {
        PlayerInfo playerInfo = PlayerIDs[clientID];

        playerInfo.Wood += wood;
        playerInfo.Stone += stone;
        playerInfo.Gold += gold;

        Packet packet = new((int) ClientPackets.updateResources);
        packet.Write(clientID);
        packet.Write(playerInfo.Wood);
        packet.Write(playerInfo.Stone);
        packet.Write(playerInfo.Gold);

        MainServer.Sender.SendTCPData(clientID, packet);
    }

    public void NextTurn()
    {
        CurrentTurn += 1;
        TurnTimer = 0;

        if (CurrentTurn >= PlayerIDs.Count)
        {
            CurrentTurn = 0;
        }

        List<int> ids = new(PlayerIDs.Keys);
        int clientID = ids[CurrentTurn];

        Packet packet = new((int)ClientPackets.nextTurn);
        packet.Write(clientID);

        if (CurrentTurn == 0)
        {
            TurnNum += 1;
            packet.Write(true);
        }
        else
        {
            packet.Write(false);
        }

        MainServer.Sender.SendTCPDataToAll(packet, LobbyNum);
    }
}

[System.Serializable]
public class PlayerInfo
{
    public int TeamNum;

    public float Stone;
    public float Wood;
    public float Gold;

    public Color32 PlayerColour;

    public PlayerInfo()
    {
        Stone = 200;
        Wood = 200;
        Gold = 0;
        TeamNum = 1;
    }
}