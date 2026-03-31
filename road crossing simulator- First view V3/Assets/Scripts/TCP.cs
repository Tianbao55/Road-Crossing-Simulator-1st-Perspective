using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System;

public class TCP : MonoBehaviour
{
    [Header("TCP Settings")]
    public string serverIP = "192.168.1.50";
    public int basePort = 3910;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isRunning = false;

    [Header("Received Data")]
    public float externalSpeed = 0f;

    private int clientIndex = 0;

    void Start()
    {
        OpenTCP();
    }

    /// <summary>
    /// Start the TCP thread
    /// </summary>
    public void OpenTCP()
    {
        if (isRunning) return;

        isRunning = true;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log("TCP Receiver started.");
    }

    /// <summary>
    /// Close TCP connection and thread
    /// </summary>
    public void CloseTCP()
    {
        isRunning = false;

        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Join();

        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();

        receiveThread = null;
        Debug.Log("TCP Receiver stopped.");
    }

    /// <summary>
    /// Thread method to connect and continuously read/write TCP data
    /// </summary>
    void ReceiveData()
    {
        try
        {
            // ----------------------------
            // STEP 1: Connect to base port 3910
            // ----------------------------
            client = new TcpClient(serverIP, basePort);
            stream = client.GetStream();

            byte[] buffer4 = new byte[4];

            // Read packetType (not used)
            stream.Read(buffer4, 0, 4);
            // Read nrOutputs
            stream.Read(buffer4, 0, 4);
            // Read nrInputs
            stream.Read(buffer4, 0, 4);
            // Read clientIndex
            stream.Read(buffer4, 0, 4);
            clientIndex = BitConverter.ToInt32(buffer4, 0);
            Debug.Log("Client Index: " + clientIndex);

            // Skip client name (256 bytes)
            byte[] nameBuffer = new byte[256];
            stream.Read(nameBuffer, 0, 256);

            // Skip float values (1024 bytes)
            byte[] valuesBuffer = new byte[1024];
            stream.Read(valuesBuffer, 0, 1024);

            client.Close();

            // ----------------------------
            // STEP 2: Reconnect to port 3910 + clientIndex
            // ----------------------------
            int newPort = basePort + clientIndex;
            client = new TcpClient(serverIP, newPort);
            stream = client.GetStream();
            Debug.Log("Reconnected to port: " + newPort);

            // ----------------------------
            // STEP 3: Loop READ + WRITE
            // ----------------------------
            while (isRunning)
            {
                // ----- READ -----
                byte[] header = new byte[272];
                stream.Read(header, 0, 272);

                byte[] floatData = new byte[1024];
                stream.Read(floatData, 0, 1024);

                // Convert first float (big-endian) → externalSpeed
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(floatData, 0, 4);

                externalSpeed = BitConverter.ToSingle(floatData, 0);

                // ----- WRITE -----
                // Send empty header
                byte[] emptyHeader = new byte[272];
                stream.Write(emptyHeader, 0, 272);

                // Send 256 floats of 0f
                byte[] sendData = new byte[1024];
                for (int i = 0; i < 256; i++)
                {
                    byte[] floatBytes = BitConverter.GetBytes(0f);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(floatBytes);
                    Buffer.BlockCopy(floatBytes, 0, sendData, i * 4, 4);
                }
                stream.Write(sendData, 0, 1024);

                // Optional: wait a tiny bit to avoid CPU overload
                Thread.Sleep(10);
            }
        }
        catch (Exception e)
        {
            Debug.Log("TCP Error: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        CloseTCP();
    }
}

