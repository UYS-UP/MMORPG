using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TcpTransport : ITransport
{
    private TcpClient tcpClient;
    private NetworkStream stream;
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private bool isConnected = false;
    public bool IsConnected => isConnected && tcpClient?.Connected == true;
    
    public event Action<ReadOnlyMemory<byte>> OnDataReceived;
    public event Action OnConnectionClosed;
    public event Action<Exception> OnConnectionError;

    
    /// <summary>
    /// 连接到服务器
    /// </summary>
    public async UniTask ConnectAsync(EndPoint serverEndPoint)
    {
        if (IsConnected)
        {
            Debug.LogWarning("TCP客户端已经连接");
            return;
        }
            
        try
        {
            tcpClient = new TcpClient();
                
            if (serverEndPoint is IPEndPoint ipEndPoint)
            {
                // 使用UniTask包装异步连接操作
                await tcpClient.ConnectAsync(ipEndPoint.Address, ipEndPoint.Port).AsUniTask();
                    
                stream = tcpClient.GetStream();
                isConnected = true;
                    
                Debug.Log($"TCP客户端已连接到服务器：{serverEndPoint}");
                    
                // 启动接收数据的任务
                StartReceiveLoop().Forget();
            }
            else
            {
                throw new ArgumentException("服务器终结点必须是IPEndPoint类型");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"TCP连接失败：{ex.Message}");
            OnConnectionError?.Invoke(ex);
            throw;
        }
    }
    
    private async UniTaskVoid StartReceiveLoop()
    {
        var buffer = new byte[4096];
            
        try
        {
            while (IsConnected && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                // 使用UniTask包装异步读取操作
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationTokenSource.Token).AsUniTask();
                    
                if (bytesRead == 0)
                {
                    Debug.Log("服务器关闭了TCP连接");
                    break;
                }
                    
                // 触发数据接收事件
                OnDataReceived?.Invoke(new ReadOnlyMemory<byte>(buffer, 0, bytesRead));
                    
                Debug.Log($"TCP接收数据：{bytesRead} 字节");
            }
        }
        catch (OperationCanceledException)
        {
            // 正常取消操作
            Debug.Log("TCP接收循环已取消");
        }
        catch (Exception ex)
        {
            Debug.LogError($"TCP接收数据时发生错误：{ex.Message}");
            OnConnectionError?.Invoke(ex);
        }
        finally
        {
            // 确保连接被正确关闭
            if (IsConnected)
            {
                Disconnect();
            }
        }
    }
    
    /// <summary>
    /// 断开连接
    /// </summary>
    public void Disconnect()
    {
        if (!IsConnected)
        {
            return;
        }
            
        try
        {
            isConnected = false;
            cancellationTokenSource.Cancel();
                
            stream?.Close();
            tcpClient?.Close();
                
            Debug.Log("TCP客户端已断开连接");
            OnConnectionClosed?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError($"TCP断开连接时发生错误：{ex.Message}");
        }
    }
    
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="data">要发送的数据</param>
    public async UniTask SendAsync(ReadOnlyMemory<byte> data)
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("TCP客户端未连接");
        }
            
        try
        {
            // 使用UniTask包装异步写入操作
            await stream.WriteAsync(data.ToArray(), 0, data.Length, cancellationTokenSource.Token).AsUniTask();
            await stream.FlushAsync(cancellationTokenSource.Token).AsUniTask();
                
            Debug.Log($"TCP发送数据：{data.Length} 字节");
        }
        catch (Exception ex)
        {
            Debug.LogError($"TCP发送数据失败：{ex.Message}");
            OnConnectionError?.Invoke(ex);
            throw;
        }
    }
    
    public void Dispose()
    {
        Disconnect();
        cancellationTokenSource?.Dispose();
    }

    
    // private void ProcessReceiveData(byte[] data, int length)
    // {
    //     // 1. 将新数据拷贝到缓冲区
    //     if (bufferOffset + length > packetBuffer.Length)
    //     {
    //         // 缓冲区不足时扩容（或断开连接）
    //         Array.Resize(ref packetBuffer, bufferOffset + length);
    //     }
    //     Buffer.BlockCopy(data, 0, packetBuffer, bufferOffset, length);
    //     bufferOffset += length;
    //
    //     // 2. 解析完整消息
    //     int pos = 0;
    //     while (pos < bufferOffset)
    //     {
    //         // 检查是否足够读取长度字段
    //         if (bufferOffset - pos < 2)
    //         {
    //             break; // 半包，等待更多数据
    //         }
    //
    //         // 读取2字节长度（大端序）
    //         int packetLength = BinaryPrimitives.ReadUInt16BigEndian(new ReadOnlySpan<byte>(packetBuffer, pos, 2));
    //
    //         // 检查是否足够读取完整消息
    //         if (bufferOffset - pos < 2 + packetLength)
    //         {
    //             break; // 半包，等待更多数据
    //         }
    //
    //         // 提取消息数据
    //         byte[] packetData = new byte[packetLength];
    //         Buffer.BlockCopy(packetBuffer, pos + 2, packetData, 0, packetLength);
    //         receiveQueue.Enqueue(packetData);
    //
    //         // 移动处理位置
    //         pos += 2 + packetLength;
    //     }
    //
    //     // 3. 处理剩余数据
    //     if (pos > 0)
    //     {
    //         int remaining = bufferOffset - pos;
    //         if (remaining > 0)
    //         {
    //             Buffer.BlockCopy(packetBuffer, pos, packetBuffer, 0, remaining);
    //         }
    //         bufferOffset = remaining;
    //     }
    // }

}

