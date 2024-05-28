using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IAV24.Final
{
    public class TowerSleepInteraction : SimpleInteraction
    {
        private TowerSmartObject towerObject;

        [SerializeField]
        private int maxNearbyEnemies = 3;

        public void Start()
        {
            towerObject = GetComponent<TowerSmartObject>();
            if(towerObject == null )
            {
                Debug.LogError("No existe el smartObject Tower");
            }
        }

        protected override void onInteractionCompleted(Performer performer, UnityAction<BaseInteraction> onCompleted)
        {
            performer.gameObject.SetActive(true);
            base.onInteractionCompleted(performer, onCompleted);
        }

        protected override void onInteractionStopped(Performer performer, UnityAction<BaseInteraction> onStopped)
        {
            performer.gameObject.SetActive(true);
            base.onInteractionStopped(performer, onStopped);
        }

        public override bool canPerform()
        {
            return base.canPerform() && towerObject.nNearbyEnemies <= maxNearbyEnemies;
        }

        public override bool canStillPerform()
        {
            return base.canStillPerform() && towerObject.nNearbyEnemies <= maxNearbyEnemies;
        }

        public override void perform(Performer performer, UnityAction<BaseInteraction> onCompleted, UnityAction<BaseInteraction> onStopped)
        {
            performer.gameObject.SetActive(false);
            base.perform(performer, onCompleted, onStopped);
        }
    }
}