

using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public interface ITMGeObjectManager
    {
        GeObject CreateObject(Type objectType, string name, GeObjectParams objParams, string[] componentes = null);
        TGeObject CreateObject<TGeObject>(string name, GeObjectParams objParams, string[] componentes = null) where TGeObject : GeObject, new();
        TGeObject LoadObject<TGeObject>(string resPath, EnumHelper<ObjectLoadFlag> flags, GeObjectParams objParams, string[] componentes = null) where TGeObject : GeObject, new();
        TGeObject LoadObject<TGeObject>(string resPath, Math.Transform transform, EnumHelper<ObjectLoadFlag> flags, GeObjectParams objParams, string[] componentes = null) where TGeObject : GeObject, new();

        void ExtractNodesWithTag<TGeNode>(List<TGeNode> extractNodes, GeNode root, string tag, GeObjectParams objParams) where TGeNode : GeNode, new();
        void ExtractNodesWithName<TGeNode>(List<TGeNode> extractNodes, GeNode root, string name, GeObjectParams objParams) where TGeNode : GeNode, new();
        void ExtractNodesWithComponent<TGeNode, TComponent>(List<TGeNode> extractNodes, GeNode root, GeObjectParams objParams) where TGeNode : GeNode, new() where TComponent : ITMNativeComponent;

        void DestroyObject(GeObject node); 
    }
}