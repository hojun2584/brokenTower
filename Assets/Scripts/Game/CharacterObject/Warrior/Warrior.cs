using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hojun;
using System;
using CustomClient;

namespace Hojun
{

    #region 워리어 클래스 스테이터스 정보
    public class WarriorInfo : SummonInfo
    {
        
        public float attackArea;
        public float atkPoint;
        public int attackAbleLayer;
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
        DEAD = 3,

    }
    #endregion



    public class Warrior : Summoned, IAttackAble, IHitAble, IDeadAble, IMoveAble
    {
        public List<Node> moveRoutes = new List<Node>();
        public WarriorInfo warriorStatus;

        public bool targetFind = false;

        public float hp;

        public Tower enemyTower;

        public GameObject target;


        public bool IsDead { get => warriorStatus.hp <= 0; }

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

        Coroutine moveCorutine;

        public void Update()
        {
            CustomStateMachine.Update();
            hp = warriorStatus.hp;
        }


        public void Start()
        {
            transform.position = currentNode.GetPositionSetY();
            CustomStateMachine.SetState((int)WarriorState.MOVE);
            
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
                    return true;
                }
            }
            targetFind = false;
            return false;
        }


        public void Attack(IHitAble hitObject)
        {
            StartCoroutine( AttackDelay(hitObject) );
        }

        private IEnumerator AttackDelay(IHitAble hitObject)
        {
            yield return new WaitForSeconds(0.4f);
            while (IsAttackAble())
            {
                hitObject.Hit(warriorStatus.atkPoint);
                yield return new WaitForSeconds(0.8f);
            }
        }

        public void Dead()
        {
            StartCoroutine(DeadTimer());
        }

        IEnumerator DeadTimer() 
        {
            yield return new WaitForSeconds(3.0f);
            gameObject.GetComponent<Collider>().enabled = false;
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
            moveCorutine = StartCoroutine(MoveCube(this.gameObject , moveRoutes));
        }

        IEnumerator MoveCube(GameObject cube, List<Node> route)
        {
            foreach (var item in route)
            {
                yield return MovePosition(cube, item, route);

                
            }
        }

        IEnumerator MovePosition(GameObject operand, Node targetPosition, List<Node> route)
        {
            while (Vector3.Distance(operand.transform.position, targetPosition.GetPositionSetY()) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition.GetPositionSetY(), 1f * Time.deltaTime);
                transform.LookAt(targetPosition.GetPositionSetY());
                yield return new WaitUntil(() => !targetFind);
            }
            operand.transform.position = targetPosition.GetPositionSetY();
            currentNode = targetPosition;
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

            // 레이어 셋팅 하는데 지금 서버에서 내꺼이냐 아니냐 가르쳐 주고 있음
            // 이거 바탕으로 생성 될 때 내꺼 레이어 설정하고 
            // 상대꺼 레이어는 무조건 enemy로 설정하면 공격하기 쉬워질 듯?
            // 이거 마우스 반대 버튼 누르면 initSummonEnemy 호출 하는 방식으로 함수 하나 더 
            
            if(ownerSessionId == NetworkManager.instance.session.SessionId)
                AllianceSummonInit();
            else
                EnemySummonInit();


            WarriorStateMachineInit();

        }


        public void WarriorStateMachineInit()
        {
            CustomStateMachine.AddState((int)WarriorState.MOVE, new WarriorMoveState(CustomStateMachine));
            CustomStateMachine.AddState((int)WarriorState.ATTACK, new WarriorAttackState(CustomStateMachine));
            CustomStateMachine.AddState((int)WarriorState.DEAD, new WarriorDeadState(CustomStateMachine));

            CustomStateMachine.stateDict[(int)WarriorState.MOVE].enterAction += Move;
            CustomStateMachine.stateDict[(int)WarriorState.MOVE].exitAction += () => StopCoroutine(moveCorutine);


        }

        public void AllianceSummonInit()
        {
            Debug.Log("내 캐릭터 소환");
            warriorStatus = new WarriorInfo.Builder().SetAtkPoint(3f).SetSpeed(1.0f).SetName("Warrior").SetHp(10).SetAttackArea(3.0f).SetAttackLayer((int)SummonLayerMask.EnemyGround | (int)SummonLayerMask.Enemy).Build();
            gameObject.layer = (int)SummonLayer.PlayerGround;
            targetNode = gamePlayManager.towers[0].currentNode;
        }

        public void EnemySummonInit()
        {
            Debug.Log("적 캐릭터 소환");
            warriorStatus = new WarriorInfo.Builder().SetAtkPoint(3f).SetSpeed(1.0f).SetName("EnemyWarrior").SetHp(10).SetAttackArea(3.0f).SetAttackLayer((int)SummonLayerMask.PlayerGround | (int)SummonLayerMask.Player).Build();
            gameObject.layer = (int)SummonLayer.EnemyGround;
            targetNode = gamePlayManager.towers[1].currentNode;
        }



        public void SetTargetLayer(int layer)
        {
            if(layer == (int)SummonLayerMask.Player)
                warriorStatus.attackAbleLayer = (int)SummonLayerMask.EnemyGround;
            else
                warriorStatus.attackAbleLayer = (int)SummonLayerMask.PlayerGround;

        }
    }

}