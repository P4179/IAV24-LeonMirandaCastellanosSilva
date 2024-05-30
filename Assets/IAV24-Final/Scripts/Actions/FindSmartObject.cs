using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace IAV24.Final
{
    public class FindSmartObject : BehaviorDesigner.Runtime.Tasks.Action
    {
        [Tooltip("The object variable that will be set when a object is found what the object is")]
        public SharedGameObject m_ReturnedObject;

        private Performer performer;
        public override void OnAwake()
        {
            base.OnAwake();
            performer = GetComponent<Performer>();
        }

        public override void OnStart()
        {
            // Como se reevalua en cada tick, se comprueba si se
            // ha terminado la accion. Si se ha terminado, elige otra
            if (performer.finishedCurrentInteraction()) performer.pickBestInteraction();
            m_ReturnedObject.Value = performer.getTargetObject();
            //Debug.Log(m_ReturnedObject.Value);
        }

        public override BehaviorDesigner.Runtime.Tasks.TaskStatus OnUpdate()
        {
            // Si no hay ningun objetivo, devuelve fracaso, y si no, devuelve exito
            if (m_ReturnedObject.Value == null) return BehaviorDesigner.Runtime.Tasks.TaskStatus.Failure;
            else return BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;
        }

    }
}
