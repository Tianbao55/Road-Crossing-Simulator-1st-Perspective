using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System;

/// <summary>
/// TCP manages a client connection to a remote server for receiving real-time data (e.g., external speed).
/// It connects in two stages: first to get the client index, then to the dedicated client port.
/// </summary>
public class TCP : MonoBehaviour
{
    [Header("TCP Settings")]
    public string serverIP = "192.168.1.50"; // IP of the remote server
    public int basePort = 3910;              // Base port to connect initially

    private TcpClient client;                // TCP client object
    private NetworkStream stream;            // Network stream for read/write
    private Thread receiveThread;            // Thread to handle TCP communication
    private bool isRunning = false;          // Flag to control the thread

    [Header("Received Data")]
    public float externalSpeed = 0f;         // First float received from server, used externally

    private int clientIndex = 0;             // Client index received from server during handshake

    public Vector3 Head_Front_Pos;            // Front marker position
    public Vector3 Head_Back_Pos;             // Back marker position

    public bool sendZeroToDFlow = true;

    void Start()
    {
        OpenTCP(); // Automatically start TCP thread when object initializes
    }

    /// <summary>
    /// Start the TCP thread to connect and receive data
    /// </summary>
    public void OpenTCP()
    {
        if (isRunning) return;

        isRunning = true;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;  // Allows thread to exit when application quits
        receiveThread.Start();

        Debug.Log("TCP Receiver started.");
    }

    /// <summary>
    /// Close TCP connection and stop the receive thread
    /// </summary>
    public void CloseTCP()
    {
        isRunning = false;

        if (stream != null)
            stream.Close();

        if (client != null)
            client.Close();

        receiveThread = null;

        Debug.Log("TCP Receiver stopped.");
    }

