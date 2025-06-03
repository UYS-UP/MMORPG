using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    private TMP_InputField usernameInput;
    private TMP_InputField passwordInput;
    private Button loginButton;
    private Button registerButton;
    private Button quitGameButton;

    protected override void Awake()
    {
        base.Awake();
        usernameInput = transform.Find("UsernameInput").GetComponent<TMP_InputField>();
        passwordInput = transform.Find("PasswordInput").GetComponent<TMP_InputField>();
        loginButton = transform.Find("LoginButton").GetComponent<Button>();
        registerButton = transform.Find("RegisterButton").GetComponent<Button>();
        quitGameButton = transform.Find("QuitGameButton").GetComponent<Button>();
        
        registerButton.onClick.AddListener(OnRegisterClick);
        loginButton.onClick.AddListener(OnLoginClick);
        quitGameButton.onClick.AddListener(OnQuitGameClick);
    }
    
    private void OnLoginClick()
    {
        if (string.IsNullOrEmpty(usernameInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            Debug.Log("用户名或密码不能为空");
            return;
        }
    }

    private void OnRegisterClick()
    {
        
    }
    
    private void OnQuitGameClick()
    {
        Application.Quit();
    }
}
