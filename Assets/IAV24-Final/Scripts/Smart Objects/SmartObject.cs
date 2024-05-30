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
        private Color feedbackColor = Color.red;
        private Color normalColor;
        private MeshRenderer meshRenderer;

        [SerializeField]
        private string displayName;

        [SerializeField]
        private Transform _interPointTransform;
        public Transform interPointTransform => _interPointTransform;

        public List<BaseInteraction> interactions { get; private set; }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            // se guardan todas las interacciones del objeto
            interactions = new List<BaseInteraction>(GetComponents<BaseInteraction>());

            SmartObjectManager.Instance.registerSmartObject(this);

            meshRenderer = GetComponent<MeshRenderer>();
            normalColor = meshRenderer.materials[1].color;
        }

        private void OnDestroy()
        {
            SmartObjectManager.Instance.deregisterSmartObject(this);
        }

        public void enableFeedback()
        {
            meshRenderer.materials[1].color = feedbackColor;
        }

        public void disableFeedback()
        {
            bool interactionExecuting = false;
            int i = 0;
            while (i < interactions.Count && !interactionExecuting)
            {
                if (interactions[i].isSomeonePerforming())
                {
                    interactionExecuting = true;
                }
                ++i;
            }
            if (!interactionExecuting)
            {
                meshRenderer.materials[1].color = normalColor;
            }
        }
    }
}
