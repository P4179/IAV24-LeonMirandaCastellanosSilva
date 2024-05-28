using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

namespace IAV24.Final
{
    public class Performer : MonoBehaviour
    {
        private BaseInteraction currentInteraction = null;
        // asegurar que cada interaccion solo se ejecuta una vez
        private bool startedPerforming = false;

        private Dictionary<StatType, Stat> statsInfo = new Dictionary<StatType, Stat>();

        [SerializeField] 
        // numero maximo de interacciones entre las que elegir en la lista de
        // interacciones ordenadas por puntuacion
        protected int maxInteractionPickSize = 3;

        private void onInteractionFinished(BaseInteraction interaction)
        {
            interaction.unlockInteraction(this);
            currentInteraction = null;
            Debug.Log("Interaccion " + interaction.displayName + " terminada");
        }

        private void onInteractionStopped(BaseInteraction interaction)
        {
            interaction.unlockInteraction(this);
            currentInteraction = null;
            Debug.Log("Interaccion " + interaction.displayName + " parada abruptamente");
        }

        // Start is called before the first frame update
        void Start()
        {
            // te devuelve los del propio objeto y los de los hijos
            Stat[] stats = GetComponentsInChildren<Stat>();
            foreach (Stat stat in stats)
            {
                //Debug.Log("Estadistica: " + stat.displayName);
                statsInfo[stat.type] = stat;
            }
        }

        // esta accion se ejecutar UNA SOLA VEZ cuando se haya llegado
        // al smart object en el que realizar la accion
        public void executeCurrentInteractionOnce()
        {
            if (currentInteraction != null && !startedPerforming)
            {
                startedPerforming = true;
                Debug.Log("Interaccion " + currentInteraction.displayName + " comenzada");
                currentInteraction.perform(this, onInteractionFinished, onInteractionStopped);
            }
        }

        // se va a usar por si el personaje esta yendo a un smart object, por lo tanto,
        // esta atado a este, pero justo durante el camino se encuentra con un
        // enemigo y decide cambiar de rumbo y dejar de ir a ese smart object
        public bool abortCurrentInteraction()
        {
            if (currentInteraction != null)
            {
                currentInteraction.unlockInteraction(this);
                currentInteraction = null;
                return true;    // abortada con exito
            }
            return false;   // abortada sin exito (porque no habia interaccion actual)
        }

        // determinar si la interaccion actual se ha dejado de hacer ya puede ser
        // porque se ha terminado, se ha parado o se ha abortado
        // se puede usar para determinar cuando ha terminado una interaccion y pasar a la siguiente
        public bool finishedCurrentInteraction()
        {
            return currentInteraction == null;
        }

        // actualizar las estadisticas del usuario que consigue al realizar
        // una interaccion
        public void updateIndividualStat(StatType targetStat, float amount)
        {
            statsInfo[targetStat].updateIndividualStat(amount);
        }

        float scoreInteractionStat(StatType targetStat, float amount)
        {
            // se obtiene el valor actual de ese stat del usuario
            float currentValue = statsInfo[targetStat].getCurrentValue01();
            // usando esta formula se consigue que si el valor del stat del usuario
            // es muy peque, el score sea mayor y si es muy grande, sea menor
            // tambien funciona para numero negativos
            float aux = statsInfo[targetStat].getChange01(amount);
            return (1.0f - currentValue) * aux;
        }

        // puntuar una interaccion
        float scoreInteraction(BaseInteraction interaction)
        {
            // si no tiene ninguna estadistica, su puntuacion es la por defecto
            if (interaction.changedStats.Length <= 0)
            {
                return interaction.noStatsInteractionScore;
            }

            float score = 0.0f;
            // para calcular la puntuaccion de la interaccion se tiene en cuenta
            // todos los stats que puede aportar
            foreach(ChangedStat stat in interaction.changedStats)
            {
                // targetStat es el stat que cambia y value el valor de cambio
                score += scoreInteractionStat(stat.targetStat, stat.value);
            }

            return score;
        }

        // dar valores a las interacciones y ver cual es mas conveniente
        // en base a que stat es mas bajo
        class ScoredInteraction
        {
            public SmartObject targetObject;
            public BaseInteraction interaction;
            public float score;
        }

        // te devuelve el smartobject al que ir y el tiempo que dura la interaccion
        Transform PickBestInteraction()
        {
            List<ScoredInteraction> unsortedInteractions = new List<ScoredInteraction>();
            // se recorren todos los smartobjects
            foreach (SmartObject smartObject in SmartObjectManager.Instance.registeredObjects)
            {
                // se recorren todas las interacciones
                foreach (BaseInteraction interaction in smartObject.interactions)
                {
                    if (interaction.canPerform())
                    {
                        // se calcula la puntuacion de esa interaccion
                        ScoredInteraction scoredInteraction = new ScoredInteraction();
                        scoredInteraction.targetObject = smartObject;
                        scoredInteraction.interaction = interaction;
                        scoredInteraction.score = scoreInteraction(interaction);

                        // se guarda en la lista
                        unsortedInteractions.Add(scoredInteraction);
                    }
                }
            }

            if (unsortedInteractions.Count <= 0)
            {
                return null;
            }

            // se ordena la lista por orden de puntuacion de mayor a menor
            List<ScoredInteraction> sortedInteractions = unsortedInteractions.OrderByDescending(scoredInteraction => scoredInteraction.score).ToList();

            // se selecciona una interaccion
            int maxIndex = Mathf.Min(maxInteractionPickSize, sortedInteractions.Count());
            int selectedIndex = Random.Range(0, maxIndex);

            SmartObject selectedObject = sortedInteractions[selectedIndex].targetObject;
            BaseInteraction selectedInteraction = sortedInteractions[selectedIndex].interaction;

            // se usa por precacuion
            abortCurrentInteraction();
            currentInteraction = selectedInteraction;
            currentInteraction.lockInteraction(this);
            startedPerforming = false;

            return selectedObject.interPointTransform;
        }

        private void Update()
        {
            if (!startedPerforming)
            {
                PickBestInteraction();
                executeCurrentInteractionOnce();
            }
        }
    }
}
