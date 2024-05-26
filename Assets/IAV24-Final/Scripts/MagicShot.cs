using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IAV24.Final
{
    public class MagicShot : MonoBehaviour
    {
        private MagicPower magicPower;
        private float lifeTime;
        private float elapsedTime = 0.0f;

        private void OnCollisionEnter(Collision collision)
        {
            GameObject colObject = collision.gameObject;
            if (colObject.tag == "Enemy")
            {
                magicPower.removeDetectedEnemy(colObject);
                Destroy(colObject);
            }
            Destroy(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > lifeTime)
            {
                elapsedTime = 0.0f;
                Destroy(this.gameObject);
            }
        }

        public void setParams(MagicPower magicPower, float lifeTime)
        {
            this.magicPower = magicPower;
            this.lifeTime = lifeTime;
        }
    }
}