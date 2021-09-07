using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace System.ToolKit
{
    /// <summary>
    /// Tcp协议异步通讯类(服务器端)
    /// </summary>
    public class AsynTcpServer
    {
        TcpListener server;
        NetworkStream stream;
        Thread thread;
        public AsynTcpServer(int port)
        {
            //serverIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), localPort);
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            server.Start();
            thread = new Thread(run);
            thread.Start();
        }
        public AsynTcpServer(int port, String iPAddress)
        {
            server = new TcpListener(IPAddress.Parse(iPAddress), port);
            server.Start();
        }
        private bool _status = false;
        public string strResultTCP;
        Byte[] bytes = new Byte[256];
        public bool IsResultTCP;



        ///// <summary>
        ///// 连接的客户端
        ///// </summary>
        //private Socket tcpServer1;

        #region Tcp协议异步监听
        /// <summary>
        /// Tcp协议异步监听
        /// </summary>
        public void StartListening()
        {
            strResultTCP = null;
            _status = false;
            TcpClient client = server.AcceptTcpClient();
            _status = true;
            stream = client.GetStream();
        }
        #endregion


        private void run()
        {
            while (true)
            {

                StartListening();
                AsynRecive();
            }

        }

        #region 接受客户端消息
        /// <summary>
        /// 接受客户端消息
        /// </summary>
        /// <param name="tcpClient"></param>
        public void AsynRecive()
        {
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    strResultTCP = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    strResultTCP = strResultTCP.ToUpper();
                    IsResultTCP = true;
                    LogHelper.Debug(strResultTCP);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Debug(ex.ToString());
            }

        }
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="tcpClient">客户端套接字</param>
        /// <param name="message">发送消息</param>
        public void AsynSend(string str)
        {
            byte[] msg = Encoding.ASCII.GetBytes(str);
            stream.Write(msg, 0, msg.Length);
            LogHelper.Debug(str);
        }
        #endregion
    }
}