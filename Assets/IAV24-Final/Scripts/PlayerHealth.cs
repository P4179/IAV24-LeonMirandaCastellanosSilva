using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
        [SerializeField]
        private UnityEngine.UI.Image healthBar;

        private Animator anim;

        private void updateHealthBar()
        {
            if(healthBar != null)
            {
                healthBar.fillAmount = currentHealth / maxHealth;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();

            if (currentHealth <= 0.0f || currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            updateHealthBar();
        }

        public void makeDamage(float damageAmount)
        {
            if (!gracePeriodEnabled)
            {
                currentHealth -= damageAmount;
                updateHealthBar();
                //Debug.Log(currentHealth);
                gracePeriodEnabled = true;
                if (currentHealth <= 0.0f)
                {
                    // Reproduce la animacion de muerte y llama a
                    // reiniciar el nivel una vez termina la animacion
                    anim.Play("Death");
                    StartCoroutine(resetLevel(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length));
                }
            }
        }

        private IEnumerator resetLevel(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            LevelManager.Instance.resetLevel();
        }

        public float getHealth() { return currentHealth; }

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