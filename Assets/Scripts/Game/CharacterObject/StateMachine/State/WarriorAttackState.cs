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
