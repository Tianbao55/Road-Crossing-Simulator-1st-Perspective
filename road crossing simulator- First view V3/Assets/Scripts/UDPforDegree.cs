using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;

public class UDPforDegree : MonoBehaviour
{
    public int port = 5005;

    UdpClient udpClient;
    Thread receiveThread;

    string latestData = "";

    public Vector3 Head_Front_Pos;
    public Vector3 Head_Back_Pos;

    bool isRunning = false;

    void Start()
    {

    }

    public void OpenUDP()
    {
        if (isRunning) return;

        udpClient = new UdpClient(port);
        isRunning = true;

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log("UDP Receiver started.");
    }

    public void CloseUDP()
    {
        isRunning = false;

        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }

        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Join();
            receiveThread = null;
        }

        Debug.Log("UDP Receiver stopped.");
    }

    void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);

        while (isRunning)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                latestData = Encoding.UTF8.GetString(data);
            }
            catch (System.Exception e)
            {
                if (isRunning)
                    Debug.Log("UDP Error: " + e);
            }
        }
    }

    void Update()
    {
        if (string.IsNullOrEmpty(latestData))
            return;

        var positions = JsonConvert.DeserializeObject<Dictionary<string, float[]>>(latestData);

        // test only
        if (positions.ContainsKey("UnlabeledMarker0"))
        {
            float[] p = positions["UnlabeledMarker0"];
            Head_Front_Pos = new Vector3(p[0], p[1], p[2]);
        }

        if (positions.ContainsKey("UnlabeledMarker1"))
        {
            float[] p = positions["UnlabeledMarker1"];
            Head_Back_Pos = new Vector3(p[0], p[1], p[2]);
        }

        // if (positions.ContainsKey("HF"))
        // {
        //     float[] p = positions["HF"];
        //     Head_Front_Pos = new Vector3(p[0], p[1], p[2]);
        // }

        // if (positions.ContainsKey("HB"))
        // {
        //     float[] p = positions["HB"];
        //     Head_Back_Pos = new Vector3(p[0], p[1], p[2]);
        // }
        latestData = "";
    }

    void OnApplicationQuit()
    {
        CloseUDP();
    }
}
