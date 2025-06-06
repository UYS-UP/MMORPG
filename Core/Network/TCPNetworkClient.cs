using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TCPNetworkClient : MonoBehaviour, INetwork
{
    public string serverIP = "127.0.0.1";
    public int serverPort = 8888;
    public int receiveBufferSize = 8192;
    public int sendBufferSize = 8192;

    private TcpClient client;
    private NetworkStream stream;
    private CancellationTokenSource cts;
    private bool isConnected = false;

    private readonly ConcurrentQueue<byte[]> receiveQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> sendQueue = new ConcurrentQueue<byte[]>();

    private byte[] packetBuffer;
    private int bufferOffset = 0;
    
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<byte[]> OnDataReceived;
    public event Action<Exception> OnError;

    private void Update()
    {
        while (receiveQueue.TryDequeue(out byte[] data))
        {
            OnDataReceived?.Invoke(data);
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    public async UniTaskVoid Connect()
    {
        if(isConnected) return;
        try
        {
            cts = new CancellationTokenSource();
            client = new TcpClient();
            client.ReceiveBufferSize = receiveBufferSize;
            client.SendBufferSize = sendBufferSize;

            await client.ConnectAsync(serverIP, serverPort)
                .AsUniTask().AttachExternalCancellation(cts.Token);

            stream = client.GetStream();
            isConnected = true;

            packetBuffer = new byte[receiveBufferSize * 2];
            
            ReceiveDataAsync(cts.Token).Forget();
            SendDataAsync(cts.Token).Forget();
            
            OnConnected?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"连接服务器失败:{e.Message}");
            OnError?.Invoke(e);
            Disconnect();
        }
    }

    public void Send(byte[] data)
    {
        if(!isConnected) return;
        byte[] packet = new byte[data.Length + 4];
        byte[] lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data.Length));
        Buffer.BlockCopy(lengthBytes, 0, packet, 0, 4);
        Buffer.BlockCopy(data, 0, packet, 4, data.Length);
        sendQueue.Enqueue(packet);
    }

    public void Disconnect()
    {
        if(!isConnected) return;
        
        cts?.Cancel();
        stream?.Close();
        client?.Close();

        isConnected = false;
        bufferOffset = 0;
        OnDisconnected?.Invoke();
    }
    

    private async UniTask SendDataAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested && isConnected)
            {
                if (sendQueue.TryDequeue(out byte[] data))
                {
                    await stream.WriteAsync(data, 0, data.Length, token).AsUniTask();
                    Debug.Log("写入成功");
                }
                else
                {
                    await UniTask.Delay(10, cancellationToken: token);
                }
            }
        }
        catch (Exception e)
        {
            if (!token.IsCancellationRequested)
            {
                OnError?.Invoke(e);
                Disconnect();
            }
        }
    }
    
    private async UniTask ReceiveDataAsync(CancellationToken token)
    {
        byte[] buffer = new byte[receiveBufferSize];
        try
        {
            while (!token.IsCancellationRequested && isConnected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token).AsUniTask();
                if (bytesRead == 0)
                {
                    Disconnect();
                    return;
                }

                ProcessReceiveData(buffer, bytesRead);
            }
        }   
        catch (Exception e)
        {
            if (!token.IsCancellationRequested)
            {
                OnError?.Invoke(e);
                Disconnect();
            }
        }
    }

    private void ProcessReceiveData(byte[] data, int length)
    {
        Buffer.BlockCopy(data, 0, packetBuffer, bufferOffset, length);
        bufferOffset += length;
        int processed = 0;
        while (bufferOffset - processed >= 4)
        {
            int packetLength = BitConverter.ToInt32(packetBuffer, processed);
            if (bufferOffset - processed >= 4 + packetLength)
            {
                byte[] packet = new byte[packetLength];
                Buffer.BlockCopy(packetBuffer, processed + 4, packet, 0, packetLength);
                receiveQueue.Enqueue(packet);
                processed += 4 + packetLength;
            }
            else
            {
                break;
            }
        }

        if (processed > 0)
        {
            int remaining = bufferOffset - processed;
            if (remaining > 0)
            {
                Buffer.BlockCopy(packetBuffer, processed, packetBuffer, 0, remaining);
                
            }

            bufferOffset = remaining;
        }
    }

}

