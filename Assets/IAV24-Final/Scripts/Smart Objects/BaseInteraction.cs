using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IAV24.Final
{
    public enum InteractionType
    {
        Instantaneous = 0,   // se realiza de inmediato
        OverTime = 1    // tarda un tiempo
    }

    [System.Serializable]
    public class AppliedStat
    {
        public StatType targetStat; // estadistica a la que va a afectar
        public float value;     // cantidad que aumenta la estadistica
    }

    // Clase abstracta sobre las interacciones de un smart object
    // Tiene atributos y metodos para luego definir cuanto se tarda
    // en realizar la interaccion, que es lo que se hace...
    public abstract class BaseInteraction : MonoBehaviour
    {
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
        private AppliedStat[] _appliedStats;
        public AppliedStat[] appliedStats => _appliedStats;

        // comprobar si se puede realizar la interaccion
        public abstract bool canPerform();

        // indicar que hay un usuario realizando la interaccion
        public abstract void lockInteraction();

        // realizar la accion
        public abstract void perform(Performer performer, UnityAction<BaseInteraction> onCompleted);

        // indicar que hay un usuario que ha dejado de realizar la interaccion 
        public abstract void unlockInteraction();

        // se va a utilizar cuando el usuario ejecute esta interaccion
        // entonces se aplica cada una de las estadisticas en la proporcion adecuada
        // (por si la accion tarda un tiempo en realizarse)
        public void applyStats(Performer performer, float proportion)
        {
            foreach (AppliedStat appliedStat in appliedStats)
            {
                performer.updateIndividualStat(appliedStat.targetStat, appliedStat.value * proportion);
            }
        }
    }
}
