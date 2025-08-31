using Hojun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WarriorAttackState : State
{

    Warrior ownerWarrior;

    public WarriorAttackState(IStateMachine sm) : base(sm)
    {
        ownerWarrior = (Warrior)sm.GetOwner();
    }

    public override void Enter()
    {
        base.Enter();
        ownerWarrior.GetAnimator.SetInteger("State", 2);
        ownerWarrior.Attack( ownerWarrior.target.GetComponent<IHitAble>() );
    }
    public override void Update()
    {
        base.Update();
        if (!ownerWarrior.IsAttackAble())
            ownerWarrior.CustomStateMachine.SetState((int)WarriorState.MOVE);

        if (ownerWarrior.IsDead)
            ownerWarrior.CustomStateMachine.SetState((int)WarriorState.DEAD);
    }

    public override void Exit()
    {
        base.Exit();
    }



}
