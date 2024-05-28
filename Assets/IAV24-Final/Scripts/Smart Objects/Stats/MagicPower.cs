using BehaviorDesigner.Runtime.Tactical;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

namespace IAV24.Final
{
    public class MagicPower : Stat
    {
        private float cdDecrement = 0.0f;
        private List<GameObject> enemiesInRange = new List<GameObject>();
        private LayerMask avoidLayers;

        private int currentValueInt { get => (int)currentValue; set => currentValue = (int)value; }

        [SerializeField]
        private int initalPowerAmount = 0;
        [SerializeField]
        private int maxPowerAmount = 5;
        [SerializeField]
        private Transform shootPointTransform;
        [SerializeField]
        private GameObject bullet;
        [SerializeField]
        private float cooldown = 3.0f;
        [SerializeField]
        private float bulletVelocity = 3.0f;
        [SerializeField]
        private TextMeshProUGUI magicPowerInfo;

        private Animator anim;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Enemy" && !enemiesInRange.Contains(other.gameObject))
            {
                enemiesInRange.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            removeDetectedEnemy(other.gameObject);
        }

        private void updateMagicPowerInfo()
        {
            if (magicPowerInfo != null)
            {
                magicPowerInfo.text = (currentValueInt).ToString();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            anim = transform.parent.gameObject.GetComponent<Animator>();
            
            avoidLayers = LayerMask.GetMask("Player") | LayerMask.GetMask("EnemyDamageZone");
            type = StatType.MagicPower;
            currentValueInt = initalPowerAmount;
            updateMagicPowerInfo();
        }

        private GameObject findFirstEnemyNonHidden()
        {
            GameObject target = null;
            if (enemiesInRange.Count > 0)
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
                        if (hitInfo.collider.gameObject == enemiesInRange[i])
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
            cdDecrement -= Time.deltaTime;
            cdDecrement = Mathf.Max(cdDecrement, 0.0f);
            if(cdDecrement <= 0.0f)
            {
                shoot();
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
            if (currentValueInt > 0)
            {
                GameObject enemyTarget = findFirstEnemyNonHidden();
                if (enemyTarget != null)
                {
                    // Reproduce la animacion de ataque, reiniciandola cada vez que la reproduce
                    anim.Play("Attack", 0, 0.0f);

                    cdDecrement = cooldown;

                    currentValueInt = currentValueInt - 1;
                    updateMagicPowerInfo();

                    GameObject bulletInstance = Instantiate(bullet, shootPointTransform.position, Quaternion.identity);
                    Rigidbody bulletRigidBody = bulletInstance.GetComponent<Rigidbody>();
                    MagicBullet magicShot = bulletInstance.GetComponent<MagicBullet>();
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

        public override void updateIndividualStat(float amount)
        {
            currentValueInt = currentValueInt + (int)amount;
            if(currentValueInt > maxPowerAmount)
            {
                currentValueInt = maxPowerAmount;
            }
            updateMagicPowerInfo();

        }

        public override float getCurrentValue01()
        {
            return currentValue / maxPowerAmount;
        }

        public override float getChange01(float amount)
        {
            return amount / maxPowerAmount;
        }
    }
}