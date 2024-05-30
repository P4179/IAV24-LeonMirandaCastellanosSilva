using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

namespace IAV24.Final
{
    public class BookSmartObject : SmartObject
    {
        [SerializeField]
        private Mesh openedBookMesh;
        [SerializeField]
        Vector3 openedBookRot;
        [SerializeField]
        GameObject book;
        private MeshFilter bookMeshFilter;
        private Transform bookTransform;

        public bool isOpen { get; private set; }

        protected override void Start()
        {
            base.Start();
            BookManager.Instance.addClosedBook();
            bookMeshFilter = book.GetComponent<MeshFilter>();
            bookTransform = book.GetComponent<Transform>();
        }

        public void openBook(Performer performer)
        {
            isOpen = true;
            if (bookMeshFilter != null)
            {
                bookMeshFilter.mesh = openedBookMesh;
                bookTransform.localRotation = Quaternion.Euler(openedBookRot);
            }
            BookManager.Instance.registerOpenedBook(performer);
        }
    }
}