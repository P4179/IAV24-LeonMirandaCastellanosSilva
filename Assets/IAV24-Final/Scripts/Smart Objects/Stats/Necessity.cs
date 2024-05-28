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
        private UnityEngine.UI.Image necessityBar;

        protected void updateNecessityBar()
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
            updateNecessityBar();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            currentValue = Mathf.Clamp01(currentValue - decayRate * Time.deltaTime);
            updateNecessityBar();
        }

        public override void updateIndividualStat(float amount)
        {
            currentValue = Mathf.Clamp01(currentValue + amount);
            updateNecessityBar();
        }

        public override float getCurrentValue01()
        {
            // por si acaso, aunque no hace falta
            return Mathf.Clamp01(currentValue);
        }

        public override float getChange01(float amount)
        {
            // por si acaso, aunque no hace falta
            return Mathf.Clamp01(amount);
        }
    }
}