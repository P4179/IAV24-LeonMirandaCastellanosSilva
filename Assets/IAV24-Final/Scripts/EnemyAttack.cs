using IAV24.Final;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private float damage = 1.0f;


    void OnTriggerStay(Collider collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if(playerHealth != null)
        {
            playerHealth.makeDamage(damage);
        }
    }
}
