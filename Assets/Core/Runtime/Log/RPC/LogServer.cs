using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cr7Sund.Logger
{
    public class LogServer
    {
        private Socket clientSocket;
        private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
        private CancellationTokenSource cts;

        private const string host = "127.0.0.1";
        private const int port = 11000;
        private static int reconnectInterval = 5000; // 重连间隔，单位为毫秒
        public const int BufferSize = 1024;
        byte[] buffer = new byte[BufferSize];
        private const byte messageType = 1;

        public async Task StartServer()
        {
            // 连接信息
            await Connect();
        }

        private async Task Connect()
        {
            while (true)
            {
                try
                {
                    // 连接信息
                    var ipAddress = IPAddress.Parse(host);

                    // Create a TCP socket.
                    clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    await clientSocket.ConnectAsync(new IPEndPoint(ipAddress, port));
                    Debug.UnityEditorDebug("Connected to {0}:{1}", host, port);

                    cts = new CancellationTokenSource();
                    // 创建发送和接收任务
                    Task sendTask = Task.Run(() => SendMessages(clientSocket), cts.Token);
                    Task receiveTask = Task.Run(() => ReceiveMessages(clientSocket), cts.Token);

                    break;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Socket exception: " + ex.ToString());
                    Console.WriteLine("Failed to connect to server. Retrying in " + reconnectInterval + " ms...");
                    await Task.Delay(reconnectInterval);
                }

            }
        }
        public async Task SendAsync(string message)
        {
            messageQueue.Enqueue(message);
            if (cts.Token.IsCancellationRequested)
            {
                await Connect();
            }
        }

        // 发送消息
        private void SendMessages(Socket client)
        {
            while (true)
            {
                if (cts.Token.IsCancellationRequested)
                {
                    break;
                }
                if (messageQueue.TryDequeue(out string message))
                {
                    // 发送消息
                    // ----------
                    /// 编码消息体
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                    // 构造消息头
                    byte[] headerBytes = new byte[5];
                    BitConverter.GetBytes(messageBytes.Length).CopyTo(headerBytes, 0);
                    headerBytes[4] = (byte)messageType;

                    // 合并消息头和消息体
                    byte[] packetBytes = new byte[headerBytes.Length + messageBytes.Length];
                    headerBytes.CopyTo(packetBytes, 0);
                    messageBytes.CopyTo(packetBytes, headerBytes.Length);

                    try
                    {
                        client.SendAsync(packetBytes, SocketFlags.None);
                    }
                    catch (SocketException e)
                    {
                        switch (e.SocketErrorCode)
                        {
                            case SocketError.Shutdown:
                                break;
                            default: break;
                        }
                        Debug.UnityEditorError($"Send Message Error: {e.Message}");

                        break;
                    }
                    catch (Exception e)
                    {
                        Debug.UnityEditorError($"Send Message Error: {e.Message} {e.StackTrace}");
                        break;
                    }

                }
            }

            DisConnect();
        }

        // 接收消息
        private async void ReceiveMessages(Socket client)
        {
            while (true)
            {
                try
                {
                    if (cts.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    int bytesRead = await client?.ReceiveAsync(buffer, SocketFlags.None);

                    if (bytesRead > 0)
                    {
                        // 处理消息
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.UnityEditorDebug($"Received message: {message}");
                    }
                    else
                    {
                        Debug.UnityEditorDebug($"Received message: Server has closed");
                        break;
                    }

                    // 打印接收到的消息
                }
                catch (SocketException e)
                {
                    switch (e.SocketErrorCode)
                    {
                        case SocketError.Shutdown:
                            break;
                        default: break;
                    }
                    Debug.UnityEditorError($"Receive message SocketError: {e.Message}");
                    break;
                }
                catch (System.ObjectDisposedException e)
                {
                    if (e.ObjectName == "System.Net.Sockets.Socket")
                    {
                        Debug.UnityEditorError($"Received message: stop receiving message \n {e.Message}");
                    }
                    break;
                }
                catch (Exception e)
                {
                    Debug.UnityEditorError($"Receive message Error: {e.Message}");
                    break;
                }
            }

            DisConnect();
        }

        public void DisConnect()
        {
            try
            {
                cts.Cancel();
                // 关闭连接
                clientSocket?.Shutdown(SocketShutdown.Both);
                clientSocket?.Close();
            }
            catch (SocketException e)
            {
                Debug.UnityEditorError($"Disconnect Socket Error: {e.Message}");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }


}
