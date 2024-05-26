using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace IAV24.Final
{
    public class ManualMovementController : MonoBehaviour
    {
        private Camera mainCamera;
        private NavMeshAgent navMeshAgent;
        [SerializeField]
        private LayerMask walkableFloor;
        [SerializeField]
        private float speed = 9.0f;

        // Start is called before the first frame update
        void Start()
        {
            mainCamera = Camera.main;
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent != null)
            {
                navMeshAgent.speed = speed;
            }
            if (walkableFloor == 0)
            {
                walkableFloor = LayerMask.GetMask("WalkableFloor");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (navMeshAgent != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, float.MaxValue, walkableFloor))
                    {
                        if (navMeshAgent.destination != hit.point)
                        {
                            navMeshAgent.SetDestination(hit.point);
                        }
                    }
                }
            }
        }
    }
}