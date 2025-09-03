using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hojun;

namespace Hojun
{
    public class WarriorDeadState : State
    {
        Warrior ownerWarrior;

        public WarriorDeadState(IStateMachine sm) : base(sm)
        {
            ownerWarrior = (Warrior)sm.GetOwner();
        }

        public override void Enter()
        {
            base.Enter();
            ownerWarrior.GetAnimator.SetInteger("State", 3);
        }
        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }


    }
}