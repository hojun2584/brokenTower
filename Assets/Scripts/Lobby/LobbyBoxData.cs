using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyBoxData : MonoBehaviour
{
    public TextMeshProUGUI roomNameText;
    public event Action buttonClick;

    public void ButtonClick()
    {
        buttonClick.Invoke();
    }
}
