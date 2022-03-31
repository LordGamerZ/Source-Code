using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PacketModule;

//Written by Abdul Galeel Ali

public class ServerSend : MonoBehaviour
{
    public Server MainServer;

    public void SendTCPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        MainServer.Clients[toClient].SendData(packet);
    }

    public void SendTCPDataToAll(Packet packet, int lobbyNum)
    {
        packet.WriteLength();
        List<int> IDs = new List<int>(MainServer.Lobbies[lobbyNum].PlayerIDs.Keys);

        for (int i = 0; i < MainServer.Lobbies[lobbyNum].PlayerIDs.Count; i++)
        {
            MainServer.Clients[IDs[i]].SendData(packet);
        }
    }

    public void SendTCPDataAll(int exceptClient, Packet packet, int lobbyNum)
    {
        packet.WriteLength();
        List<int> IDs = new List<int>(MainServer.Lobbies[lobbyNum].PlayerIDs.Keys);

        for (int i = 0; i < MainServer.Lobbies[lobbyNum].PlayerIDs.Count; i++)
        {
            if (IDs[i] != exceptClient)
            {
                MainServer.Clients[IDs[i]].SendData(packet);
            }
        }
    }

    public void Welcome(int toClient, string msg)
    {
        Packet packet = new Packet((int)ClientPackets.welcome);
        packet.Write(msg);
        packet.Write(toClient);

        SendTCPData(toClient, packet);
    }
}