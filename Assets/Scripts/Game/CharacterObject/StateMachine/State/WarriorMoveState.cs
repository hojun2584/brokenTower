using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hojun;
using System;

namespace Hojun
{

    public class WarriorMoveState : State
    {
        Warrior ownerWarrior;

        public WarriorMoveState(IStateMachine sm) : base(sm)
        {
            ownerWarrior = (Warrior)sm.GetOwner();
        }

        public override void Enter()
        {
            base.Enter();

            
            ownerWarrior.GetAnimator.SetInteger("State", 1);
        }
        public override void Update() 
        {
            base.Update();

            if (ownerWarrior.IsAttackAble())
            {
                ownerWarrior.CustomStateMachine.SetState((int)WarriorState.ATTACK);
            }

        }

        public override void Exit() 
        {
            base.Exit();
        }
     

    }
}