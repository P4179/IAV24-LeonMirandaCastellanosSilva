using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace IAV24.Final
{
    public class cameraMovement : MonoBehaviour
    {
        private Transform tr;

        [SerializeField]
        private float zoomAmount = 40f;
        private float initZoom;
        private float zoom;

        // Reference to the original position of the camera
        private Vector3 originalPosition;
        private Vector2 lastMousePos;
        public float dragSpeed = 2;
        private bool dragging = false;

        [SerializeField]
        Vector2 xBounds;
        [SerializeField]
        Vector2 zBounds;


        // Start is called before the first frame update
        void Start()
        {
            tr = Camera.main.transform;

            originalPosition = tr.position;
            initZoom = Camera.main.fieldOfView;
            zoom = initZoom;
        }

        // Update is called once per frame
        void Update()
        {
            // Zoom in
            if (Input.mouseScrollDelta.y > 0)
                zoom -= zoomAmount * Time.deltaTime;

            // Zoom out
            if (Input.mouseScrollDelta.y < 0)
            {
                zoom += zoomAmount * Time.deltaTime;

                // Se va recolocando la camara hacia la posicion original
                Vector3 direction = (originalPosition - tr.position).normalized;
                tr.position += direction * zoomAmount * Time.deltaTime;

            }

            // Se limita el zoom de la camara a un maximo y un minimo
            zoom = Mathf.Clamp(zoom, 5f, initZoom);
            if (zoom == initZoom) tr.position = originalPosition;
            Camera.main.fieldOfView = zoom;


            // Si se hace click derecho, se reinician la posicion y el zoom
            if (Input.GetMouseButtonDown(1))
            {
                tr.position = originalPosition;
                Camera.main.fieldOfView = initZoom;
                zoom = initZoom;
                return;
            }
            // Si se hace click izquierdo, comienza a arrastrar la camara
            else if (Input.GetMouseButtonDown(0))
            {
                dragging = true;
                lastMousePos = Input.mousePosition;
                return;
            }
            else if (!Input.GetMouseButton(0))
            {
                dragging = false;
                return;
            }

            // Si se esta arrastrando la camara y el zoom no esta cambiado, se mueve segun el movimiento del raton
            if (dragging && zoom != initZoom)
            {
                Vector2 mouseMovement = (Vector2)Input.mousePosition - lastMousePos;
                lastMousePos = Input.mousePosition;

                Vector3 movement = new Vector3(mouseMovement.x, 0, mouseMovement.y) * dragSpeed * Time.deltaTime;

                Vector3 newPosition = tr.position - movement;
                if(newPosition.x > xBounds.x && newPosition.x < xBounds.y && newPosition.z > zBounds.x && newPosition.z < zBounds.y)
                {
                    tr.position = newPosition;
                }
            }
        }

    }
}