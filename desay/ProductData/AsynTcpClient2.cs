using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace System.ToolKit
{
    public class AsynTcpClient2
    {
        private Socket tcpClient;
        public AsynTcpClient2(string ip,int port)
        {
            IP = ip;
            Port = port;
            tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }
       ~AsynTcpClient2()
        {
            tcpClient.Dispose();
        }
        public void ReleaseSocket()
        {
            if (tcpClient != null)
            {
                if (tcpClient.Connected)
                {
                    tcpClient.Close();
                }
                tcpClient.Dispose();
            }
        }
        public string IP { get; set; }
        public int Port { get; set; }
        public bool IsConnected { get; set; }
        public bool SocketIsConnected
        {
            get
            {
                if (tcpClient != null)
                {
                    return tcpClient.Connected;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsRecive { get; set; }
        public string RevMessage { get; set; }
        //{
        //    //get
        //    //{                
        //    //    return tcpClient.Connected;
        //    //}
           
        //}
        #region 异步连接
        /// <summary>
        /// Tcp协议异步连接服务器
        /// </summary>
        public void AsynConnect()
        {
            //主机IP
            try
            {        
                if (!tcpClient.Connected) {
                    tcpClient.Close();
                    tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint serverIp = new IPEndPoint(IPAddress.Parse(IP), Port);
                    tcpClient.BeginConnect(serverIp, asyncResult =>
                    {
                        try
                        {
                            tcpClient.EndConnect(asyncResult);
                            //Console.WriteLine("client-->-->{0}", serverIp.ToString());
                            //AsynSend("Online...");
                            //AsynSend("第一次发送消息...");
                            //AsynSend("第二次发送消息...");
                            AsynRecive();
                            IsConnected = tcpClient.Connected;

                        }
                        catch (Exception ex)
                        {
                            IsConnected = false;
                        }
                    }, null);
                }

            }
            catch (Exception ex)
            {
                IsConnected = false;
            }
        }
        #endregion
        public void testsend()
        {
            try
            {
                if (!tcpClient.Connected)
                {
                    IsConnected = false;
                    AsynConnect();
                }
                else { AsynSend("Online..."); }
              
            }
            catch (Exception ex)
            {
                IsConnected = false;
                AsynConnect();
            }           
        }
        #region 异步接受消息
        /// <summary>
        /// 异步连接客户端回调函数
        /// </summary>
        /// <param name="tcpClient"></param>
        public void AsynRecive()
        {
            var watchCT = new Stopwatch();
            var data = new byte[1024];
            try { 
            if (tcpClient.Connected )
            {
                tcpClient.BeginReceive(data, 0, data.Length, SocketFlags.None, asyncResult =>
                {
                    try {
                        int length = tcpClient.EndReceive(asyncResult);
                        if (length > 0) {

                            RevMessage = Encoding.UTF8.GetString(data);
                        }
                        //Console.WriteLine("client<--<--server:{0}", Encoding.UTF8.GetString(data));                  
                        if (RevMessage != "" && length > 0) IsRecive = true;
                        AsynRecive();
                    }
                    catch (Exception ex)
                    {

                    }
                }, null);
              
        }
            else {  }
            }
            catch { }
        }
        #endregion

        
        #region 异步发送消息
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="tcpClient">客户端套接字</param>
        /// <param name="message">发送消息</param>
        public void AsynSend(string message)
        {
            try
            {
               RevMessage="";
                if (tcpClient.Connected)
                {
                    var data = Encoding.UTF8.GetBytes(message);
                    tcpClient.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
                    {
                        //完成发送消息

                        int length = tcpClient.EndSend(asyncResult);                       
                        //Console.WriteLine("client-->-->server:{0}", message);
                    }, null);
                }
                else {  }
            }
            catch(Exception ex) {  }
           
        }
        #endregion
    }
}
