using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hojun;
using CustomPacket;

namespace Hojun
{


    public class GamePlayManager : MonoBehaviour
    {
        public CustomPriorityQue<Tower> towers = new CustomPriorityQue<Tower> ( ( x , y ) => { return x.towerPriority > y.towerPriority; } );
        public List<Node> nodes = new List<Node> ();

        public Tower enemyTower;
        public Tower allieTower;

        public GameObject warrior;
        public Dictionary<int , GameObject> spawnDict = new Dictionary<int , GameObject> ();

        static GamePlayManager instance;
        public static GamePlayManager Instance { get => instance; }

        public int setSpawnObjet;

        public void Awake()
        {
            spawnDict[0] = warrior;
        }

        public void Start()
        {
            instance = this;
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
            if (Input.GetMouseButtonDown(1)) // ���콺 ���� �� ��ư Ŭ��
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


                        // �� ���� ��Ʈ�� ������ position ����� �ؼ� ���̶� ���� �ִµ� �׳� �ڵ������ ���� �ذ� ����!
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

            //if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư Ŭ�� ����
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