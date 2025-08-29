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
        }
        
        public void GameCameraSetting()
        {
            if (LobbyManager.Instance.CurrentGameRoom.roomMaster != NetworkManager.instance.session.SessionId)
                gameCamera.CameraViewSetting();
        }




        public void SpawnCharacter(Node spawn)
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


        void Update()
        {
            if (Input.GetMouseButtonDown(1)) // 마우스 오른 쪽 버튼 클릭
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.TryGetComponent<Node>(out Node spawn))
                    {

                        Debug.Log("Spawn Node ID: " + spawn.NodeId);

                        SummondPacket packet = new SummondPacket();
                        packet.Init(spawn.NodeId , LobbyManager.Instance.CurrentGameRoom.roomNum);

                        NetworkManager.instance.session.Send(packet.Write());


                        // 이 둘이 세트임 워리어 position 안잡고 해서 땅이랑 겹쳐 있는데 그냥 코드상으로 대충 해결 했음!
                        //Vector3 spawnPosition = spawn.GetPositionSetY(5f);
                        //GameObject spawnCharater = Instantiate(spawnDict[setSpawnObjet], spawnPosition, Quaternion.identity);

                        //if (spawnCharater.TryGetComponent<Summoned>(out Summoned summonObj))
                        //{
                        //    summonObj.gamePlayManager = this;
                        //    summonObj.currentNode = spawn;
                        //    summonObj.targetNode = enemyTower.currentNode;
                        //}

                    }
                }

            }

            //if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 감지
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    RaycastHit hit;
            //    if (Physics.Raycast(ray, out hit))
            //    {
            //        if (hit.transform.TryGetComponent<Node>(out Node spawn))
            //        {
            //            Vector3 spawnPosition = spawn.GetPositionSetY(5f);
            //            GameObject spawnCharater = Instantiate(spawnDict[setSpawnObjet], spawnPosition, Quaternion.identity);

            //            if (spawnCharater.TryGetComponent<Summoned>(out Summoned summonObj))
            //            {
            //                summonObj.gamePlayManager = this;
            //                summonObj.currentNode = spawn;
            //                summonObj.targetNode = allieTower.currentNode;
            //            }

            //        }
            //    }

            //}

        }

        public void OnDestroy()
        {
            instance = null;
        }


    }
}