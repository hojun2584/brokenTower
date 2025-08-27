using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hojun
{

    public interface IStateMachine
    {
        object GetOwner();
        void SetState(int stateName);
    }

    public class StateMachine<T> : IStateMachine where T : class
    {
        public T owner = null;

        public State CurState
        {
            get
            {
                return curState;
            }
        }
        State curState;

        public Dictionary<int, State> stateDict;

        public StateMachine(T owner)
        {
            this.owner = owner;
            stateDict = new Dictionary<int, State>();

        }

        public void AddState(int stateName, State state)
        {
            if (stateDict.ContainsKey(stateName))
                return;

            stateDict.Add(stateName, state);
            state.Init(this);
        }



        public object GetOwner()
        {
            return owner;
        }

        public void SetState(int stateName)
        {
            if (stateDict.ContainsKey(stateName))
            {
                if (curState != null)
                {
                    curState.Exit();
                }

                if (stateDict[stateName] == CurState)
                    return;


                curState = stateDict[stateName];
                curState.Enter();

            }
        }


        public void Update()
        {
            curState?.Update();
        }

    }
}
