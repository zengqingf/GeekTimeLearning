
using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal sealed partial class AssetManager
    {
        internal delegate void OnAssetTreeBuildComplete();

        internal class AssetTree
        {
            readonly AssetManager m_AssetManager;
            readonly OnAssetTreeBuildComplete m_OnAssetTreeBuildComplete; 

            public AssetTree(AssetManager assetManager)
            {
                TMDebug.Assert(null != assetManager, "Asset manager object can not be null!");
                m_AssetManager = assetManager;
                m_OnAssetTreeBuildComplete = m_AssetManager._OnAssetTreeBuildComplete;
            }

            public void BuildAssetTree(ITMAssetTreeData assetTree)
            {
                if (null == assetTree)
                {
                    TMDebug.LogErrorFormat("Asset tree data can not be null!");
                    return;
                }
           
                List<AssetDesc> assetDescList = assetTree.GetAssetDescMap();
                List<AssetPackageDesc> assetPackageDescList = assetTree.GetAssetPackageDescMap();

                List<LinearMap<string, AssetDesc>.KeyValuePair<string, AssetDesc>> assetDescTable = new List<LinearMap<string, AssetDesc>.KeyValuePair<string, AssetDesc>>();
                for(int i = 0,icnt = assetDescList.Count;i<icnt;++i)
                {
                    AssetDesc curAssetDesc = assetDescList[i];
                    assetDescTable.Add(new LinearMap<string, AssetDesc>.KeyValuePair<string, AssetDesc>(curAssetDesc.AssetName, curAssetDesc));
                }

                List<LinearMap<AssetPackageName, AssetPackageDesc>.KeyValuePair<AssetPackageName, AssetPackageDesc>> assetPackageDescTable = new List<LinearMap<AssetPackageName, AssetPackageDesc>.KeyValuePair<AssetPackageName, AssetPackageDesc>>();
                for (int i = 0, icnt = assetPackageDescList.Count; i < icnt; ++i)
                {
                    AssetPackageDesc curAssetPackageDesc = assetPackageDescList[i];
                    assetPackageDescTable.Add(new LinearMap<AssetPackageName, AssetPackageDesc>.KeyValuePair<AssetPackageName, AssetPackageDesc>(curAssetPackageDesc.PackageName, curAssetPackageDesc));
                }

                m_AssetManager.m_AssetDescTable.Fill(assetDescTable,true);
                m_AssetManager.m_AssetPackageDescTable.Fill(assetPackageDescTable, true);

                if (null != m_OnAssetTreeBuildComplete)
                    m_OnAssetTreeBuildComplete();
            }

            private void _ClearAssetTree()
            {
                m_AssetManager.m_AssetDescTableIsReady = false;
                m_AssetManager.m_AssetDescTable.Clear();
                m_AssetManager.m_AssetPackageDescTableIsReady = false;
                m_AssetManager.m_AssetPackageDescTable.Clear();
            }
        }
    }
}