using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoViewer : MonoBehaviour
{

    public Sprite readyToIcon;
    public Image playerIcon;

    public string PlayerName 
    { 
        set 
        {
            playerName.text = value;
        }
    }
    public TextMeshProUGUI playerName;

    public event Action playerInfoSet;
    public PlayerStruct PlayerInfo 
    {
        get => playerInfo;
        set
        {
            playerInfo = value;
            playerInfoSet?.Invoke();
        }
    }
    PlayerStruct playerInfo;

    public void Awake()
    {
        playerInfoSet += PlayerInfoView;
    }
    
    public void PlayerInfoView()
    {
        playerName.text = PlayerInfo.playerName;
        playerIcon.sprite = readyToIcon;
    }


}
