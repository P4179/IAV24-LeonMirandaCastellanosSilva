using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace IAV24.Final
{
    public class AbortSmartObject : BehaviorDesigner.Runtime.Tasks.Action
    {
        private Performer performer;

        private bool done;

        public override void OnAwake()
        {
            base.OnAwake();
            performer = GetComponent<Performer>();
        }

        public override void OnStart()
        {
            // abortar la interaccion actual en el caso de que haya una
            done = performer.abortCurrentInteraction();
        }

        public override BehaviorDesigner.Runtime.Tasks.TaskStatus OnUpdate()
        {
            if (done) return BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;
            else return BehaviorDesigner.Runtime.Tasks.TaskStatus.Failure;
        }
    }
}
