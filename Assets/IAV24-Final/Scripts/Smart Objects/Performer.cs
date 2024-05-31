using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Timeline;

namespace IAV24.Final
{
    public class Performer : MonoBehaviour
    {
        public class CurrentInteraction
        {
            public GameObject target = null;
            public BaseInteraction interaction = null;

            public void reset()
            {
                target = null;
                interaction = null;
            }

        }
        CurrentInteraction currInteraction = new CurrentInteraction();

        // asegurar que cada interaccion solo se ejecuta una vez
        private bool startedPerforming = false;

        private List<MemoryFragment> permanentMemory = new List<MemoryFragment>();
        private List<MemoryFragment> shortTermMemory = new List<MemoryFragment>();

        private Dictionary<StatType, Stat> statsInfo = new Dictionary<StatType, Stat>();

        [SerializeField]
        private int permanentMemoryThreshold = 2;

        [SerializeField]
        private float startFilling = 0.7f;

        [SerializeField]
        // numero maximo de interacciones entre las que elegir en la lista de
        // interacciones ordenadas por puntuacion
        protected int maxInteractionPickSize = 3;

        private void onInteractionFinished(BaseInteraction interaction)
        {
            interaction.unlockInteraction(this);
            currInteraction.reset();
            LevelManager.Instance.interactionInfoText = interaction.displayName + " ended";
            Debug.Log(interaction.displayName + " terminada");
        }

        private void onInteractionStopped(BaseInteraction interaction)
        {
            interaction.unlockInteraction(this);
            currInteraction.reset();
            LevelManager.Instance.interactionInfoText = interaction.displayName + " aborted";
            Debug.Log(interaction.displayName + " parada abruptamente");
        }

        // Start is called before the first frame update
        void Start()
        {
            // te devuelve los del propio objeto y los de los hijos
            Stat[] stats = GetComponentsInChildren<Stat>();
            foreach (Stat stat in stats)
            {
                statsInfo[stat.type] = stat;
            }
        }

        private void Update()
        {
            // actualizar memoria de corto plazo
            for (int index = shortTermMemory.Count - 1; index >= 0; index--)
            {
                if (shortTermMemory[index].updateTime())
                {
                    shortTermMemory.RemoveAt(index);
                }
            }
        }

        // esta accion se ejecutar UNA SOLA VEZ cuando se haya llegado
        // al smart object en el que realizar la accion
        public void executeCurrentInteractionOnce()
        {
            if (currInteraction.interaction != null && !startedPerforming)
            {
                startedPerforming = true;
                LevelManager.Instance.interactionInfoText = currInteraction.interaction.displayName + " started";
                Debug.Log(currInteraction.interaction.displayName + " comenzada");
                currInteraction.interaction.perform(this, onInteractionFinished, onInteractionStopped);
            }
        }

        // se va a usar por si el personaje esta yendo a un smart object, por lo tanto,
        // esta atado a este, pero justo durante el camino se encuentra con un
        // enemigo y decide cambiar de rumbo y dejar de ir a ese smart object
        public bool abortCurrentInteraction()
        {
            if (currInteraction.interaction != null)
            {
                currInteraction.interaction.unlockInteraction(this);
                currInteraction.reset();
                return true;
            }
            return false;
        }

        // determinar si la interaccion actual se ha dejado de hacer ya puede ser
        // porque se ha terminado, se ha parado o se ha abortado
        // se puede usar para determinar cuando ha terminado una interaccion y pasar a la siguiente
        public bool finishedCurrentInteraction()
        {
            return currInteraction.interaction == null;
        }

        // actualizar las estadisticas del usuario que consigue al realizar
        // una interaccion
        public void updateIndividualStat(StatType targetStat, float amount)
        {
            statsInfo[targetStat].updateIndividualStat(amount);
        }

        // amount es la cantidad de estadistica que la interaccion aporta
        float scoreInteractionStat(StatType targetStat, float amount)
        {
            // se obtiene el valor actual de ese stat del usuario
            float currentValue = statsInfo[targetStat].getCurrentValue01();

            memoryModifiesScore(currentValue, targetStat, shortTermMemory);
            memoryModifiesScore(currentValue, targetStat, permanentMemory);

            // usando esta formula se consigue que si el valor del stat del usuario
            // es muy peque, el score sea mayor y si es muy grande, sea menor
            // tambien funciona para numero negativos
            float aux = statsInfo[targetStat].normalizeValue(amount);
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
            foreach (ChangedStat stat in interaction.changedStats)
            {
                // targetStat es el stat que cambia y value el valor de cambio
                score += scoreInteractionStat(stat.targetStat, stat.value);
            }
            return score;
        }

