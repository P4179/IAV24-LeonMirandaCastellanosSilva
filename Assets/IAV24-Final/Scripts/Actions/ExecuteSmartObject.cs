using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace IAV24.Final
{
    public class ExecuteSmartObject : BehaviorDesigner.Runtime.Tasks.Action
    {
        private Performer performer;

        public override void OnAwake()
        {
            base.OnAwake();
            performer = GetComponent<Performer>();
        }

        public override void OnStart()
        {
            performer.executeCurrentInteractionOnce();
        }

        public override BehaviorDesigner.Runtime.Tasks.TaskStatus OnUpdate()
        {
            // Si se ha completado la accion, devuelve exito y si no, devuelve que se esta ejecutando el nodo
            if (performer.finishedCurrentInteraction()) return BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;
            else return BehaviorDesigner.Runtime.Tasks.TaskStatus.Running;
        }

        public override void OnConditionalAbort()
        {
            base.OnConditionalAbort();
            performer.abortCurrentInteraction();
        }
    }
}
