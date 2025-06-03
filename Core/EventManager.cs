using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    private readonly Dictionary<Type, EventHandlerWrapper> eventHandlers = new Dictionary<Type, EventHandlerWrapper>();
    private readonly Dictionary<Type, List<EventRecord>> eventHistory = new Dictionary<Type, List<EventRecord>>();
    
    /// <summary>
    /// 包装类
    /// 所有事件参数必须继承自EventArgs
    /// </summary>
    private class EventHandlerWrapper
    {
        private event EventHandler<EventArgs> handler;

        public void AddHandler(EventHandler<EventArgs> handler)
        {
            this.handler += handler;
        }

        public void RemoveHandler(EventHandler<EventArgs> handler)
        {
            this.handler -= handler;
        }

        public void Invoke(object sender, EventArgs eventArgs)
        {
            this.handler?.Invoke(sender, eventArgs);
        }
        
        public bool HasHandlers =>  handler!= null;
        
    }

    public class EventRecord
    {
        public DateTime Timestamp { get; } = DateTime.Now;
        public object Sender { get; }
        public EventArgs Args { get; }
        public string CallerInfo { get; }

        public EventRecord(object sender, EventArgs args, string callerInfo)
        {
            Sender = sender;
            Args = args;
            CallerInfo = callerInfo;
        }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss.fff}] {CallerInfo} - Sender: {Sender?.GetType().Name}, Args: {Args}";
        }
    }

    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <typeparam name="T">EventArgs类型</typeparam>
    public void SubScribe<T>(EventHandler<T> handler) where T : EventArgs
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        var eventType = typeof(T);
        if (!eventHandlers.TryGetValue(eventType, out var wrapper))
        {
            wrapper = new EventHandlerWrapper();
            eventHandlers[eventType] = wrapper;
        }
        wrapper.AddHandler((sender, e) => handler(sender, (T)e));
    }

    /// <summary>
    /// 取消订阅事件
    /// </summary>
    public void UnsubScribe<T>(EventHandler<T> handler) where T : EventArgs
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        var eventType = typeof(T);
        if (eventHandlers.TryGetValue(eventType, out var wrapper))
        {
            wrapper.RemoveHandler((sender, e) => handler(sender, (T)e));
            
            if (!wrapper.HasHandlers)
            {
                eventHandlers.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    public void Publish<T>(object sender, T args) where T : EventArgs
    {
        if (args == null) throw new ArgumentNullException(nameof(args));

        var eventType = typeof(T);
        if (!eventHandlers.TryGetValue(eventType, out var wrapper))
            return;

        // 记录事件历史
        var stackTrace = new StackTrace(1, true);
        var callerFrame = stackTrace.GetFrame(0);
        var callerInfo = $"{callerFrame?.GetMethod()?.DeclaringType?.Name}.{callerFrame?.GetMethod()?.Name}";

        if (!eventHistory.TryGetValue(eventType, out var historyList))
        {
            historyList = new List<EventRecord>();
            eventHistory[eventType] = historyList;
        }

        historyList.Add(new EventRecord(sender, args, callerInfo));
        wrapper.Invoke(sender, args);
    }

    /// <summary>
    /// 获取事件触发历史
    /// </summary>
    public IReadOnlyList<EventRecord> GetEventHistory<T>() where T : EventArgs
    {
        return eventHistory.TryGetValue(typeof(T), out var history) 
            ? history.AsReadOnly() 
            : new List<EventRecord>().AsReadOnly();
    }

    /// <summary>
    /// 清空指定类型的事件历史
    /// </summary>
    public void ClearHistory<T>() where T : EventArgs
    {
        eventHistory.Remove(typeof(T));
    }

    /// <summary>
    /// 清空所有事件历史
    /// </summary>
    public void ClearAllHistory()
    {
        eventHistory.Clear();
    }

}
