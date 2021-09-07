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

        private IPAddress _serverIP = IPAddress.Parse("127.0.0.1");
        private int _localPort = 8000;

        /// <summary>哈希列表保存连接的客户端</summary>
        public Hashtable _sessionTable = new Hashtable(53);
        private byte[] _byteMsgRec = new byte[1024 * 1024 * 4];
        private int _bufferSize = 1024 * 1024 * 4;
        //private Mutex m_listenEvent = new Mutex();

        private bool _status = false;

        /// <summary>设置服务端监听的端口号</summary>
        public int localPort
        {
            get { return _localPort; }
            set { _localPort = value; }
        }

        /// <summary>设置服务端的IP地址</summary>
        public string serverIP
        {
            get { return _serverIP.ToString(); }
            set { _serverIP = IPAddress.Parse(value); }
        }

        public bool m_Status
        {
            get { return _status; }
        }

        public int m_BufferSize
        {
            set
            {
                _bufferSize = value;
                _byteMsgRec = new byte[value];
            }
            get { return _bufferSize; }
        }

        #region Tcp协议异步监听
        /// <summary>
        /// Tcp协议异步监听
        /// </summary>
        public void StartListening()
        {
            //主机IP
            IPEndPoint serverIp = new IPEndPoint(_serverIP, _localPort);
            Socket tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpServer.Bind(serverIp);
            tcpServer.Listen(100);
            Console.WriteLine("异步开启监听...");
            _status = true;
            AsynAccept(tcpServer);
        }
        #endregion

        #region  关闭异步监听
        /// <summary>
        /// 关闭套接字
        /// </summary>
        public void CloseSockt()
        {
            //主机IP
            IPEndPoint serverIp = new IPEndPoint(_serverIP, _localPort);
            Socket tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpServer.Bind(serverIp);
            tcpServer.Shutdown(SocketShutdown.Both);
            tcpServer.Close();
            _status = true;
        }
        #endregion

        #region 异步连接客户端
        /// <summary>
        /// 异步连接客户端
        /// </summary>
        /// <param name="tcpServer"></param>
        public void AsynAccept(Socket tcpServer)
        {
            tcpServer.BeginAccept(asyncResult =>
            {
                Socket tcpClient = tcpServer.EndAccept(asyncResult);
                Console.WriteLine("server<--<--{0}", tcpClient.RemoteEndPoint.ToString());
                AsynSend(tcpClient, "收到连接...");//发送消息
                AsynAccept(tcpServer);
                AsynRecive(tcpClient);
            }, null);
        }
        #endregion

        #region 异步接受客户端消息
        /// <summary>
        /// 异步接受客户端消息
        /// </summary>
        /// <param name="tcpClient"></param>
        public void AsynRecive(Socket tcpClient)
        {
            byte[] data = new byte[1024];
            try
            {
                tcpClient.BeginReceive(data, 0, data.Length, SocketFlags.None,
                asyncResult =>
                {
                    int length = tcpClient.EndReceive(asyncResult);
                    Console.WriteLine("server<--<--client:{0}", Encoding.UTF8.GetString(data));
                    AsynSend(tcpClient, "收到消息...");
                    AsynRecive(tcpClient);
                }, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常信息：", ex.Message);
            }
        }
        #endregion

        #region 异步发送消息
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="tcpClient">客户端套接字</param>
        /// <param name="message">发送消息</param>
        public void AsynSend(Socket tcpClient, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            try
            {
                tcpClient.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
                {
                    //完成发送消息
                    int length = tcpClient.EndSend(asyncResult);
                    Console.WriteLine("server-->-->client:{0}", message);
                }, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常信息：{0}", ex.Message);
            }
        }
        #endregion
    }
}