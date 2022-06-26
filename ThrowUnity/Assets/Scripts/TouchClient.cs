using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TouchClient : MonoBehaviour
{
    private const int port = 23457;
    private Socket sock;

    public string host = "127.0.0.1";
    public Toggle toggle;

    public void ToggleConnect()
    {
        if (toggle.isOn)
        {
            Connect();
        }
        else
        {
            Disconnect();
        }
    }

    private void Connect()
    {
        Debug.Log("Connect to " + host + ":" + port);

        IPAddress ip = IPAddress.Parse(host);
        sock = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        sock.NoDelay = true;
        sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, false);
        sock.ReceiveTimeout = 1000;
        sock.SendBufferSize = 32;
        IPEndPoint remoteEP = new IPEndPoint(ip, port);
        sock.Connect(remoteEP);
    }

    private void Disconnect()
    {
        if (sock != null)
        {
            Debug.Log("Disonnect");
            sock.Close();
            sock = null;
        }
    }

    public void ThrowBall()
    {
        Send("a");
    }

    public void Send(string key)
    {
        Debug.Log("Send: " + key);

        if (sock != null)
        {
            byte[] msg = Encoding.UTF8.GetBytes(key);

            lock (sock)
            {
                IAsyncResult ar = sock.BeginSend(msg, 0, msg.Length, SocketFlags.None, null, null);
                sock.EndSend(ar);
            }
        }
    }
}
