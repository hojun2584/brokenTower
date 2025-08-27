using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hojun;
using System;

namespace Hojun
{

    #region 워리어 클래스 스테이터스 정보
    public class WarriorInfo : SummonInfo
    {
        
        public float attackArea;
        public float atkPoint;
        public int attackAbleLayer = (int)SummonLayer.Enemy;
        public float speed = 1.0f;


        public class Builder
        {
            WarriorInfo info = new WarriorInfo();


            public Builder SetAttackArea(float attackArea)
            {
                info.attackArea = attackArea;
                return this;
            }
            
            public Builder SetAtkPoint(float atkPoint)
            {
                info.atkPoint = atkPoint;
                return this;
            }

            public Builder SetAttackLayer(LayerMask attackLayer)
            {
                info.attackAbleLayer = attackLayer;
                return this;
            }
            public Builder SetSpeed(float speed) 
            {
                info.speed = speed;
                return this;
            }

            public Builder SetName(string name)
            {
                info.name = name;
                return this;
            }

            public Builder SetHp(float hp)
            {
                info.hp = hp;
                return this;
            }

            public WarriorInfo Build()
            {
                return info;
            }
        }
    }

    public enum WarriorState
    {
        MOVE = 1,
        ATTACK = 2,


    }
    #endregion



    public class Warrior : Summoned, IAttackAble, IHitAble, IDeadAble, IMoveAble
    {
        public List<Node> moveRoutes = new List<Node>();
        public WarriorInfo warriorStatus;

        public bool targetFind = false;
        
        public Tower towerPositon;

        public LayerMask layer;
        public GameObject target;


        public Animator GetAnimator { 
            get 
            {
                if (summonAnimator == null)
                    summonAnimator = GetComponent<Animator>();

                return summonAnimator;
            }
        }

        StateMachine<Warrior> stateMachine;
        public StateMachine<Warrior> CustomStateMachine 
        {
            get
            {
                if (stateMachine == null)
                {
                    stateMachine = new StateMachine<Warrior>(this);
                }
                return stateMachine;
            }
            set
            {
                stateMachine = value;
            }
        }


        public void Awake()
        {
            InitObject();
            warriorStatus.attackAbleLayer = (int)SummonLayer.Enemy;
        }

        public void Update()
        {

            //Collider[] hitColliders = Physics.OverlapSphere(transform.position, warriorStatus.attackArea, warriorStatus.attackAbleLayer );
            
            //var sortedColliders = hitColliders.OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).ToArray();
            //layer = warriorStatus.attackAbleLayer;
            //foreach (var item in sortedColliders)
            //{
            //    if (item.transform.TryGetComponent<IHitAble>(out IHitAble hitObject))
            //    {
            //        target = item.gameObject;
            //        targetFind = true;

            //        break;
            //    }
            //}

            CustomStateMachine.Update();
        }


        public void Start()
        {
            transform.position = currentNode.GetPositionSetY();
            CustomStateMachine.SetState((int)WarriorState.MOVE);
        }

        public void InitObject()
        {
            Debug.Log("범위 바꿀 것");
            warriorStatus = new WarriorInfo.Builder().SetName("Warrior").SetHp(50).SetAttackArea(3.0f).Build();

            targetNode = towerPositon.currentNode;
            CustomStateMachine.AddState((int)WarriorState.MOVE, new WarriorMoveState(CustomStateMachine));
            CustomStateMachine.stateDict[(int)WarriorState.MOVE].enterAction += Move;
            CustomStateMachine.AddState((int)WarriorState.ATTACK, new WarriorAttackState(CustomStateMachine));
            CustomStateMachine.stateDict[(int)WarriorState.ATTACK].enterAction += () => StopCoroutine("Move");
        }


        public bool IsAttackAble()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, warriorStatus.attackArea, warriorStatus.attackAbleLayer);

            var sortedColliders = hitColliders.OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).ToArray();
            
            foreach (var item in sortedColliders)
            {
                if (item.transform.TryGetComponent<IHitAble>(out IHitAble hitObject))
                {
                    target = item.gameObject;
                    targetFind = true;

                    Debug.Log("targetFind");
                    return true;
                }
            }
            return false;
        }


        public void Attack(IHitAble hitObject)
        {
            if (hitObject != null)
            {
                hitObject.Hit(warriorStatus.atkPoint);
            }
        }

        public void Dead()
        {

            StartCoroutine(DeadTimer());
        }

        IEnumerator DeadTimer() 
        {
            yield return new WaitForSeconds(5.0f);
            Destroy(this.gameObject);
        }

        public void Hit(float hitObject)
        {
            warriorStatus.speed = 0f;

            if (warriorStatus.hp - hitObject <= 0)
            {
                Dead();
            }
            else
            {
                warriorStatus.hp -= hitObject;    
            }
        }


        

        public void Move()
        {
            moveRoutes = AstarAlgorithm.Instacne.FindPath(currentNode, targetNode, AstarAlgorithm.Instacne.nodeList);

            StartCoroutine(moveCube(this.gameObject , moveRoutes));
        }

        IEnumerator moveCube(GameObject cube, List<Node> route)
        {
            foreach (var item in route)
            {
                yield return movePosition(cube, item);
            }
        }

        IEnumerator movePosition(GameObject operand, Node targetPosition)
        {

            while (Vector3.Distance(operand.transform.position, targetPosition.GetPositionSetY()) > 0.1f)
            {
                
                transform.position = Vector3.MoveTowards(transform.position, targetPosition.GetPositionSetY(), warriorStatus.speed * Time.deltaTime);
                transform.LookAt(targetPosition.GetPositionSetY());

                yield return new WaitWhile(() => targetFind);
            }

            operand.transform.position = targetPosition.GetPositionSetY();
        }


        void OnDrawGizmos()
        {
            // 기즈모 색상 설정
            Gizmos.color = Color.red;

            if(warriorStatus != null)
                Gizmos.DrawWireSphere(transform.position, warriorStatus.attackArea);
        }

        public override void InitSummon()
        {
            
        }
    }

}