using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IAV24.Final {
    public enum StatType { MagicPower, Energy, Hunger, Thirsty }

    public abstract class Stat : MonoBehaviour
    {
        [SerializeField]
        private string _displayName;
        public string displayName => _displayName;

        [SerializeField]
        private StatType _type;
        public StatType type { get => _type; set => _type = value; }
        public float currentValue { get; set; }

        public abstract void updateIndividualStat(float amount);

        public abstract float getCurrentValue01();

        public abstract float getChange01(float amount);
    }
}