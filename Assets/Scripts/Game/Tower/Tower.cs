using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hojun;
using CustomPacket;


namespace Hojun
{

    public class Tower : MonoBehaviour ,IHitAble
    {
        public Node currentNode;
        public int towerPriority;
        public string ownerPlayerName;

        public bool isAlieTower;

        [SerializeField]
        float hPoint= 50;

        public float HPoint
        {
            get
            {
                return hPoint;
            }

            set
            {
                if (value <= 0)
                {
                    Debug.Log("dead");
                    gameObject.SetActive(false);
                }
                hPoint = value;
            }
        }

        public void Hit(float hitObject)
        {
            HPoint -= hitObject;
            if(HPoint <= 0)
                GameOver();
            
        }

        public void GameOver()
        {
            Debug.Log("GameOver");
            
            GameEndPacket packet = new GameEndPacket();
            packet.Init(LobbyManager.Instance.CurrentGameRoom.roomNum , NetworkManager.instance.session.SessionId, !isAlieTower);

            NetworkManager.instance.session.Send(packet.Write());

        }

    }
}