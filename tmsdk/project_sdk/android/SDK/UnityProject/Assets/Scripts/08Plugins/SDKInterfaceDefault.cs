using UnityEngine;
using System.Collections;

namespace SDKClient
{
    public class SDKInterfaceDefault : SDKInterface
    {
        //TODO SVN Merge from 1.0
        public override string GetClipboardText()
        {
            return GUIUtility.systemCopyBuffer;
        }
    }
}