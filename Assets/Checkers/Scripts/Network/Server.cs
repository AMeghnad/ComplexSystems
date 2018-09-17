using System.Collections;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;

public class Server : MonoBehaviour
{
    public int port = 6321;

    private List<ServerClient> disconnectList;
    private List<ServerClient> clients;

    private TcpListener server;
    private bool serverStarted;

    // Perform initialisation for server
    public void Init()
    {
        // Define variables
        DontDestroyOnLoad(gameObject);
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        // Perform a 'try-catch' block for any errors (Gotta catch em all)
        try
        {
            // Create a new TcpListener for any IP addresses
            server = new TcpListener(IPAddress.Any, port);
            // Start the server
            server.Start();
            // Start the listening method
            StartListening();
            // Flag the server as 'started'
            serverStarted = true;
        }
        catch (Exception e)
        {
            // If an error is detected, displayed the message
            Debug.Log("Socket error: " + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Is the server not started?
        if (!serverStarted)
            return;

        // Loop through entire list of clients
        foreach (ServerClient client in clients)
        {
            if (IsConnected(client.tcp))
            {
                // Get the client's network stream
                NetworkStream stream = client.tcp.GetStream();
                // Check if data is available on the stream
                if (stream.DataAvailable)
                {
                    // Setup a reader for the stream
                    StreamReader reader = new StreamReader(stream, true);
                    // Read the data from the reader of the stream
                    string data = reader.ReadLine();
                    // If there is any data
                    if (data != null)
                    {
                        // Give the client & its data to the method
                        OnIncomingData(client, data);
                    }
                }
            }
            else // ... The client disconnected
            {
                // Close the client's tcp protocol (disconnect client)
                client.tcp.Close();
                // Add to list of disconnected
                disconnectList.Add(client);
                continue;
            }

            // Loop through all the disconnected clients
            for (int i = 0; i < disconnectList.Count; i++)
            {
                // Tell our player somebody has disconnected
                clients.Remove(disconnectList[i]);
            }

            // Clear the disconnected list for another time
            disconnectList.Clear();
        }
    }

    void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    // Callback method for listening for tcp clients
    void AcceptTcpClient(IAsyncResult result)
    {
        // Get the listener
        TcpListener listener = (TcpListener)result.AsyncState;

        string allUsers = "";
        // Loop through all currrently connected clients
        foreach (ServerClient i in clients)
        {
            // Append with client name
            allUsers += i.clientName + '|';
        }

        // Get the connected clien from the listener
        ServerClient connectedClient = new ServerClient(listener.EndAcceptTcpClient(result));
        // Add newly connected client to the list
        clients.Add(connectedClient);
        // Continue lsitening for more clients
        StartListening();
        // Broadcast to all clients that there is a newly connected client
        Broadcast("SWHO|" + allUsers, connectedClient);
    }

    bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            else
                return true;
        }
        catch
        {
            return false;
        }
    }

    // Broadcast data to a list of incomingClients
    void Broadcast(string data, List<ServerClient> incomingClients)
    {
        // Loop through all incoming clients from broadcast
        foreach (ServerClient client in incomingClients)
        {
            // Try sending the data to the the client
            try
            {
                // Get a writer specifically for the current client
                StreamWriter writer = new StreamWriter(client.tcp.GetStream());
                // Send the data to client with writer
                writer.WriteLine(data);
                // Flush the writer when done
                writer.Flush();
            }
            catch (Exception e) // ... the data couldn't be sent
            {
                // Print the message error
                Debug.Log("Write error : " + e.Message);
            }
        }
    }

    // Broadcast data to a single client - This function exists for simplicity (less syntax)
    void Broadcast(string data, ServerClient incomingClient)
    {
        // Create a list containing the individual client
        List<ServerClient> client = new List<ServerClient> { incomingClient };
        // Broadcast to that list of one client
        Broadcast(data, client);
    }

    void OnIncomingData(ServerClient client, string data)
    {
        /* Client Commands:
         * CWHO - Client who connected
         * CMOV - Movement data from client
         * CMSG - Message from the client
         * 
         * Server Commands:
         * SCNN - Server new connection
         * SMOV - Server movement broadcast
         * SMSG - Server message broadcast
         */
        Debug.Log("Server:" + data);

        // NOTE: The data could be thought of as the "packet"

        // Split the data with in-line | (poles)
        string[] aData = data.Split('|');

        // Switch the header of the packet
        switch (aData[0])
        {
            // Client connected.Syntax - "CWHO | clientName | isHost"
            case:
                "CWHO":
                // Get the client's name
                client.clientName = aData[1];
                // Check if the client is a host
                client.isHost = (aData[2] == "0") ? false : true;
                // Broadcast the new client to all other clients
                Broadcast("SCNN|" + client.clientName, clients);
                break;

            // Client has made a move. Syntax - "CMOV | x1 | y1 | x2 | y2"
            //                                           |   |     |   |
            //                                            \ /       \ /
            //                                       Start Drag   End drag
            case "CMOV":
                // Send this data to all clients
                Broadcast("SMOV|" + aData[1] + "|" + aData[2] + "|" + aData[3] + "|" + aData[4], clients);
                break;
            // Chat message.Syntax - "CMSG | chatMessage"
            case "CMSG":
                Broadcast("SMSG|" + client.clientName + " : " + aData[1], clients);
                break;
        }
    }
}
