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
            //packet.Init(GameManager.Instance.CurrentPlayer.playerName);

            NetworkManager.instance.session.Send(packet.Write());

        }

    }
}