    /// <summary>
    /// Thread method to handle TCP communication
    /// </summary>
    void ReceiveData()
    {
        try
        {
            // ----------------------------
            // STEP 1: Initial connection to base port
            // ----------------------------
            client = new TcpClient(serverIP, basePort);
            stream = client.GetStream();

            byte[] buffer4 = new byte[4];

            // Read handshake integers from server
            stream.Read(buffer4, 0, 4); // packetType (ignored)
            stream.Read(buffer4, 0, 4); // nrOutputs (ignored)
            stream.Read(buffer4, 0, 4); // nrInputs (ignored)
            stream.Read(buffer4, 0, 4); // clientIndex
            clientIndex = BitConverter.ToInt32(buffer4, 0);

            Debug.Log("Client Index: " + clientIndex);

            // Skip client name (256 bytes)
            byte[] nameBuffer = new byte[256];
            stream.Read(nameBuffer, 0, 256);

            // Skip initial float values (1024 bytes)
            byte[] valuesBuffer = new byte[1024];
            stream.Read(valuesBuffer, 0, 1024);

            client.Close(); // Close initial connection

            // ----------------------------
            // STEP 2: Reconnect to basePort + clientIndex
            // ----------------------------
            int newPort = basePort + clientIndex;
            client = new TcpClient(serverIP, newPort);
            stream = client.GetStream();
            Debug.Log("Reconnected to port: " + newPort);

            // ----------------------------
            // STEP 3: Continuous loop for reading and writing
            // ----------------------------
            while (isRunning)
            {
                byte[] header = new byte[272];
                byte[] floatData = new byte[1024];

                ReadFull(stream, header, 272);
                ReadFull(stream, floatData, 1024);

                if (BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Array.Reverse(floatData, i * 4, 4);
                    }
                }

                externalSpeed = BitConverter.ToSingle(floatData, 0);
                Head_Front_Pos.x = BitConverter.ToSingle(floatData, 4);
                Head_Front_Pos.y = BitConverter.ToSingle(floatData, 8);
                Head_Front_Pos.z = BitConverter.ToSingle(floatData, 12);
                Head_Back_Pos.x = BitConverter.ToSingle(floatData, 16);
                Head_Back_Pos.y = BitConverter.ToSingle(floatData, 20);
                Head_Back_Pos.z = BitConverter.ToSingle(floatData, 24);

                // ----- WRITE BACK TO SERVER -----

                if (sendZeroToDFlow)
                {
                    // Send empty header (272 bytes)
                    byte[] emptyHeader = new byte[272];
                    stream.Write(emptyHeader, 0, 272);

                    // Send 256 floats of 0f (1024 bytes)
                    byte[] sendData = new byte[1024];
                    for (int i = 0; i < 256; i++)
                    {
                        byte[] zeroBytes = BitConverter.GetBytes(1f);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(zeroBytes);
                        Buffer.BlockCopy(zeroBytes, 0, sendData, i * 4, 4);
                    }
                    stream.Write(sendData, 0, 1024);

                    // Optional tiny delay to reduce CPU usage
                    Thread.Sleep(10);
                }
                else
                {
                    // Send empty header (272 bytes)
                    byte[] emptyHeader = new byte[272];
                    stream.Write(emptyHeader, 0, 272);

                    // Send 256 floats of 0f (1024 bytes)
                    byte[] sendData = new byte[1024];
                    for (int i = 0; i < 256; i++)
                    {
                        byte[] floatBytes = BitConverter.GetBytes(0f);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(floatBytes);
                        Buffer.BlockCopy(floatBytes, 0, sendData, i * 4, 4);
                    }
                    stream.Write(sendData, 0, 1024);

                    // Optional tiny delay to reduce CPU usage
                    Thread.Sleep(10);
                }
                // if (sendZeroToDFlow)
                // {
                //     // Send empty header (272 bytes)
                //     byte[] emptyHeader = new byte[272];
                //     stream.Write(emptyHeader, 0, 272);

                //     // Send 256 floats of 0f (1024 bytes)
                //     byte[] sendData = new byte[1024];
                //     for (int i = 0; i < 256; i++)
                //     {
                //         byte[] zeroBytes = BitConverter.GetBytes(1f);
                //         if (BitConverter.IsLittleEndian)
                //             Array.Reverse(zeroBytes);
                //         Buffer.BlockCopy(zeroBytes, 0, sendData, i * 4, 4);
                //     }
                //     stream.Write(sendData, 0, 1024);

                //     // Optional tiny delay to reduce CPU usage
                //     Thread.Sleep(10);
                // }
                // else
                // {
                //     // Send empty header (272 bytes)
                //     byte[] emptyHeader = new byte[272];
                //     stream.Write(emptyHeader, 0, 272);

                //     // Send 256 floats of 0f (1024 bytes)
                //     byte[] sendData = new byte[1024];
                //     for (int i = 0; i < 256; i++)
                //     {
                //         byte[] floatBytes = BitConverter.GetBytes(0f);
                //         if (BitConverter.IsLittleEndian)
                //             Array.Reverse(floatBytes);
                //         Buffer.BlockCopy(floatBytes, 0, sendData, i * 4, 4);
                //     }
                //     stream.Write(sendData, 0, 1024);

                //     // Optional tiny delay to reduce CPU usage
                //     Thread.Sleep(10);
                // }
            }
        }
        catch (Exception e)
        {
            Debug.Log("TCP Error: " + e.Message);
        }
    }

    /// <summary>
    /// Automatically called when the application quits to clean up TCP
    /// </summary>
    void OnApplicationQuit()
    {
        CloseTCP();
    }

    void ReadFull(NetworkStream stream, byte[] buffer, int length)
    {
        int total = 0;

        while (total < length)
        {
            int bytesRead = stream.Read(buffer, total, length - total);

            if (bytesRead == 0)
                throw new Exception("TCP disconnected");

            total += bytesRead;
        }
    }
}



// using UnityEngine;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading;
// using System.Collections.Generic;
// using Newtonsoft.Json;

// /// <summary>
// /// UDPforDegree receives real-time head marker positions via UDP.
// /// It runs a background thread to listen for UDP messages and parses them into Vector3 positions.
// /// </summary>
// public class UDPforDegree : MonoBehaviour
// {
//     [Header("UDP Settings")]
//     public int port = 5005;                   // Port to listen on

