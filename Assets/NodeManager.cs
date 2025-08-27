using Hojun;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class NodeConnector : MonoBehaviour
{
    // Start is called before the first frame update

    int nodeId = 0;

    public List<Node> nodeList = new List<Node>();
    public GamePlayManager gamePlayManager;

    void Start()
    {
        foreach (Transform child in gameObject.transform)
        {
            child.GetComponent<LandManager>().SetLandId(ref nodeId, nodeList);
        }

        gamePlayManager.nodes = nodeList;
    }

}
