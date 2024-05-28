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
            public Performer performer; // quien lo ha realizado
            public float elapsedTime;   // cuanto tiempo ha pasado realizando la interaccion
            public UnityAction<BaseInteraction> onCompleted;
        }

        protected List<PerformerInfo> currentPerformes = new List<PerformerInfo>();
        protected int numCurrentUsers = 0;
        [SerializeField]
        // maximo numero de personas que pueden realizar a la vez la interaccion
        protected int maxSimultaneousUsers = 1;

        public override bool canPerform()
        {
            return numCurrentUsers < maxSimultaneousUsers;
        }

        public override void lockInteraction()
        {
            ++numCurrentUsers;

            // solo es para depurar
            if(numCurrentUsers > maxSimultaneousUsers)
            {
                Debug.LogError("Demasiados usuarios en " + displayName);
            }
        }

        public override void perform(Performer performer, UnityAction<BaseInteraction> onCompleted)
        {
            // solo es para depurar
            if(numCurrentUsers <= 0)
            {
                Debug.Log("Tratando de realizar la interaccion " + displayName + " cuando no hay usuarios");
            }

            switch (interactionType)
            {
                case InteractionType.Instantaneous:
                    // la proporcion va dese 0-1
                    // como se realiza de inmediato, la proporcion es la maxima
                    applyStats(performer, 1.0f);
                    onCompleted.Invoke(this);
                    break;

                case InteractionType.OverTime:
                    PerformerInfo performerInfo = new PerformerInfo();
                    performerInfo.performer = performer;
                    performerInfo.elapsedTime = 0.0f;
                    performerInfo.onCompleted = onCompleted;
                    currentPerformes.Add(performerInfo);
                    break;
            }
        }

        public override void unlockInteraction()
        {
            if(numCurrentUsers <= 0)
            {
                Debug.LogError("Tratando de desbloquea la interaccion " + displayName + " en la que no hay usuarios");
            }

            --numCurrentUsers;
        }

        // para las acciones que se realizan en el tiempo
        private void Update()
        {
            // se recorre en sentido inverso para poder borrar
            for(int i = currentPerformes.Count - 1; i >= 0; --i)
            {
                PerformerInfo performerInfo = currentPerformes[i];

                // para que la interaccion se realice en el tiempo se calcula la proporcion que se tiene que
                // hacer en cada frame
                float previousElapsedTime = performerInfo.elapsedTime;
                // ademas, para que no se sume tiempo de mas (milisegundos) se hace que el elapsedTime sea como maximo
                // la duracion.
                // Por ejemplo, si la duracion es 2.0 y el elapsedTime en el frame anterior (previousElapsedTime)
                // era 1.9 y este frame ha tardado en ejecutarse 0.2 (elapsedTime es 2.1), la diferencia no va a ser correcta
                // y se va a sumar de mas porque la duracion es como maximo 2.0. La diferencia real tendria que ser 0.1
                performerInfo.elapsedTime = Mathf.Min(performerInfo.elapsedTime + Time.deltaTime, duration);

                applyStats(performerInfo.performer, (performerInfo.elapsedTime - previousElapsedTime) / duration);

                if(performerInfo.elapsedTime >= duration)
                {
                    performerInfo.onCompleted.Invoke(this);
                    currentPerformes.RemoveAt(i);
                }
            }
        }
    }
}
