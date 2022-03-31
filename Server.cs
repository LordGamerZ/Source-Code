using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using PacketModule;
using UnityEngine.UI;
using TMPro;

//Written by Abdul Galeel Ali

public class Server : MonoBehaviour
{
    public GameObject ServerConsole;

    public GameObject LogPrefab;
    public GameObject LobbyPrefab;

    public List<Color32> Colours;
    public Dictionary<int, Lobby> Lobbies = new();

    public int MaxLobbies;
    public int MaxPlayers;

    public ServerHandle Handler;
    public ServerSend Sender;

    public static int Port = 1058;

    public Dictionary<int, ServerClient> Clients = new();
    public delegate void PacketHandler(int fromClient, Packet packet, int lobbyNum);
    public Dictionary<int, PacketHandler> PacketHandlers;

    private TcpListener tcpListener;

    public void Start()
    {
        StartServer();
    }

    private void StartServer()
    {
        for (int i = 1; i < MaxLobbies; i++)
        {
            Lobbies[i] = Instantiate(LobbyPrefab).GetComponent<Lobby>();
        }

        Port = 1058;

        Debug.Log("Booting Server...");
        InitialiseServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        Debug.Log($"Lobby start on {Port}.");
    }

    public int CreateLobby(int hostID, string title, string password)
    {
        for (int i = 1; i < Lobbies.Count; i++)
        {
            if (Lobbies[i].HostID == 0)
            {
                Lobbies[i].HostID = hostID;
                Lobbies[i].StartLobby(this, title, i, 4, hostID);
                Lobbies[i].Joinable = true;
                Lobbies[i].Password = password;

                return i;
            }
        }

        return 0;
    }

    public string JoinLobby(int lobbyNum, int playerID, string password)
    {
        if (lobbyNum < Lobbies.Count)
        {
            if (Lobbies[lobbyNum].Joinable)
            {
                if (!Lobbies[lobbyNum].InGame)
                {
                    if (Lobbies[lobbyNum].Password == "" || Lobbies[lobbyNum].Password == password)
                    {
                        if (Lobbies[lobbyNum].AddPlayer(playerID))
                        {
                            return "";
                        }
                        else
                        {
                            return "Error: Lobby is full, refresh page";
                        }
                    }
                    else
                    {
                        return "Error: Incorrect password";
                    }
                }
                else
                {
                    return "Error: This game has already started, refresh page";
                }
            }
            else
            {
                return "Error: This lobby no longer exists, refresh page";
            }
        }
        else
        {
            return "Error: Incorrect Lobby ID, refresh page";
        }
    }

    private void TCPConnectCallback(IAsyncResult result)
    {
        TcpClient client = tcpListener.EndAcceptTcpClient(result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        Debug.Log($"Incoming connection from {client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (Clients[i].Socket == null)
            {
                Clients[i].Connect(client, this);

                return;
            }
        }

        Debug.Log($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
    }

    private void InitialiseServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            Clients.Add(i, new ServerClient(i, this));
        }

        PacketHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int) ClientPackets.welcome, Handler.WelcomeReceived },
            { (int) ClientPackets.createLobby, Handler.CreateLobby },
            { (int) ClientPackets.joinLobby, Handler.JoinLobby },
            { (int) ClientPackets.createDisplay, Handler.SendToAll },
            { (int) ClientPackets.changeDisplay, Handler.ChangePlayerDisplay},
            { (int) ClientPackets.chat, Handler.SendToAll },
            { (int) ClientPackets.startMatch, Handler.StartMatch },
            { (int) ClientPackets.createUnit, Handler.CreateUnit },
            { (int) ClientPackets.moveUnit, Handler.SendToAll},
            { (int) ClientPackets.unitAttack, Handler.SendToAll},
            { (int) ClientPackets.nextTurn, Handler.NextTurn},
            { (int) ClientPackets.updateResources, Handler.UpdateResources},
            { (int) ClientPackets.startBuild, Handler.StartBuild},
            { (int) ClientPackets.userLeave, Handler.UserLeave},
            { (int) ClientPackets.displayLobbies, Handler.DisplayLobbies},
            { (int) ClientPackets.removeForest, Handler.SendToAll},
            { (int) ClientPackets.otherDisplays, Handler.OtherDisplays}
        };

        Debug.Log("Initialize packets.");
    }
}