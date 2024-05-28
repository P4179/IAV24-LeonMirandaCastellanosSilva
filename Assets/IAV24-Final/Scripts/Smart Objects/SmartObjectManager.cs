using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IAV24.Final
{
    // Registra todo los smart objects, de modo que el agente
    // puede saber cuales hay en el entorno y decidir a cual ir
    public class SmartObjectManager : MonoBehaviour
    {
        public static SmartObjectManager Instance { get; private set; }

        public List<SmartObject> registeredObjects { get; private set; } = new List<SmartObject>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public void registerSmartObject(SmartObject smartObject)
        {
            if (!registeredObjects.Contains(smartObject))
            {
                registeredObjects.Add(smartObject);
            }
        }

        public void deregisterSmartObject(SmartObject smartObject)
        {
            if (registeredObjects.Contains(smartObject))
            {
                registeredObjects.Remove(smartObject);
            }
        }
    }
}