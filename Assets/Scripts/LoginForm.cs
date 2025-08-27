using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LoginForm : MonoBehaviour
{

    [SerializeField]
    TMP_InputField userId;

    [SerializeField]
    TMP_InputField userPw;

    [SerializeField]
    Button loginBtn;

    public event Action loginBtnClick;

    public string UserId => userId.text;

    public string UserPw => userPw.text;

    private void Awake()
    {
        if (userId == null || userPw == null)
        {
            Debug.Log("userId, uwerPw not set");
        }
        loginBtn.onClick.AddListener(() => { loginBtnClick();});
    }

}
