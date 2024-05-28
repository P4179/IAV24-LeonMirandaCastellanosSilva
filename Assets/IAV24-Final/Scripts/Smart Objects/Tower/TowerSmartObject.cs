using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IAV24.Final
{
    public class TowerSmartObject : SmartObject
    {
        private List<GameObject> nearbyEnemies = new List<GameObject>();
        public int nNearbyEnemies => nearbyEnemies.Count;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Enemy" && !nearbyEnemies.Contains(other.gameObject))
            {
                nearbyEnemies.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Enemy" && nearbyEnemies.Contains(other.gameObject))
            {
                nearbyEnemies.Remove(other.gameObject);
            }
        }
    }
}