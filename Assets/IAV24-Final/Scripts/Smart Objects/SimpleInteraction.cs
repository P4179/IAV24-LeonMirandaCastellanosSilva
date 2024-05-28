using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IAV24.Final
{
    public class SimpleInteraction : BaseInteraction
    {
        // informacion sobre el agente que realiza la interaccion
        protected class PerformerInfo
        {
            public bool isPerforming;   // esta la accion en proceso o ya la ha termindo
            public float elapsedTime;   // cuanto tiempo ha pasado realizando la interaccion
            public UnityAction<BaseInteraction> onCompleted;
            public UnityAction<BaseInteraction> onStopped;
        }

        private List<Performer> performersToCleanup = new List<Performer>();
        protected Dictionary<Performer, PerformerInfo> currentPerformers = new Dictionary<Performer, PerformerInfo>();

        protected int numCurrentUsers => currentPerformers.Count;
        [SerializeField]
        // maximo numero de personas que pueden realizar a la vez la interaccion
        protected int maxSimultaneousUsers = 1;

        public override bool canPerform()
        {
            return numCurrentUsers < maxSimultaneousUsers;
        }

        public override void lockInteraction(Performer performer)
        {
            if (!currentPerformers.ContainsKey(performer))
            {
                PerformerInfo performerInfo = new PerformerInfo();
                performerInfo.isPerforming = false;
                currentPerformers.Add(performer, performerInfo);
            }
        }

        public override void perform(Performer performer, UnityAction<BaseInteraction> onCompleted, UnityAction<BaseInteraction> onStopped)
        {
            if (currentPerformers.ContainsKey(performer))
            {
                switch (interactionType)
                {
                    case InteractionType.Instantaneous:
                        // la proporcion va dese 0-1
                        // como se realiza de inmediato, la proporcion es la maxima
                        applyStats(performer, 1.0f);
                        onCompleted.Invoke(this);
                        break;

                    case InteractionType.OverTime:
                        PerformerInfo performerInfo = currentPerformers[performer];
                        performerInfo.isPerforming = true;
                        performerInfo.elapsedTime = 0.0f;
                        performerInfo.onCompleted = onCompleted;
                        performerInfo.onStopped = onStopped;
                        break;
                }
            }
        }

        public override void unlockInteraction(Performer performer)
        {
            if (currentPerformers.ContainsKey(performer))
            {
                performersToCleanup.Add(performer);
            }
        }

        // para las acciones que se realizan en el tiempo
        private void Update()
        {
            // se recorre en sentido inverso para poder borrar
            foreach (var currentPerformer in currentPerformers)
            {
                Performer performer = currentPerformer.Key;
                PerformerInfo performerInfo = currentPerformer.Value;

                if (performerInfo.isPerforming)
                {
                    if (canStillPerform())
                    {
                        // para que la interaccion se realice en el tiempo se calcula la proporcion que se tiene que
                        // hacer en cada frame
                        float previousElapsedTime = performerInfo.elapsedTime;
                        // ademas, para que no se sume tiempo de mas (milisegundos) se hace que el elapsedTime sea como maximo
                        // la duracion.
                        // Por ejemplo, si la duracion es 2.0 y el elapsedTime en el frame anterior (previousElapsedTime)
                        // era 1.9 y este frame ha tardado en ejecutarse 0.2 (elapsedTime es 2.1), la diferencia no va a ser correcta
                        // y se va a sumar de mas porque la duracion es como maximo 2.0. La diferencia real tendria que ser 0.1
                        performerInfo.elapsedTime = Mathf.Min(performerInfo.elapsedTime + Time.deltaTime, duration);

                        bool isFinalTick = performerInfo.elapsedTime >= duration;
                        if (interactionType == InteractionType.OverTime || (interactionType == InteractionType.AfterTime && isFinalTick))
                        {
                            applyStats(performer, (performerInfo.elapsedTime - previousElapsedTime) / duration);
                        }

                        if (isFinalTick)
                        {
                            performerInfo.onCompleted(this);
                        }
                    }
                    else
                    {
                        performerInfo.onStopped(this);
                    }
                }
            }

            // refresh de los usuarios que han terminado la interaccion
            foreach(Performer performer in performersToCleanup)
            {
                currentPerformers.Remove(performer);
            }
            performersToCleanup.Clear();
        }
    }
}
