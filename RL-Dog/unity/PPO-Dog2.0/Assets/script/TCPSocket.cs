using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;
using System.Text;
using System;


public class TCPSocket  {

    #region private members
    private TcpClient socketConnection;

    #endregion

    const int RevSize = 1024;
    const int SenfSize = 1024;
    public byte[] RevBuffer = new byte[RevSize];
    public byte[] SendBuffer = new byte[SenfSize];

    public string strMsg = string.Empty;
    public bool MsgReceived = false;

    public void TCPSocketQuit() {

        socketConnection.Close();

        Debug.Log("TCP Client has quitted!");

    }

     public void Connect2TcpServer() {

        try
        {
            socketConnection = new TcpClient("192.168.0.104", 50213);
            socketConnection.GetStream().BeginRead(RevBuffer, 0, RevSize, new AsyncCallback(Listen4Data), null);

            Debug.Log("TCP Client connected!");
        }
        catch{

            Debug.Log("Open thread for build client is error!  ");

        }

    }

     public void Listen4Data(IAsyncResult ar) {

        int BytesRead;

        try
        {
            BytesRead = socketConnection.GetStream().EndRead(ar);

            if (BytesRead < 1)
            {
         
                Debug.Log("Disconnected");
                return;
            }

            strMsg = Encoding.Default.GetString(RevBuffer, 0, BytesRead );

            //Debug.Log(strMesg);
            MsgReceived = true;

            socketConnection.GetStream().BeginRead(RevBuffer, 0, RevSize, new AsyncCallback(Listen4Data), null);

        }
        catch {
            Debug.Log("Disconnected");
        }


    }

     public void SendMessage(string msg)
    {
        if (socketConnection == null)
        {
            return;
        }

        try
        {		
            NetworkStream stream = socketConnection.GetStream();

            if (stream.CanWrite)
            {
                //string clientMessage = "This is a message from one of your clients.";
            
                SendBuffer = Encoding.ASCII.GetBytes(msg);

                stream.Write(SendBuffer, 0, SendBuffer.Length);
                
            }
        }

        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}



