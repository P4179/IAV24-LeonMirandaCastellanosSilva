using BehaviorDesigner.Runtime.Tasks.Unity.Math;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace IAV24.Final
{
    public enum InteractionType
    {
        Instantaneous = 0,   // se realiza de inmediato
        OverTime = 1,       // tarda un tiempo
        AfterTime = 2,      // se realiza despues de un tiempo
    }

    [System.Serializable]
    public class ChangedStat
    {
        public StatType targetStat; // estadistica a la que va a afectar
        public float value;     // cantidad que aumenta la estadistica
    }

    [System.Serializable]
    public class Outcome
    {
        public string displayName;  // nombre
        [Range(0f, 1f)]
        public float probability;   // probabilidad de que salga
        public bool stopInteraction = false;    // si sale para la accion
        public ChangedStat[] statsMultipliers;  // multiplicadores que aplicar a las diferentes necesidades
        public float normalisedProbability { get; set; }
        public Dictionary<StatType, List<float>> statsMultipliersAux = new Dictionary<StatType, List<float>>();

        public void init()
        {
            foreach(ChangedStat stat in statsMultipliers)
            {
                StatType changedStatType = stat.targetStat;
                if (!statsMultipliersAux.ContainsKey(changedStatType))
                {
                    statsMultipliersAux[changedStatType] = new List<float>();
                }
                statsMultipliersAux[changedStatType].Add(stat.value);
            }
        }
    }

    // Clase abstracta sobre las interacciones de un smart object
    // Tiene atributos y metodos para luego definir cuanto se tarda
    // en realizar la interaccion, que es lo que se hace...
    public abstract class BaseInteraction : MonoBehaviour
    {
        protected SmartObject smartObject;

        [SerializeField]
        [Range(0f, 1f)]
        protected float _noStatsInteractionScore = 0f;
        public float noStatsInteractionScore => _noStatsInteractionScore;

        [SerializeField]
        private string _displayName;
        public string displayName => _displayName;

        [SerializeField]
        protected InteractionType interactionType = InteractionType.Instantaneous;
        [SerializeField]
        private float _duration = 0.0f;
        public float duration => _duration;

        [SerializeField]
        // la interaccion puede afectar a mas de una estadistica
        private ChangedStat[] _changedStats;
        public ChangedStat[] changedStats => _changedStats;

        [SerializeField]
        private Outcome[] outcomes;


        protected virtual void Start()
        {
            smartObject = GetComponent<SmartObject>();
            foreach (Outcome outcome in outcomes)
            {
                outcome.init();
            }
        }

        public abstract bool isSomeonePerforming();

        // comprobar si se puede realizar la interaccion
        public abstract bool canPerform();

        public virtual bool canStillPerform() { return true; }

        // indicar que hay un usuario realizando la interaccion
        public abstract void lockInteraction(Performer performer);

        // realizar la accion
        public abstract void perform(Performer performer, UnityAction<BaseInteraction> onCompleted, UnityAction<BaseInteraction> onStopped);

        // indicar que hay un usuario que ha dejado de realizar la interaccion 
        public abstract void unlockInteraction(Performer performer);

        public Outcome pickOutcome()
        {
            if (outcomes.Length > 0)
            {
                // se normalizan las probabiliadd de todos los elementos
                // en funcion a la suma total de las probabilidades
                Outcome selectedOutcome = null;
                float probabilitySum = 0.0f;
                foreach (Outcome outcome in outcomes)
                {
                    probabilitySum += outcome.probability;
                }

                foreach (Outcome outcome in outcomes)
                {
                    outcome.normalisedProbability = outcome.probability / probabilitySum;
                }

                // se selecciona un elemento de forma aleatoria en base a su probabilidad
                float randProbability = Random.value;
                bool found = false;
                for (int i = 0; i < outcomes.Length && !found; ++i)
                {
                    Outcome outcome = outcomes[i];
                    if (randProbability <= outcome.normalisedProbability)
                    {
                        found = true;
                        selectedOutcome = outcome;
                    }
                }

                if (found)
                {
                    // se comprueba si el elemento detiene la interaccion
                    return selectedOutcome;
                }
            }
            return null;
        }

        // se va a utilizar cuando el usuario ejecute esta interaccion
        // entonces se aplica cada una de las estadisticas en la proporcion adecuada
        // (por si la accion tarda un tiempo en realizarse)
        public void applyStats(Performer performer, Outcome outcome, float proportion)
        {
            foreach (ChangedStat changedStat in changedStats)
            {
                StatType changedStatType = changedStat.targetStat;

                float outcomeMultiplier = 1.0f;
                if (outcome != null && outcome.statsMultipliersAux.ContainsKey(changedStatType))
                {
                    foreach(float multiplier in outcome.statsMultipliersAux[changedStatType])
                    {
                        outcomeMultiplier *= multiplier;
                    }
                }

                performer.updateIndividualStat(changedStat.targetStat, changedStat.value * proportion * outcomeMultiplier);
            }
        }
    }
}