//     private UdpClient udpClient;              // UDP client for receiving data
//     private Thread receiveThread;             // Thread for background UDP reception
//     private bool isRunning = false;           // Flag to control thread loop
//     private string latestData = "";           // Stores the latest received UDP JSON string

//     [Header("Head Marker Positions")]
//     public Vector3 Head_Front_Pos;            // Front marker position
//     public Vector3 Head_Back_Pos;             // Back marker position

//     /// <summary>
//     /// Open the UDP listener and start background thread
//     /// </summary>
//     public void OpenUDP()
//     {
//         if (isRunning) return;

//         udpClient = new UdpClient(port);
//         isRunning = true;

//         // Start thread for receiving data
//         receiveThread = new Thread(new ThreadStart(ReceiveData));
//         receiveThread.IsBackground = true; // Thread stops when application exits
//         receiveThread.Start();

//         Debug.Log("UDP Receiver started on port " + port);
//     }

//     /// <summary>
//     /// Close UDP listener and stop the receive thread
//     /// </summary>
//     public void CloseUDP()
//     {
//         isRunning = false;

//         if (udpClient != null)
//         {
//             udpClient.Close();
//             udpClient = null;
//         }

//         if (receiveThread != null && receiveThread.IsAlive)
//         {
//             receiveThread.Join(); // Wait for thread to finish
//             receiveThread = null;
//         }

//         Debug.Log("UDP Receiver stopped.");
//     }

//     /// <summary>
//     /// Thread method: continuously receive UDP packets
//     /// </summary>
//     private void ReceiveData()
//     {
//         IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);

//         while (isRunning)
//         {
//             try
//             {
//                 // Receive UDP packet (blocking call)
//                 byte[] data = udpClient.Receive(ref remoteEP);
//                 latestData = Encoding.UTF8.GetString(data); // Convert bytes to string
//             }
//             catch (System.Exception e)
//             {
//                 if (isRunning) // Only log errors if still running
//                     Debug.Log("UDP Error: " + e);
//             }
//         }
//     }

//     /// <summary>
//     /// Update is called every frame in the main thread
//     /// Parses the latest JSON string and updates marker positions
//     /// </summary>
//     void Update()
//     {
//         if (string.IsNullOrEmpty(latestData))
//             return;

//         try
//         {
//             // Parse JSON into dictionary: key = marker name, value = float array [x,y,z]
//             var positions = JsonConvert.DeserializeObject<Dictionary<string, float[]>>(latestData);

//             // Update front marker position if present
//             if (positions.ContainsKey("UnlabeledMarker0"))
//             {
//                 float[] p = positions["UnlabeledMarker0"];
//                 Head_Front_Pos = new Vector3(p[0], p[1], p[2]);
//             }

//             // Update back marker position if present
//             if (positions.ContainsKey("UnlabeledMarker1"))
//             {
//                 float[] p = positions["UnlabeledMarker1"];
//                 Head_Back_Pos = new Vector3(p[0], p[1], p[2]);
//             }

//             // if (positions.ContainsKey("HF"))
//             // {
//             //     float[] p = positions["HF"];
//             //     Head_Front_Pos = new Vector3(p[0], p[1], p[2]);
//             // }

//             // if (positions.ContainsKey("HB"))
//             // {
//             //     float[] p = positions["HB"];
//             //     Head_Back_Pos = new Vector3(p[0], p[1], p[2]);
//             // }

//             // Clear latestData to avoid parsing the same packet multiple times
//             latestData = "";
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogWarning("Failed to parse UDP JSON: " + e.Message);
//             latestData = ""; // Clear to prevent repeated errors
//         }
//     }

//     /// <summary>
//     /// Ensure UDP is closed when application exits
//     /// </summary>
//     void OnApplicationQuit()
//     {
//         CloseUDP();
//     }
// }
