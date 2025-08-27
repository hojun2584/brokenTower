using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeRPC : MonoBehaviour
{

    public static InvokeRPC instance;

    Dictionary<string, Action> rpcDict;

    public bool AddFunc(string name , Action function) 
    {
        if (rpcDict.ContainsKey(name))
            return false;
        
        rpcDict.Add(name, function);
        return true;
    }

    public bool InvokeRpc(string name)
    {
        if (!rpcDict.ContainsKey(name))
            return false;

        rpcDict[name]();
        return true;
    }


    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
            return;
        }


        rpcDict = new Dictionary<string, Action>();
    }






}
