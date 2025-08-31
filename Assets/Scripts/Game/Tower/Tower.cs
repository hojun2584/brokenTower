using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hojun;


namespace Hojun
{

    public class Tower : MonoBehaviour ,IHitAble
    {
        public Node currentNode;
        public int towerPriority;

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
        }


    }
}