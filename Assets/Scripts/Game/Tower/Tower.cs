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


        public void Hit(float hitObject)
        {
            throw new System.NotImplementedException();
        }



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}