using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.AddPanel(PanelType.LoginPanel);
        UIManager.Instance.ShowPanel(PanelType.LoginPanel);
    }
}
