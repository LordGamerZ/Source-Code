using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PacketModule;

//Written by Abdul Galeel Ali

public class ServerHandle : MonoBehaviour
{
    public Server MainServer;

    public void WelcomeReceived(int fromClient, Packet packet, int lobbyNum)
    {
        int clientIDCheck = packet.ReadInt();
        string username = packet.ReadString();

        Debug.Log($"{MainServer.Clients[fromClient].Socket.Client.RemoteEndPoint} connected succesfully and is now player {fromClient}.");

        if (fromClient != clientIDCheck)
        {
            Debug.Log($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIDCheck})!");
        }

        MainServer.Clients[fromClient].Username = username;
    }

    public void JoinLobby(int fromClient, Packet packet, int lobbyNum)
    {
        Debug.Log("Attempted Join");

        int clientID = packet.ReadInt();
        int lobby = packet.ReadInt();
        string password = packet.ReadString();

        Packet newPacket = new Packet((int)ClientPackets.joinLobby);
        newPacket.Write(clientID);

        string message = MainServer.JoinLobby(lobby, clientID, password);

        if (message == "")
        {
            newPacket.Write(true);
            newPacket.Write(lobby);
            newPacket.Write(MainServer.Lobbies[lobby].MaxViewIDs);
        }
        else
        {
            newPacket.Write(false);
            newPacket.Write(message);
        }

        MainServer.Sender.SendTCPData(clientID, newPacket);
    }

    public void CreateLobby(int fromClient, Packet packet, int lobbyNum)
    {
        Debug.Log("Attempted Create");

        int clientID = packet.ReadInt();
        string title = packet.ReadString();
        string password = packet.ReadString();

        Packet newPacket = new Packet((int)ClientPackets.createLobby);
        newPacket.Write(clientID);

        int lobby = MainServer.CreateLobby(clientID, title, password);

        if (lobby != -1)
        {
            newPacket.Write(true);
            newPacket.Write(lobby);
            newPacket.Write(MainServer.Lobbies[lobby].MaxViewIDs);
        }
        else
        {
            newPacket.Write(false);
        }

        MainServer.Sender.SendTCPData(clientID, newPacket);
    }

    public void DisplayLobbies(int fromClient, Packet packet, int lobbyNum)
    {
        for (int i = 1; i < MainServer.Lobbies.Count; i++)
        {
            Lobby lobby = MainServer.Lobbies[i];

            if (lobby.HostID != 0)
            {
                Packet newPacket = new Packet((int)ClientPackets.displayLobbies);
                newPacket.Write(fromClient);
                newPacket.Write(lobby.Title);
                newPacket.Write(MainServer.Clients[lobby.HostID].Username);
                newPacket.Write(lobby.PlayerIDs.Count + "/" + lobby.MaxPlayers);

                if (lobby.Password == "")
                {
                    newPacket.Write(false);
                }
                else
                {
                    newPacket.Write(true);
                }

                newPacket.Write(i);

                MainServer.Sender.SendTCPData(fromClient, newPacket);
            }
        }
    }

    public void StartMatch(int fromClient, Packet packet, int lobbyNum)
    {
        int clientID = packet.ReadInt();
        int boardLength = packet.ReadInt();
        int[] playerIDs = packet.ReadIntArray();
        int[] teamNums = packet.ReadIntArray();
        int turnTime = packet.ReadInt();
        int[] baseIDs = new int[playerIDs.Length];

        for (int i = 0; i < playerIDs.Length; i++)
        {
            baseIDs[i] = FreeViewID(MainServer.Lobbies[lobbyNum].ServerObjects);
            MainServer.Lobbies[lobbyNum].ServerObjects[baseIDs[i]] = ViewTypes.building;
        }

        int[] odds = new int[20] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2 };
        int[] board = new int[boardLength * boardLength];

        for (int i = 0; i < board.Length; i++)
        {
            board[i] = odds[Random.Range(0, 19)];
        }

        Color[] colors = new Color[playerIDs.Length];
        for (int i = 0; i < playerIDs.Length; i++)
        {
            colors[i] = MainServer.Colours[i];
            MainServer.Lobbies[lobbyNum].PlayerIDs[playerIDs[i]].PlayerColour = colors[i];
        }

        Packet newPacket = new Packet((int)ClientPackets.startMatch);
        newPacket.Write(clientID);
        newPacket.Write(boardLength);
        newPacket.Write(playerIDs);
        newPacket.Write(teamNums);
        newPacket.Write(turnTime);
        newPacket.Write(baseIDs);
        newPacket.Write(colors);
        newPacket.Write(board);

        MainServer.Sender.SendTCPDataToAll(newPacket, lobbyNum);
        MainServer.Lobbies[lobbyNum].StartMatch(boardLength, playerIDs, teamNums, turnTime);
    }

    public void CreateUnit(int fromClient, Packet packet, int lobbyNum)
    {
        int newViewID = FreeViewID(MainServer.Lobbies[lobbyNum].ServerObjects);

        if (newViewID != -1)
        {
            packet.Write(newViewID);
            packet.Write(MainServer.Lobbies[lobbyNum].PlayerIDs[fromClient].PlayerColour);

            MainServer.Lobbies[lobbyNum].ServerObjects[newViewID] = ViewTypes.unit;

            MainServer.Sender.SendTCPDataToAll(packet, lobbyNum);
        }
    }

    public void OtherDisplays(int fromClient, Packet packet, int lobbyNum)
    {
        List<int> playerIDs = new List<int>(MainServer.Lobbies[lobbyNum].PlayerIDs.Keys);

        int[] ids = new int[playerIDs.Count];
        string[] usernames = new string[playerIDs.Count];
        int[] teamNums = new int[playerIDs.Count];

        for (int i = 0; i < playerIDs.Count; i++)
        {
            if(playerIDs[i] != fromClient)
            {
                ids[i] = playerIDs[i];
                usernames[i] = MainServer.Clients[playerIDs.Count].Username;
                teamNums[i] = MainServer.Lobbies[lobbyNum].PlayerIDs[playerIDs[i]].TeamNum;
            }
        }

        Packet packet1 = new Packet((int)ClientPackets.changeDisplay);

        packet1.Write(ids);
        packet1.Write(usernames);
        packet1.Write(teamNums);

        MainServer.Sender.SendTCPData(fromClient, packet1);
    }

    public void ChangePlayerDisplay(int fromClient, Packet packet, int lobbyNum)
    {
        int clientID = packet.ReadInt();
        int teamNum = packet.ReadInt();

        MainServer.Lobbies[lobbyNum].PlayerIDs[fromClient].TeamNum = teamNum;

        Packet packet1 = new Packet((int) ClientPackets.changeDisplay);
        packet1.Write(clientID);
        packet1.Write(teamNum);

        MainServer.Sender.SendTCPDataAll(fromClient, packet, lobbyNum);
    }

    public void StartBuild(int fromClient, Packet packet, int lobbyNum)
    {
        int newViewID = FreeViewID(MainServer.Lobbies[lobbyNum].ServerObjects);

        if (newViewID != -1)
        {
            int clientID = packet.ReadInt();
            int builderID = packet.ReadInt();
            int profileID = packet.ReadInt();
            float wood = packet.ReadFloat();
            float ore = packet.ReadFloat();
            float gold = packet.ReadFloat();

            MainServer.Lobbies[lobbyNum].UpdateResources(fromClient, wood, ore, gold);
            MainServer.Lobbies[lobbyNum].ServerObjects[newViewID] = ViewTypes.building;

            Packet newPacket = new Packet((int)ClientPackets.startBuild);
            newPacket.Write(clientID);
            newPacket.Write(builderID);
            newPacket.Write(profileID);
            newPacket.Write(newViewID);
            newPacket.Write(MainServer.Lobbies[lobbyNum].PlayerIDs[fromClient].PlayerColour);

            MainServer.Sender.SendTCPDataToAll(newPacket, lobbyNum);
        }
    }

    public void SendToAll(int fromClient, Packet packet, int lobbyNum)
    {
        if (fromClient == packet.ReadInt(false))
        {
            MainServer.Sender.SendTCPDataToAll(packet, lobbyNum);
        }
    }

    public int FreeViewID(ViewTypes[] viewIDs)
    {
        int newViewID = -1;

        for (int i = 0; i < viewIDs.Length; i++)
        {
            if (viewIDs[i] == ViewTypes.empty)
            {
                newViewID = i;
                break;
            }
        }

        return newViewID;
    }

    public void NextTurn(int fromClient, Packet packet, int lobbyNum)
    {
        int clientID = new List<int>(MainServer.Lobbies[lobbyNum].PlayerIDs.Keys)[MainServer.Lobbies[lobbyNum].CurrentTurn];
        if (packet.ReadInt() == clientID)
        {
            MainServer.Lobbies[lobbyNum].NextTurn();
        }
    }

    public void UpdateResources(int fromClient, Packet packet, int lobbyNum)
    {
        int clientID = packet.ReadInt();
        float wood = packet.ReadFloat();
        float ore = packet.ReadFloat();
        float gold = packet.ReadFloat();

        MainServer.Lobbies[lobbyNum].UpdateResources(fromClient, wood, ore, gold);
    }

    public void UserLeave(int clientID, Packet packet, int lobbyNum)
    {
        MainServer.Lobbies[lobbyNum].UserLeave(clientID);
    }
}