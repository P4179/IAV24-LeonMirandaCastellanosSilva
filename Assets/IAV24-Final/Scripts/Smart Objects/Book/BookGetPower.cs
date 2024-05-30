using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IAV24.Final
{
    public class BookGetPower : SimpleInteraction
    {
        public override bool canPerform()
        {
            return base.canPerform() && BookManager.Instance.allBooksOpened();
        }
    }
}
