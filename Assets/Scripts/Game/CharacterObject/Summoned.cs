using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hojun
{

    public class SummonInfo
    {
        public string name;
        public float hp;

    }

    [System.Flags]
    public enum SummonLayer
    {
        
        Player = 1 << 10,
        PlayerFlight = 1 << 11,
        Enemy = 1 << 20,
        EnemyFlight = 1 << 21
    }


    public abstract class Summoned : MonoBehaviour
    {
        [SerializeField]
        public GamePlayManager gamePlayManager;
        public Animator summonAnimator;


        public Node currentNode;
        public Node targetNode;

        public abstract void InitSummon();

    }

}