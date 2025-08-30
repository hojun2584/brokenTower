using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hojun;
using CustomPacket;
using System;
using CustomClient;

namespace Hojun
{


    public class GamePlayManager : MonoBehaviour
    {
        public CustomPriorityQue<Tower> towers = new CustomPriorityQue<Tower> ( ( x , y ) => { return x.towerPriority > y.towerPriority; } );
        public List<Node> nodes = new List<Node> ();

        public GameRoomCameraController gameCamera;

        public Tower enemyTower;
        public Tower allieTower;

        public GameObject warrior;
        public Dictionary<int , GameObject> spawnDict = new Dictionary<int , GameObject> ();

        public Action gameSetting;

        static GamePlayManager instance;
        public static GamePlayManager Instance { get => instance; }

        public int setSpawnObjet;

        public void Awake()
        {
            spawnDict[0] = warrior;
            gameSetting += GameCameraSetting;
        }

        public void Start()
        {
            instance = this;
            gameSetting?.Invoke();

            if (LobbyManager.Instance.CurrentGameRoom.roomMasterSessionId != NetworkManager.instance.session.SessionId)
            {
                Tower swaper = enemyTower;
                enemyTower = allieTower;
                allieTower = swaper;
            }
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
                summonObj.targetNode = enemyTower.currentNode;
            }
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
                        //packet.Init(spawn.NodeId, LobbyManager.Instance.CurrentGameRoom.roomNum);

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