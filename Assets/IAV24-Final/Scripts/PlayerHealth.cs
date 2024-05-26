using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace IAV24.Final
{
    public class PlayerHealth : MonoBehaviour
    {
        private bool gracePeriodEnabled = false;
        private float elapsedTime = 0.0f;

        [SerializeField]
        private float currentHealth;
        [SerializeField]
        private float maxHealth = 10.0f;
        [SerializeField]
        private float gracePeriod = 2.0f;

        // Start is called before the first frame update
        void Start()
        {
            if (currentHealth <= 0.0f)
            {
                currentHealth = maxHealth;
            }
        }

        public void makeDamage(float damageAmount)
        {
            if (!gracePeriodEnabled)
            {
                currentHealth -= damageAmount;
                Debug.Log(currentHealth);
                gracePeriodEnabled = true;
                if (currentHealth <= 0.0f)
                {
                    LevelManager.Instance.resetLevel();
                }
            }
        }

        private void Update()
        {
            if (gracePeriodEnabled)
            {
                elapsedTime += Time.deltaTime;
                if(elapsedTime > gracePeriod)
                {
                    elapsedTime = 0.0f;
                    gracePeriodEnabled = false;
                }
            }
        }
    }
}