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
        private float currentHealth = 0.0f;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float initialhealth = 0.5f;
        [SerializeField]
        private float gracePeriod = 2.0f;
        [SerializeField]
        private UnityEngine.UI.Image healthBar;

        private Animator anim;

        private void updateBar()
        {
            if (healthBar != null)
            {
                healthBar.fillAmount = currentHealth;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();

            currentHealth = initialhealth;
            updateBar();
        }

        public void heal(float amount)
        {
            currentHealth = Mathf.Clamp01(currentHealth + amount);
            updateBar();
        }

        public void decreaseHealth(float amount)
        {
            currentHealth = Mathf.Clamp01(currentHealth - amount);
            updateBar();
            if (currentHealth <= 0.0f)
            {
                // Reproduce la animacion de muerte y llama a
                // reiniciar el nivel una vez termina la animacion
                anim.Play("Death");
                StartCoroutine(resetLevel(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length));
            }
        }

        public void getHit(float amount)
        {
            if (!gracePeriodEnabled)
            {
                // Reproduce la animacion de ataque, reiniciandola cada vez que la reproduce
                anim.Play("GetHit", 0, 0.0f);
                gracePeriodEnabled = true;
                decreaseHealth(amount);
            }
        }

        private IEnumerator resetLevel(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            LevelManager.Instance.resetLevel();
        }

        public bool inGracePeriod() { return gracePeriodEnabled; }

        private void Update()
        {
            if (gracePeriodEnabled)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime > gracePeriod)
                {
                    elapsedTime = 0.0f;
                    gracePeriodEnabled = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.I))
                decreaseHealth(0.01f);
            else if (Input.GetKeyDown(KeyCode.O))
                heal(0.01f);
        }
    }
}