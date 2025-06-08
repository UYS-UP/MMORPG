using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ITransoport
{
    /// <summary>
    /// 连接到服务器
    /// </summary>
    /// <param name="serverEndPoint">服务器终结点</param>
    /// <returns>表示异步操作的UniTask</returns>
    UniTask ConnectAsync(EndPoint serverEndPoint);
        
    /// <summary>
    /// 断开连接
    /// </summary>
    void Disconnect();
        
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="data">要发送的数据</param>
    /// <returns>表示异步操作的UniTask</returns>
    UniTask SendAsync(ReadOnlyMemory<byte> data);
        
    /// <summary>
    /// 是否已连接
    /// </summary>
    bool IsConnected { get; }
        
    /// <summary>
    /// 数据接收事件
    /// </summary>
    event Action<ReadOnlyMemory<byte>> OnDataReceived;
        
    /// <summary>
    /// 连接关闭事件
    /// </summary>
    event Action OnConnectionClosed;
        
    /// <summary>
    /// 连接错误事件
    /// </summary>
    event Action<Exception> OnConnectionError;
}
