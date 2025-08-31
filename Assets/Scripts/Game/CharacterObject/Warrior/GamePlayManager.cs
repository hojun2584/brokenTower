using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hojun;
using CustomPacket;
using System;
using CustomClient;
using Unity.VisualScripting;

namespace Hojun
{


    public class GamePlayManager : MonoBehaviour
    {
        

        public List<Node> nodes = new List<Node> ();
        public GameRoomCameraController gameCamera;

        public List<Tower> towers = new List<Tower> ();
        public GameObject warrior;
        public Dictionary<int , GameObject> spawnDict = new Dictionary<int , GameObject> ();
        public Action gameSetting;

        public Tower roomMasterTower;
        public Tower visitorTower;



        public bool IsRoomMaster { get => NetworkManager.instance.session.SessionId == LobbyManager.Instance.CurrentGameRoom.roomMasterSessionId;}

        public bool isGameSetting;

        static GamePlayManager instance;
        public static GamePlayManager Instance { get => instance; }
        public int setSpawnObjet;

        public void Awake()
        {
            instance = this;
            spawnDict[0] = warrior;
            gameSetting += GameCameraSetting;
        }

        public void Start()
        {
            if(instance == null)
                instance = this;
            gameSetting?.Invoke();
            TowerSwap();
        }

        public void TowerSwap()
        {
            bool towerSetting = LobbyManager.Instance.CurrentGameRoom.roomMasterSessionId == NetworkManager.instance.session.SessionId;
        }
        
        public void GameCameraSetting()
        {
            if (LobbyManager.Instance.CurrentGameRoom.roomMasterSessionId != NetworkManager.instance.session.SessionId)
                gameCamera.CameraViewSetting();
        }


        public void GameAlieSetting(GameObject summondObj)
        {
            IAttackAble attacker = summondObj.GetComponent<IAttackAble>();
        }


        public void SpawnCharacter(Node spawn , int playerSessionId)
        {

            Vector3 spawnPosition = spawn.GetPositionSetY(5f);
            GameObject spawnCharater = Instantiate(spawnDict[setSpawnObjet], spawnPosition, Quaternion.identity);
            
            if (spawnCharater.TryGetComponent<Summoned>(out Summoned summonObj))
            {
                summonObj.gamePlayManager = this;
                summonObj.currentNode = spawn;

                bool towerSetting = LobbyManager.Instance.CurrentGameRoom.roomMasterSessionId == NetworkManager.instance.session.SessionId;
                

                summonObj.InitSummon();
            }
        }

        public Tower GetEnemyTower()
        {
            
            return towers[0];
        }


        public void SummonCharacter()
        {

            if (Input.GetMouseButtonDown(0)) // 마우스 오른 쪽 버튼 클릭
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.TryGetComponent<Node>(out Node spawn))
                    {

                        Debug.Log("Spawn Node ID: " + spawn.NodeId);

                        SummondPacket packet = new SummondPacket();
                        packet.Init(spawn.NodeId, LobbyManager.Instance.CurrentGameRoom.roomNum, NetworkManager.instance.session.SessionId);

                        NetworkManager.instance.session.Send(packet.Write());
                    }
                }
            }
        }

        void Update()
        {
            SummonCharacter();
        }

        public void OnDestroy()
        {
            instance = null;
        }


    }
}