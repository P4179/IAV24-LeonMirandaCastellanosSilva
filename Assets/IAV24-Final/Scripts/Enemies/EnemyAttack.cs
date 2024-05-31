using IAV24.Final;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IAV24.Final
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField]
        [Range(0f, 1f)]
        private float damage = 0.01f;

        private Animator anim;

        private void Start()
        {
            anim = transform.parent.gameObject.GetComponent<Animator>();
        }

        void OnTriggerStay(Collider collision)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Reproduce la animacion de ataque, reiniciandola cada vez que la reproduce,
                // y aplica el dano una vez ha terminado (en este caso, a la mitad de la
                // animacion, ya que coincide con el momento en el que el enemigo ataca)
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerHealth.inGracePeriod())
                {
                    anim.Play("Attack", 0, 0.0f);
                    StartCoroutine(makeDamage(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2, playerHealth, damage));
                }
            }
        }

        private IEnumerator makeDamage(float delay, PlayerHealth playerHealth, float damage)
        {
            yield return new WaitForSecondsRealtime(delay);
            playerHealth.getHit(damage);
        }
    }
}