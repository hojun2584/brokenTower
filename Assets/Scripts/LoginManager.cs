using CustomClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPacket;

public class LoginManager : MonoBehaviour
{

    [SerializeField]
    GameObject login_UI;

    [SerializeField]
    GameObject sign_UI;

    [SerializeField]
    LoginForm loginData;

    [SerializeField]
    SignForm signData;

    public void Awake()
    {
        loginData = login_UI.GetComponent<LoginForm>();
        signData = sign_UI.GetComponent<SignForm>();

        if (loginData == null)
            Debug.Log("fail Getcomponenet loginData");

        if (signData == null)
            Debug.Log("fail Getcomponenet signData");
    }

    public void Start()
    {
        loginData.loginBtnClick += Login;
        signData.signBtnClick += Sign;
    }

    public void ShowLoginForm()
    {
        login_UI.SetActive(true);
    }
    public void ShowSignForm()
    {
        sign_UI.SetActive(true);
    }

    public void HideSignForm()
    {
        sign_UI.SetActive(false);
    }

    public void HideLoginForm()
    {
        login_UI.SetActive(false);
    }
    
    public void Login()
    {
        LoginPacket loginPacket = new LoginPacket();
        loginPacket.Init(loginData.UserId , loginData.UserPw);
        NetworkManager.instance.session.Send( loginPacket.Write() );
    }

    public void Sign()
    {
        PlayerSignPacket packet = new PlayerSignPacket();

        packet.Init(signData.UserId, signData.UserPw);
        NetworkManager.instance.session.Send(packet.Write());
    }


}
