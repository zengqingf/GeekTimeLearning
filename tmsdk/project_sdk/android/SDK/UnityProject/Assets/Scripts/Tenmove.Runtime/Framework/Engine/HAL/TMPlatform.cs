

namespace Tenmove.Runtime.HAL
{
    public interface ITMPlatformNativeInterface
    {
        void ExcuteRestartClientApplication();
        bool ExcuteInstallPackageWithPath(string packagePath);
        void ExcuteQuitApplication();
    }

    public class Platform
    {
        private static Platform sm_Instance = null;

        private ITMPlatformNativeInterface m_PlatformNativeInterface;
                
        public Platform()
        {
            m_PlatformNativeInterface = Utility.Assembly.CreateInstance("Tenmove.Runtime.HAL.Unity.UnityPlatformNativeInterface") as ITMPlatformNativeInterface;
        }

        static public void RestartClientApplication()
        {
            Platform platform = _GetInstance();
            if (null != platform && null != platform.m_PlatformNativeInterface)
                platform.m_PlatformNativeInterface.ExcuteRestartClientApplication();
        }

        static public void InstallPackage(string path)
        {
            Platform platform = _GetInstance();
            if (null != platform && null != platform.m_PlatformNativeInterface)
                platform.m_PlatformNativeInterface.ExcuteInstallPackageWithPath(path);
        }

        static public void QuitApplication()
        {
            Platform platform = _GetInstance();
            if (null != platform && null != platform.m_PlatformNativeInterface)
                platform.m_PlatformNativeInterface.ExcuteQuitApplication();
        }

        static private Platform _GetInstance()
        {
            if(null == sm_Instance)
                sm_Instance = new Platform();

            return sm_Instance;
        }
    }
}