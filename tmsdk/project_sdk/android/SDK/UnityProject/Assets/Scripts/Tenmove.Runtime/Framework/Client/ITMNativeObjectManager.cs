

using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public enum NativeOperateState
    {
        OK,
        Failed,
    }

    public enum ObjectLoadFlag
    {
        None = 0x00,
        LoadFromPool = 0x01,
        HideAfterLoad = 0x02,
    }

    /// <summary>
    /// 节点对象加载完成代理回调
    /// </summary>
    /// <param name="nodeObj">节点对象</param>
    /// <param name="requestID">请求的ID</param>
    /// <param name="errorMsg">错误消息，如果加载成功传回空字符串（不是null）</param>
    public delegate void OnNativeObjLoaded(NativeOperateState state, ITMNativeObject nodeObj,string resPath,int requestID,string errorMsg);

    public interface ITMNativeObjectManager
    {
        /// <summary>
        /// 从内存里创建本地节点对象 （立即返回）
        /// </summary>
        /// <param name="name">节点对象资源路径</param>
        /// <param name="userData">用户数据</param>
        /// <returns>本次请求的ID</returns>
        ITMNativeObject CreateNativeObject(string name, string[] components);

        /// <summary>
        /// 从存储器中加载本地节点对象
        /// </summary>
        /// <param name="resPath">节点对象资源路径</param>
        /// <param name="userData">用户数据</param>
        /// <param name="onLoaded">节点对象加载完成回调</param>
        /// <returns>本次请求的ID</returns>
        int LoadNativeNodeObject(string resPath, string[] components, OnNativeObjLoaded onLoaded, EnumHelper<ObjectLoadFlag> loadFlag);

        /// <summary>
        /// 从存储器中加载本地节点对象
        /// </summary>
        /// <param name="resPath">节点对象资源路径</param>
        /// <param name="transform">初始变换</param>
        /// <param name="userData">用户数据</param>
        /// <param name="onLoaded">节点对象加载完成回调</param>
        /// <returns>本次请求的ID</returns>
        int LoadNativeNodeObject(string resPath, Math.Transform transform, string[] components, OnNativeObjLoaded onLoaded, EnumHelper<ObjectLoadFlag> loadFlag);

        /// <summary>
        /// 从节点抽取指定标签的子节点
        /// </summary>
        /// <typeparam name="TNativeObject"></typeparam>
        /// <param name="root">根节点</param>
        /// <param name="extractTag">指定标签</param>
        /// <param name="extractedObjests">返回满足条件的节点</param>
        void ExtractNativeObjectsByTag(ITMNativeObject root, string extractTag, List<ITMNativeObject> extractedObjests);

        /// <summary>
        /// 从节点抽取指定名字的子节点
        /// </summary>
        /// <typeparam name="TNativeObject"></typeparam>
        /// <param name="root">根节点</param>
        /// <param name="extractName">指定名称</param>
        /// <param name="extractedObjests">返回满足条件的节点</param>
        void ExtractNativeObjectsByName(ITMNativeObject root, string extractName, List<ITMNativeObject> extractedObjests);

        /// <summary>
        /// 从节点抽取含有指定组件的子节点
        /// </summary>
        /// <typeparam name="TNativeObject"></typeparam>
        /// <param name="root">根节点</param>
        /// <param name="componentType">指定组件类型</param>
        /// <param name="extractedObjests">返回满足条件的节点</param>
        void ExtractNativeObjectsByComponent(ITMNativeObject root, Type componentType, List<ITMNativeObject> extractedObjests);
    }
}