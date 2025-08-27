using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public class JobQueue
    {
        static JobQueue instance;
        public Queue<Action> jobActions = new Queue<Action>();
        public static JobQueue Instance 
        {
            get
            {
                if (instance == null)
                    instance = new JobQueue();
                return instance;
            }
        }


        public void Add(Action queAction)
        {
            jobActions.Enqueue(queAction);
        }
    }
}
