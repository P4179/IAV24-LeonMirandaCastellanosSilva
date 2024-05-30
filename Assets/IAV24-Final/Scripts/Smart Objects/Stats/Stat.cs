using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace IAV24.Final
{
    public enum StatType { MagicPower, Energy, Hunger, Thirst }

    public abstract class Stat : MonoBehaviour
    {
        [SerializeField]
        private string _displayName;
        public string displayName => _displayName;

        [SerializeField]
        private StatType _type;
        public StatType type { get => _type; set => _type = value; }
        public float currentValue { get; set; } = 0.0f;

        protected virtual void Update()
        {
            decreaseStat();
            updateUI();
        }

        protected virtual void decreaseStat() { }

        protected abstract void updateUI();

        public abstract void updateIndividualStat(float amount);

        public abstract float getCurrentValue01();

        public abstract float getValue01(float amount);
    }
}