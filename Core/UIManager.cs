using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public enum PanelType
{
    LoginPanel
}

public enum UILayer
{
    Background = 0,    // 背景层（最低优先级）
    Common = 100,      // 普通面板层
    Popup = 200,       // 弹出窗口层
    Toast = 300,       // 提示信息层
    Loading = 400,     // 加载层
    System = 500,      // 系统级（最高优先级）
}

public class UIManager : SingletonMono<UIManager>
{
    private readonly Dictionary<PanelType, BasePanel> panelDict = new Dictionary<PanelType, BasePanel>();
    private readonly Dictionary<UILayer, RectTransform> layerContainers = new Dictionary<UILayer, RectTransform>();
    
    public Canvas Canvas { get; private set; }
    public RectTransform CanvasRect { get; private set; }

    private void Awake()
    {
        InitializeCanvas("Prefabs/UI/Canvas");
        InitializeEventSystem("Prefabs/UI/EventSystem");
    }
    
    private void InitializeCanvas(string path)
    {
        GameObject canvasObj = Instantiate(Resources.Load<GameObject>(path));
        CanvasRect = canvasObj.transform.GetComponent<RectTransform>();
        Canvas = canvasObj.GetComponent<Canvas>();
        DontDestroyOnLoad(canvasObj);
        InitializeLayerContainers();
    }

    private void InitializeEventSystem(string path)
    {
        GameObject eventSystemObj = Instantiate(Resources.Load<GameObject>(path));
        DontDestroyOnLoad(eventSystemObj);
    }
    
    private void InitializeLayerContainers()
    {
        foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
        {
            GameObject layerObj = new GameObject(layer + "Layer");
            RectTransform layerRect = layerObj.AddComponent<RectTransform>();
            layerRect.SetParent(CanvasRect, false);
            layerRect.anchorMin = Vector2.zero;
            layerRect.anchorMax = Vector2.one;
            layerRect.offsetMin = Vector2.zero;
            layerRect.offsetMax = Vector2.zero;
            layerContainers[layer] = layerRect;

            // 设置层级顺序
            Canvas layerCanvas = layerObj.AddComponent<Canvas>();
            layerCanvas.overrideSorting = true;
            layerCanvas.sortingOrder = (int)layer;
        }
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="panelType">面板类型</param>
    /// <param name="onBegin">开始回调</param>
    /// <param name="onComplete">结束回调</param>
    /// <param name="fadeOut">是否开启淡出效果</param>
    public void ShowPanel(PanelType panelType, Action onBegin = null, Action onComplete = null,
        bool fadeOut = false)
    {
        if (!panelDict.TryGetValue(panelType, out var panel)) return;
        if(panel.isVisible) return;

        panel.ShowMe(onBegin, onComplete, fadeOut);
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="panelType">面板类型</param>
    /// <param name="onBegin">开始回调</param>
    /// <param name="onComplete">结束回调</param>
    /// <param name="fadeIn">是否开启淡入效果</param>
    public void HidePanel(PanelType panelType, Action onBegin = null, Action onComplete = null,
        bool fadeIn = false)
    {
        if (!panelDict.TryGetValue(panelType, out var panel)) return;
        if(!panel.isVisible) return;
        panel.HideMe(onBegin, onComplete, fadeIn);
    }

    /// <summary>
    /// 添加面板
    /// </summary>
    /// <param name="panelType">面板类型</param>
    /// <param name="layer">面板层级</param>
    public void AddPanel(PanelType panelType, UILayer layer = UILayer.Common)
    {
        if (panelDict.ContainsKey(panelType)) return;

        GameObject panelObj = Object.Instantiate(
            Resources.Load<GameObject>($"Prefabs/UI/{panelType.ToString()}"), 
            layerContainers[layer], 
            false
        );
        BasePanel panel = panelObj.GetComponent<BasePanel>();
        panel.layer = layer;
        panelDict[panelType] = panel;
    }
    /// <summary>
    /// 删除面板
    /// </summary>
    /// <param name="panelType">面板类型</param>
    public void RemovePanel(PanelType panelType)
    {
        if(!panelDict.ContainsKey(panelType)) return;
        Destroy(panelDict[panelType].gameObject);
        panelDict.Remove(panelType);
        
    }

    /// <summary>
    /// 移除所有面板
    /// </summary>
    public void RemoveAllPanel()
    {
        foreach (var panel in panelDict.Values)
        {
            Destroy(panel.gameObject);
        }
        panelDict.Clear();
    }

    /// <summary>
    /// 改变渲染层级
    /// </summary>
    /// <param name="panelType">面板类型</param>
    /// <param name="newLayer">层级</param>
    public void ChangeLayer(PanelType panelType, UILayer newLayer)
    {
        if(!panelDict.TryGetValue(panelType, out var panel)) return;
        if(!layerContainers.TryGetValue(newLayer, out var newParent)) return;
        
        panel.UpdateLayer(newLayer, newParent);
        // panel.transform.SetAsLastSibling();
    }
    
    
    
}
