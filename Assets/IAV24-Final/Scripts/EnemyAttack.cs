using IAV24.Final;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private float damage = 1.0f;

    private void OnCollisionStay(Collision collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if(playerHealth != null)
        {
            playerHealth.makeDamage(damage);
        }
    }

    private void Start()
    {
        Debug.Log("start");
    }
}
