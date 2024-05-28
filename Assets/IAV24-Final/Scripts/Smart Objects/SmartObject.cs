using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IAV24.Final
{
    // Consiste en un contenedor de las interacciones que tiene
    // Por ejemplo, una TV puede tener una interaccion de encenderla, cambiar de canal...
    public class SmartObject : MonoBehaviour
    {
        [SerializeField]
        private string displayName;

        [SerializeField]
        private Transform _interPointTransform;
        public Transform interPointTransform => _interPointTransform;

        public List<BaseInteraction> interactions { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            // se guardan todas las interacciones del objeto
            interactions = new List<BaseInteraction>(GetComponents<BaseInteraction>());

            SmartObjectManager.Instance.registerSmartObject(this);
        }

        private void OnDestroy()
        {
            SmartObjectManager.Instance.deregisterSmartObject(this);
        }
    }
}
