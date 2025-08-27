using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Hojun;

namespace Hojun
{

    public abstract class State
    {
        protected IStateMachine stateMachine = null;
        protected Component owner;
        private object ownerObj;

        public event Action enterAction;
        public event Action exitAction;
        public event Action updateAction;

        public State(IStateMachine sm)
        {
            ownerObj = sm.GetOwner();
            owner = ownerObj as Warrior;
        }

        public virtual void Init(IStateMachine sm)
        {
            this.stateMachine = sm;

        }

        public virtual void Enter()
        {
            enterAction?.Invoke();
        }
        public virtual void Update()
        {
            updateAction?.Invoke();
        }
        public virtual void Exit()
        {
            exitAction?.Invoke();
        }
    }



}


