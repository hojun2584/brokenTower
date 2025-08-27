using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

namespace Hojun
{

    public class Node : MonoBehaviour
    {
        [SerializeField]
        int nodeId;
        public int NodeId { get => nodeId; set { nodeId = value; } }

        public float heuristic;
        public float weight;
        public Renderer render;
        public Node previousNode;

        public bool towerFlag;

        public float Pathcost { get { return (heuristic); } }
        public float enterCost = 1f;

        public float distanceX = 1f;
        public float distanceZ = 1f;

        public void Clear()
        {
            heuristic = 2f;
            weight = 0f;
            previousNode = null;
            enterCost = 1f;
        }


        public Vector3 GetPosition
        {
            get { return gameObject.transform.position; }
        }

        public void Awake()
        {
            AstarAlgorithm.Instacne.nodeList.Add(this);
        }

        public void Start()
        {
            render = GetComponent<Renderer>();

        }

        public Vector3 GetPositionSetY(float y = 0.5f)
        {
            Vector3 pos = new Vector3(transform.position.x, y, transform.position.z);

            return pos;
        }

        public bool IsNeighbor(Node neighbor)
        {
            Vector3 neighborPos = neighbor.GetPositionSetY();
            float dx = Mathf.Abs(GetPositionSetY().x - neighborPos.x);
            float dz = Mathf.Abs(GetPositionSetY().z - neighborPos.z);

            if ((dx <= distanceX && dz == 0) || (dz <= distanceZ && dx == 0))
                return true;


            return false;
        }

        public bool IsSqureNeighbor(Node neighbor)
        {
            Vector3 neighborPos = neighbor.GetPositionSetY();
            float dx = Mathf.Abs(GetPositionSetY().x - neighborPos.x);
            float dz = Mathf.Abs(GetPositionSetY().z - neighborPos.z);

            if (dx <= distanceX && dz <= distanceZ && dx != 0 && dz != 0)
                return true;

            return false;
        }


        public void ChangeColor()
        {
            render.material.color = Color.white;
        }

    }


}
