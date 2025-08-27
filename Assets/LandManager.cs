using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hojun;

public class LandManager : MonoBehaviour
{

    public bool SetLandId(ref int nextId , List<Node> nodeList)
    {
        foreach (Transform child in gameObject.transform)
        {
            Node node = child.GetComponent<Node>();
            if (node != null)
            {
                node.NodeId = nextId;
                nodeList.Add(node);
                nextId++;
            }
        }

        return true;
    }

}
