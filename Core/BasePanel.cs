using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    public PanelType panelType;
    public UILayer layer = UILayer.Common;
    public CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;
    [HideInInspector] public bool isVisible;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        // 设置 Canvas 的 sortingOrder，基于层级
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        if (GetComponent<GraphicRaycaster>() == null) gameObject.AddComponent<GraphicRaycaster>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = (int)layer;
        isVisible = false;
    }

    /// <summary>
    /// 显示该面板
    /// </summary>
    /// <param name="onBegin">开始回调</param>
    /// <param name="onComplete">结束回调</param>
    /// <param name="fadeIn">是否开启淡入效果</param>
    public virtual void ShowMe(Action onBegin = null, Action onComplete = null, bool fadeIn = false)
    {
        if(canvasGroup == null) return;
        onBegin?.Invoke();
        
        if (fadeIn)
        {
            canvasGroup
                .DOFade(1f, fadeDuration)
                .From(0f)
                .OnComplete(() =>
                {
                    onComplete?.Invoke(); 
                    gameObject.SetActive(true);
                })
                .SetUpdate(true);
        }else
        { 
            canvasGroup.alpha = 1f;
            gameObject.SetActive(true);
        }

        isVisible = true;

    }

    /// <summary>
    /// 隐藏该面板
    /// </summary>
    /// <param name="onBegin">开始回调</param>
    /// <param name="onComplete">结束回调</param>
    /// <param name="fadeOut">是否开启淡出效果</param>
    public virtual void HideMe(Action onBegin = null, Action onComplete = null, bool fadeOut = false)
    {
        if(canvasGroup == null) return;
        
        onBegin?.Invoke();
        if (fadeOut)
        {
            canvasGroup.DOFade(0f, fadeDuration)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                    gameObject.SetActive(false);
                })
                .SetUpdate(true);
        }else
        {
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        isVisible = false;
        

    }
    
    /// <summary>
    /// 更新面板层级
    /// </summary>
    /// <param name="newLayer">层级</param>
    /// <param name="newParent">层级父物体</param>
    public void UpdateLayer(UILayer newLayer, RectTransform newParent)
    {
        layer = newLayer;
        transform.SetParent(newParent, false);
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = (int)newLayer;
        }
    }
}
