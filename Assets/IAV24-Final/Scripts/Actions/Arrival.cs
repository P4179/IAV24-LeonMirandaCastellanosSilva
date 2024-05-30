using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace IAV24.Final
{
    public class Arrival : BehaviorDesigner.Runtime.Tasks.Action
    {
        [UnityEngine.Serialization.FormerlySerializedAs("target")]
        public SharedGameObject m_Target;

        [UnityEngine.Serialization.FormerlySerializedAs("slow distance")]
        public SharedFloat slowDist;

        [UnityEngine.Serialization.FormerlySerializedAs("stop distance")]
        public SharedFloat stopDist;

        private Transform tr = null;
        private NavMeshAgent navMeshAg = null;
        private Vector3 lastVel;

        public override void OnAwake()
        {
            base.OnAwake();
            tr = GetComponent<Transform>();
            navMeshAg = GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
            
            lastVel = navMeshAg.velocity;
        }

        public override BehaviorDesigner.Runtime.Tasks.TaskStatus OnUpdate()
        {
            float dist = Vector3.Distance(m_Target.Value.transform.position, tr.position);
            // Si esta dentro del area de ralentizado
            if (dist <= slowDist.Value)
            {
                // Si ya esta dentro, la llegada ha completado, por lo que se
                // devuelve exito en el nodo y se detiene la navegacion
                if (dist <= stopDist.Value) 
                {
                    navMeshAg.ResetPath();
                    //navMeshAg.velocity = new Vector3(0, 0, 0);
                    return BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;
                }
                // Si no, se va cambiando la velocidad del agente segun la velocidad
                // que tenia al entrar el objetivo en el rango de ralentizar. Cuanto
                // mas cerca este del objetivo, mas lento se movera
                else
                {
                    float vel = dist / slowDist.Value;
                    //Debug.Log(lastVel * vel);
                    navMeshAg.velocity = lastVel * vel;
                    navMeshAg.velocity.Normalize();
                    return BehaviorDesigner.Runtime.Tasks.TaskStatus.Running;
                }
            }
            lastVel = navMeshAg.velocity;
            //Debug.Log(lastVel);

            // Si no, la tarea "ha fallado", ya que el objetivo no esta
            // a la distancia suficiente como para frenarse o pararse
            return BehaviorDesigner.Runtime.Tasks.TaskStatus.Failure;
        }


        // Draw the seeing radius
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Owner == null) return;
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, slowDist.Value);
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}
