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

        protected virtual void Start()
        {
            smartObject = GetComponent<SmartObject>();
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

        // se va a utilizar cuando el usuario ejecute esta interaccion
        // entonces se aplica cada una de las estadisticas en la proporcion adecuada
        // (por si la accion tarda un tiempo en realizarse)
        public void applyStats(Performer performer, float proportion)
        {
            foreach (ChangedStat appliedStat in changedStats)
            {
                performer.updateIndividualStat(appliedStat.targetStat, appliedStat.value * proportion);
            }
        }
    }
}