        float memoryModifiesScore(float currentValue, StatType targetStat, List<MemoryFragment> memories)
        {
            foreach (MemoryFragment memory in memories)
            {
                foreach (ChangedStat statMultiplier in memory.changedStats)
                {
                    if (statMultiplier.targetStat == targetStat)
                    {
                        currentValue = Mathf.Clamp01(currentValue + statMultiplier.value);
                    }
                }
            }
            return currentValue;
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
        public void pickBestInteraction()
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

            if (unsortedInteractions.Count > 0)
            {
                // se ordena la lista por orden de puntuacion de mayor a menor
                List<ScoredInteraction> sortedInteractions = unsortedInteractions.OrderByDescending(scoredInteraction => scoredInteraction.score).ToList();

                // se selecciona una interaccion
                int maxIndex = Mathf.Min(maxInteractionPickSize, sortedInteractions.Count());
                int selectedIndex = UnityEngine.Random.Range(0, maxIndex);

                SmartObject selectedObject = sortedInteractions[selectedIndex].targetObject;
                BaseInteraction selectedInteraction = sortedInteractions[selectedIndex].interaction;

                // se usa por precacuion
                abortCurrentInteraction();
                currInteraction.interaction = selectedInteraction;
                currInteraction.interaction.lockInteraction(this);
                currInteraction.target = selectedObject.gameObject;
                startedPerforming = false;
            }
        }

        public GameObject getTargetObject()
        {
            return currInteraction.target;
        }

        public bool checkNecessities()
        {
            foreach (var statInfo in statsInfo)
            {
                Stat stat = statInfo.Value;
                if (!stat.enabled) return true;
                else
                {
                    if (stat.getCurrentValue01() < startFilling) return true;
                }
            }

            return false;
        }

        public void addMemories(MemoryFragment[] newMemories)
        {
            foreach (MemoryFragment memory in newMemories)
            {
                addMemory(memory);
            }
        }


        protected void addMemory(MemoryFragment newMemory)
        {
            MemoryFragment cancelledMemory = null;

            // MEMORIA PERMANENTE
            bool duplicatedPermanentMemory = false;
            foreach (MemoryFragment memory in permanentMemory)
            {
                // si se encuentra en la memoria permanente
                // se descarta por completo
                if (newMemory.isTheSameAs(memory))
                {
                    duplicatedPermanentMemory = true;
                }
                // cancela a una fragmento ya existente
                if (memory.isCancelledBy(newMemory))
                {
                    cancelledMemory = memory;
                }
            }
            // se hace despues para que no se rompa el iterador
            if (cancelledMemory != null)
            {
                permanentMemory.Remove(cancelledMemory);
            }

            if (!duplicatedPermanentMemory)
            {
                // MEMORIA DE CORTO PLAZO
                cancelledMemory = null;
                MemoryFragment existingMemory = null;
                foreach (var memory in shortTermMemory)
                {
                    // se guarda porque se va a aumentar el numero de ocurrencias
                    if (newMemory.isTheSameAs(memory))
                    {
                        existingMemory = memory;
                    }
                    // cancela a un fragmento ya existente
                    if (memory.isCancelledBy(newMemory))
                    {
                        cancelledMemory = memory;
                    }
                }
                // eliminar el fragmento cancelado
                if (cancelledMemory != null)
                {
                    shortTermMemory.Remove(cancelledMemory);
                }
                // insertar el nuevo fragmento
                if (existingMemory == null)
                {
                    shortTermMemory.Add(newMemory.duplicate());
                }
                // aumentar el numero de ocurrencias en el caso de que ya existiera
                else
                {
                    existingMemory.newOcurrence(newMemory);
                    // se supera el umbral, por lo tanto, se convierte
                    // de un fragmento reciente a uno permanente
                    if (existingMemory.ocurrences >= permanentMemoryThreshold)
                    {
                        permanentMemory.Add(existingMemory);
                        shortTermMemory.Remove(existingMemory);
                    }
                }
            }
        }
    }
}
