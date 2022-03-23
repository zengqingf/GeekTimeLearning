
namespace Tenmove.Runtime
{
    internal sealed partial class AssetManager
    {
        private class Asset : Referable
        {
            private readonly string m_Name;
            private readonly int m_NameHashCode;
            private readonly ITMAssetObject m_AssetObject;      
            private readonly AssetPackage m_AssetPackage;
            private readonly ITMReferencePool<AssetPackage> m_AssetPackagePool;
            private long m_TimeStamp = ~0;

            private bool   m_Locked;

            public Asset(string assetName,ITMAssetObject assetObject,AssetPackage assetPackage,ITMReferencePool<AssetPackage> assetPackagePool)
            {
                TMDebug.Assert(!string.IsNullOrEmpty(assetName),"Asset name can not be null or empty string!");
                TMDebug.Assert(null != assetObject,"Asset object can not be null!");

                m_Name = assetName;
                m_NameHashCode = assetName.GetHashCode();
                m_AssetPackage = assetPackage;
                if(null != m_AssetPackage)
                    m_AssetPackage.SpawmAsset();

                m_AssetPackagePool = assetPackagePool;
                m_AssetObject = assetObject;

                // �´�����Դ����Ҫ���lock״̬
                _CheckLockState();
            }

            public object CreateAssetInst(bool overrideTransform,Math.Transform transform)
            {
                return m_AssetObject.CreateAssetInst(overrideTransform,transform);
            }

            public override void OnSpawn()
            {
                m_TimeStamp = Utility.Time.GetTicksNow();
            }

            public override void OnUnspawn()
            {
            }

            public override void OnRelease()
            {
                if (null != m_AssetPackage)
                {
                    m_AssetPackage.UnspawnAsset();
                    m_AssetPackagePool.Unspawn(m_AssetPackage);
                }
            }

            public override string Name
            {
                get
                {
                    return m_Name;
                }
            }            

            public override int NameHashCode
            {
                get
                {
                    return m_NameHashCode;
                }
            }

            public override bool IsInUse
            {
                get
                {
                    return m_AssetObject.IsInUse;
                }
            }

            public override bool IsLocked
            {
                get
                {
                    return m_Locked;
                }
            }

            public override long LastUseTime
            {
                get
                {
                    return m_TimeStamp;
                }
            }

            public override int SpawnCount
            {
                get
                {
                    return m_AssetObject.SpawnCount;
                }
            }

            public override void Lock(bool bLock)
            {
                m_Locked = bLock;

                // �������������Դ����lockʱ��Ҫ��ȡǿ����
                if (m_Locked)
                {
                    m_AssetObject.Lock();
                }
                else
                {
                    m_AssetObject.Unlock();
                }
            }

            private void _CheckLockState()
            {
                m_Locked = TMAssetManagerHelper.IsAssetLocked(Name);
                if (m_Locked)
                {
                    m_AssetObject.Lock();
                }
                else
                {
                    m_AssetObject.Unlock();
                }
            }
        }
    }
}