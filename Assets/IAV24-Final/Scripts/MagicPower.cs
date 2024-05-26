using BehaviorDesigner.Runtime.Tactical;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

namespace IAV24.Final
{
    public class MagicPower : MonoBehaviour
    {
        private int powerAmount;
        private float elapsedTime = 0.0f;
        private bool cantShoot = true;
        private List<GameObject> enemiesInRange = new List<GameObject>();
        private Transform shootPointTransform;
        private LayerMask avoidLayers;

        [SerializeField]
        private int maxPowerAmount = 5;
        [SerializeField]
        private GameObject shootPoint;
        [SerializeField]
        private GameObject bullet;
        [SerializeField]
        private float cooldown = 3.0f;
        [SerializeField]
        private float bulletVelocity = 3.0f;
        [SerializeField]
        private TextMeshProUGUI magicPowerInfo;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Enemy" && !enemiesInRange.Contains(other.gameObject))
            {
                enemiesInRange.Add(other.gameObject);
            }

            // tambien cuando se recargue y si hay algun enemigo en la zona
            shoot();
        }

        private void OnTriggerExit(Collider other)
        {
            removeDetectedEnemy(other.gameObject);
        }

        private void updateMagicPowerInfo()
        {
            if(magicPowerInfo != null)
            {
                magicPowerInfo.text = powerAmount.ToString();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            shootPointTransform = shootPoint.GetComponent<Transform>();
            avoidLayers = LayerMask.GetMask("Player") | LayerMask.GetMask("EnemyDamageZone");
            if (powerAmount <= 0 || powerAmount > maxPowerAmount)
            {
                powerAmount = maxPowerAmount;
            }
            updateMagicPowerInfo();
        }

        private GameObject findFirstEnemyNonHidden()
        {
            GameObject target = null;
            if(enemiesInRange.Count > 0)
            {
                bool found = false;
                int i = 0;
                while (i < enemiesInRange.Count && !found)
                {
                    RaycastHit hitInfo;
                    Vector3 direction = enemiesInRange[i].transform.position - shootPointTransform.position;
                    Debug.DrawLine(shootPointTransform.position, enemiesInRange[i].transform.position, Color.red);
                    if (Physics.Raycast(shootPointTransform.position, direction, out hitInfo, direction.magnitude, ~avoidLayers))
                    {
                        if(hitInfo.collider.gameObject == enemiesInRange[i])
                        {
                            target = enemiesInRange[i];
                            found = true;
                        }
                    }
                    ++i;
                }
            }
            return target;
        }
        

        // Update is called once per frame
        void Update()
        {
            if (!cantShoot)
            {
                elapsedTime += Time.deltaTime;
                if(elapsedTime > cooldown)
                {
                    elapsedTime = 0.0f;
                    cantShoot = true;
                }
            }
        }

        public void removeDetectedEnemy(GameObject enemy)
        {
            if (enemy.tag == "Enemy" && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
        }

        public void shoot()
        {
            if (cantShoot && powerAmount > 0)
            {
                GameObject enemyTarget = findFirstEnemyNonHidden();
                if (enemyTarget != null)
                {
                    cantShoot = false;
                    --powerAmount;
                    updateMagicPowerInfo();
                    GameObject bulletInstance = Instantiate(bullet, shootPointTransform.position, Quaternion.identity);
                    Rigidbody bulletRigidBody = bulletInstance.GetComponent<Rigidbody>();
                    MagicShot magicShot = bulletInstance.GetComponent<MagicShot>();
                    if (bulletRigidBody != null && magicShot != null)
                    {
                        Collider enemyCollider = enemyTarget.GetComponent<Collider>();
                        if (enemyCollider != null)
                        {
                            bulletRigidBody.velocity = bulletVelocity * (enemyCollider.bounds.center - shootPointTransform.position);
                        }
                        else
                        {
                            bulletRigidBody.velocity = bulletVelocity * (enemyTarget.transform.position - shootPointTransform.position);
                        }
                        magicShot.setParams(this, cooldown);
                    }
                }
            }
        }
    }
}