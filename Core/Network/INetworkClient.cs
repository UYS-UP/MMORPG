using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface INetwork
{
    UniTaskVoid Connect();
    void Send(byte[] data);
    void Disconnect();
    
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<byte[]> OnDataReceived;
    public event Action<Exception> OnError;
}
