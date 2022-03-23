using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMSDKClient
{
    public static class GUIDGenerator
    {   
        public static string GetUUID()
        {
            return System.Guid.NewGuid().ToString("N");
        }
    }
}
