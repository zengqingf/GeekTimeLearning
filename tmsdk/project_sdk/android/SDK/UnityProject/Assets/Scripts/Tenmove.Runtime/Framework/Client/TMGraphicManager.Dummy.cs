

using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public partial class GraphicManager 
    {
        public class DummyNativeObjectManager : ITMNativeObjectManager
        {
            private GraphicManager m_GraphicManager;

            public void Init(GraphicManager gm)
            {
                m_GraphicManager = gm;
            }
            
            public ITMNativeObject CreateNativeObject(string name, string[] components)
            {
                return null;
            }

            public int LoadNativeNodeObject(string resPath,string[] components, OnNativeObjLoaded onLoaded, EnumHelper<ObjectLoadFlag> loadFlag)
            {
                return m_GraphicManager._DummyLoadObject(resPath, components, onLoaded, loadFlag);
            }

            public int LoadNativeNodeObject(string resPath, Math.Transform transform, string[] components, OnNativeObjLoaded onLoaded, EnumHelper<ObjectLoadFlag> loadFlag)
            {
                return m_GraphicManager._DummyLoadObject(resPath, components, onLoaded, loadFlag);
            }
            
            public void ExtractNativeObjectsByTag(ITMNativeObject root, string extractTag, List<ITMNativeObject> extractedObjests)
            {
            }
            
            public void ExtractNativeObjectsByName(ITMNativeObject root, string extractName, List<ITMNativeObject> extractedObjests)
            {
            }
            
            public void ExtractNativeObjectsByComponent(ITMNativeObject root, Type componentType, List<ITMNativeObject> extractedObjests)
            {
            }
        }
    }
}