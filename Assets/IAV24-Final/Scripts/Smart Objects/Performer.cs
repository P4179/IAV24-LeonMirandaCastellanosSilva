using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        // si no se ha podido calcular la puntuacion de una interaccion, se usa esta
        protected float defaultInteractionScore = 0f;
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
                currentInteraction.perform(this, onInteractionFinished, onInteractionStopped);
            }
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
            if (interaction.appliedStats.Length <= 0)
            {
                return defaultInteractionScore;
            }

            float score = 0.0f;
            // para calcular la puntuaccion de la interaccion se tiene en cuenta
            // todos los stats que puede aportar
            foreach(AppliedStat stat in interaction.appliedStats)
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
        Transform PickBestInteraction(out float duration)
        {
            duration = 0.0f;

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

            currentInteraction = selectedInteraction;
            currentInteraction.lockInteraction(this);
            startedPerforming = false;

            duration = selectedInteraction.duration;
            return selectedObject.interPointTransform;
        }

        private void Update()
        {
            if (!startedPerforming)
            {
                float duration = 0.0f;
                PickBestInteraction(out duration);
                executeCurrentInteractionOnce();
            }
        }
    }
}
