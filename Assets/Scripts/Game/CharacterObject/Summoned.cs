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
        PlayerGround = 1 << 11,
        PlayerFlight = 1 << 12,

        Enemy = 1 << 20,
        EnemyGround = 1 << 21,
        EnemyFlight = 1 << 22,
    }


    public abstract class Summoned : MonoBehaviour
    {
        [SerializeField]
        public GamePlayManager gamePlayManager;
        public Animator summonAnimator;
        public int ownerSessionId;

        public Node currentNode;
        public Node targetNode;

        public abstract void InitSummon();
    }

}