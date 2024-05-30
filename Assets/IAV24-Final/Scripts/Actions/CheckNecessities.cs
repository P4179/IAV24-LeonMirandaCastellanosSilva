using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace IAV24.Final
{
    public class CheckNecessities : BehaviorDesigner.Runtime.Tasks.Conditional
    {
        private Performer performer;

        public override void OnAwake()
        {
            base.OnAwake();
            performer = GetComponent<Performer>();
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override BehaviorDesigner.Runtime.Tasks.TaskStatus OnUpdate()
        {
            
            bool fill = performer.checkNecessities();
            if (fill) return BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;
            else return BehaviorDesigner.Runtime.Tasks.TaskStatus.Failure;
        }

    }
}
