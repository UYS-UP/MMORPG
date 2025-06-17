using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    private Transform loginTransform;
    private Transform registerTransform;
    private TMP_InputField loginUsernameInput;
    private TMP_InputField loginPasswordInput;
    private TMP_InputField registerUsernameInput;
    private TMP_InputField registerPasswordInput;
    private TMP_InputField registerRePasswordInput;
    private TMP_InputField registerCodeInput;
    private Button sendCodeButton;
    private Button loginButton;
    private Button registerButton;
    private Button quitGameButton;

    protected override void Awake()
    {
        base.Awake();
        loginTransform = transform.Find("Login");
        loginUsernameInput = loginTransform.Find("UsernameInput").GetComponent<TMP_InputField>();
        loginPasswordInput = loginTransform.Find("PasswordInput").GetComponent<TMP_InputField>();
        loginButton = loginTransform.Find("LoginButton").GetComponent<Button>();
        
        registerTransform = transform.Find("Register");
        registerUsernameInput = registerTransform.Find("UsernameInput").GetComponent<TMP_InputField>();
        registerPasswordInput = registerTransform.Find("PasswordInput").GetComponent<TMP_InputField>();
        registerRePasswordInput = registerTransform.Find("RePasswordInput").GetComponent<TMP_InputField>();
        registerCodeInput = registerTransform.Find("CodeInput").GetComponent<TMP_InputField>();
        registerButton = registerTransform.Find("RegisterButton").GetComponent<Button>();
        
        quitGameButton = transform.Find("QuitGameButton").GetComponent<Button>();
        
        registerButton.onClick.AddListener(OnRegisterClick);
        loginButton.onClick.AddListener(OnLoginClick);
        quitGameButton.onClick.AddListener(OnQuitGameClick);
        
        loginTransform.gameObject.SetActive(true);
        registerTransform.gameObject.SetActive(false);
    }

    private void Start()
    {
        GameClient.Instance.RegisterHandler((ushort)Protocol.Login, OnLoginResponse);
    }

    private async void OnLoginClick()
    {
        if (string.IsNullOrEmpty(loginUsernameInput.text) || string.IsNullOrEmpty(loginPasswordInput.text))
        {
            Debug.Log("用户名或密码不能为空");
            return;
        }

        byte[] bytes = MessagePackSerializer.Serialize(new LoginData
        {
            Username = "haha@123.com",
            Password = "12345678"
        });
        await GameClient.Instance.SendPacketAsync(new GamePacket((ushort)Protocol.Login, bytes));
    }

    private void OnLoginResponse(GamePacket packet)
    {
        
    }

    private void OnRegisterClick()
    {
        
    }
    
    private void OnQuitGameClick()
    {
        Application.Quit();
    }
}
