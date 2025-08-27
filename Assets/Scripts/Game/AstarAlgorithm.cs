using Hojun;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarAlgorithm
{
    public static AstarAlgorithm Instacne = new AstarAlgorithm();
    public CustomPriorityQue<Node> priorityQue;

    public float squreMoveWeight = 1.3f;

    public List<Node> nodeList = new List<Node>();
    List<Node> cubeRoute = new List<Node>();
    List<Node> closeList;


    float Heuristic(Vector3 current, Vector3 target)
    {
        return Vector3.Distance(current, target);
    }

    public List<Node> FindPath(Node start , Node goal , List<Node> nodes)
    {

        Instacne.priorityQue = new CustomPriorityQue<Node> ( (x,y) => { return x.Pathcost + x.enterCost > y.Pathcost + y.enterCost; } );
        closeList = new List<Node> ();

        start.heuristic = Heuristic(start.GetPositionSetY() , goal.GetPositionSetY());
        start.weight = 0;

        Instacne.priorityQue.Enque(start);

        List<Node> path = new List<Node>();

        while (Instacne.priorityQue.Count > 0) 
        {
            Node way = Instacne.priorityQue.Deque();

            if (closeList.Contains(way))
                continue;

            closeList.Add(way);

            if (way == goal)
            {
                List<Node> route = new List<Node>();

                while(way.previousNode != null)
                {
                    route.Add(way);
                    way = way.previousNode;
                }
                foreach (var item in nodeList)
                {
                    item.Clear();
                }

                route.Reverse();
                return route;
            }

            path = FindNeighbor(way , nodes);
            
            foreach (var item in path)
            {
                if (closeList.Contains(item))
                {
                    continue;
                }

                item.heuristic = Heuristic(item.GetPositionSetY() , goal.GetPositionSetY() );
                item.weight = way.weight + item.enterCost;
                item.previousNode = way;
                Instacne.priorityQue.Enque(item);
            }
        }


        Instacne.priorityQue.Clear();
        return null;
    }

    public List<Node> FindNeighbor(Node node , List<Node> nodeList)
    {
        List<Node> answer = new List<Node>();


        foreach (var nodeIter in nodeList)
        {
            if (node == nodeIter)
                continue;

            if (node.IsNeighbor(nodeIter))
            {
                answer.Add(nodeIter);
            }
            else if (node.IsSqureNeighbor(nodeIter))
            {

                nodeIter.enterCost = squreMoveWeight;
                answer.Add(nodeIter);
            }

        }

        return answer;
    }


    public List<Node> FindSqureNeighbor(Node node, List<Node> nodeList)
    {
        List<Node> answer = new List<Node>();


        foreach (var nodeIter in nodeList)
        {
            if (node.IsSqureNeighbor(nodeIter))
                answer.Add(nodeIter);
        }


        return answer;
    }


}