using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IAV24.Final
{
    public class BookOpen : SimpleInteraction
    {
        private BookSmartObject book;

        protected override void Start()
        {
            base.Start();
            book = GetComponent<BookSmartObject>();
        }

        protected override void onInteractionCompleted(Performer performer, UnityAction<BaseInteraction> onCompleted)
        {
            book.openBook(performer);
            base.onInteractionCompleted(performer, onCompleted);
        }

        public override bool canPerform()
        {
            return base.canPerform() && !book.isOpen;
        }
    }
}
