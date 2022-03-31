using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using PacketModule;

//Written by Abdul Galeel Ali

public class ServerClient
{
    public static int DataBufferSize = 4096;
    public int ClientID;
    public string Username;

    public Server MainServer;
    public int LobbyNum;

    public TcpClient Socket;
    private NetworkStream Stream;
    private Packet ReceivedData;
    private byte[] ReceiveBuffer;

    public ServerClient(int clientID, Server mainServer)
    {
        MainServer = mainServer;
        ClientID = clientID;
    }

    public void Connect(TcpClient socket, Server mainServer)
    {
        MainServer = mainServer;

        Socket = socket;
        Socket.ReceiveBufferSize = DataBufferSize;
        Socket.SendBufferSize = DataBufferSize;

        Stream = Socket.GetStream();

        ReceivedData = new Packet();
        ReceiveBuffer = new byte[DataBufferSize];

        Stream.BeginRead(ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);

        MainServer.Sender.Welcome(ClientID, "Connection Successful");
    }

    public void SendData(Packet packet)
    {
        try
        {
            if (Socket != null)
            {
                Stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
        }
        catch (Exception exception)
        {
            Debug.Log($"Error sending data to player {ClientID} via TCP: {exception}");
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            int byteLength = Stream.EndRead(result);

            if (byteLength <= 0)
            {
                Disconnect();
                return;
            }

            byte[] data = new byte[byteLength];
            Array.Copy(ReceiveBuffer, data, byteLength);

            ReceivedData.Reset(HandleData(data));
            Stream.BeginRead(ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
        }
        catch (Exception exception)
        {
            Debug.Log($"Error receiving TCP data: {exception}.");
            Disconnect();
        }
    }

    private bool HandleData(byte[] data)
    {
        int packetLength = 0;

        ReceivedData.SetBytes(data);

        if (ReceivedData.UnreadLength() >= 4)
        {
            packetLength = ReceivedData.ReadInt();
            if (packetLength <= 0)
            {
                return true;
            }
        }

        while (packetLength > 0 && packetLength <= ReceivedData.UnreadLength())
        {
            Packet packet = new Packet(ReceivedData.ReadBytes(packetLength));

            ThreadManager.ExecuteOnMainThread(() =>
            {
                MainServer.PacketHandlers[packet.ReadInt()](ClientID, packet, LobbyNum);
            });

            packetLength = 0;

            if (ReceivedData.UnreadLength() >= 4)
            {
                packetLength = ReceivedData.ReadInt();

                if (packetLength <= 0)
                {
                    return true;
                }
            }
        }

        if (packetLength <= 1)
        {
            return true;
        }

        Debug.Log("False");
        return false;
    }

    public void Disconnect()
    {
        if (LobbyNum != 0)
        {
            MainServer.Lobbies[LobbyNum].UserLeave(ClientID);
        }

        Socket.Close();
        Stream = null;
        ReceivedData = null;
        ReceiveBuffer = null;
        Socket = null;
    }
}