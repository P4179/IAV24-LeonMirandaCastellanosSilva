using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace IAV24.Final
{
    public class TimedWander : BehaviorDesigner.Runtime.Tasks.Movement.Wander
    {
        [UnityEngine.Serialization.FormerlySerializedAs("wander time")]
        public SharedFloat wanderTime;

        private float timer = 0.0f;
        public override void OnStart()
        {
            base.OnStart();
            timer = 0.0f;
        }

        // Si se pasa del tiempo de merodeo, devuelve exito, y si no, ejecuta el merodeo
        public override BehaviorDesigner.Runtime.Tasks.TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer > wanderTime.Value) return BehaviorDesigner.Runtime.Tasks.TaskStatus.Success;

            return base.OnUpdate();
        }


    }
}