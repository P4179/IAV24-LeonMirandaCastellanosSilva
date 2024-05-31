using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IAV24.Final
{
    public class Necessity : Stat
    {
        [SerializeField]
        [Range(0f, 1f)]
        private float initialValue = 0.5f;
        [SerializeField]
        [Range(0f, 1f)]
        protected float decayRate = 0.005f;   // aprox 100 segundos en caer por completo

        [SerializeField]
        [Range(0f, 1f)]
        private float healthLimit = 0.01f;
        [SerializeField]
        [Range(0f, 1f)]
        private float healthDecayRate = 0.003f;

        [SerializeField]
        private UnityEngine.UI.Image necessityBar;

        private PlayerHealth health;

        protected override void updateUI()
        {
            if (necessityBar != null)
            {
                necessityBar.fillAmount = currentValue;
            }
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            currentValue = initialValue;
            updateUI();
            health = GetComponent<PlayerHealth>();
        }

        protected override void decreaseStat()
        {
            currentValue = Mathf.Clamp01(currentValue - decayRate * Time.deltaTime);

            if (currentValue <= healthLimit)
            {
                health?.decreaseHealth(healthDecayRate * Time.deltaTime);
            }
        }

        public override void updateIndividualStat(float amount)
        {
            currentValue = Mathf.Clamp01(currentValue + amount);
            updateUI();
        }

        public override float getCurrentValue01()
        {
            // por si acaso, aunque no hace falta
            return Mathf.Clamp01(currentValue);
        }

        public override float getValue01(float value)
        {
            // por si acaso, aunque no hace falta
            return Mathf.Clamp01(value);
        }
    }
}