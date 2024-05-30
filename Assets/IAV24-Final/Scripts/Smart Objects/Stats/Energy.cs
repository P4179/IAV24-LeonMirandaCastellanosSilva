using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IAV24.Final
{
    public class Energy : Necessity
    {
        private LevelManager levelManager;

        [SerializeField]
        private float nightMultiplier = 2.0f;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            type = StatType.Energy;
            levelManager = LevelManager.Instance;
        }

        protected override void decreaseStat()
        {
            float multiplier = levelManager.night ? nightMultiplier : 1.0f;
            currentValue = Mathf.Clamp01((float)currentValue - decayRate * multiplier * Time.deltaTime);
        }
    }
}
