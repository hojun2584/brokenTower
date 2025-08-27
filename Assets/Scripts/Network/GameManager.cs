using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    string lobbySceneName;
    [SerializeField]
    PlayerStruct currentPlayer;

    public PlayerStruct CurrentPlayer 
    {
        get 
        {
            return currentPlayer;
        }
        set
        {
            currentPlayer = value;
        }
    }


    public static GameManager Instance;
    public event Action enterLobby;


    private Dictionary<string, Action> loadSceneAction = new Dictionary<string, Action>();

    // Start is called before the first frame update
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        loadSceneAction[lobbySceneName] = enterLobby;
        SceneManager.sceneLoaded += LoadSceneCallEvent;
    }

    public void LoadSceneCallEvent(Scene scene, LoadSceneMode mode)
    {
        if (loadSceneAction.ContainsKey(scene.name))
            loadSceneAction[scene.name]?.Invoke();
    }



}
