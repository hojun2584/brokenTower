using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignForm : MonoBehaviour
{

    [SerializeField]
    TMP_InputField userId;

    [SerializeField]
    TMP_InputField userPw;
    [SerializeField]
    Button signButton;

    public event Action signBtnClick;

    public string UserId => userId.text;

    public string UserPw => userPw.text;

    private void Awake()
    {
        if (userId == null || userPw == null)
        {
            Debug.Log("userId, uwerPw not set");
        }
        signButton.onClick.AddListener(() => { signBtnClick(); });
    }
}
