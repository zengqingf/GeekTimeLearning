

namespace Tenmove.Runtime.EmbedUI
{
    internal class UIFormHotUpdate : UIFormProgress
    {
        private IUIImage m_WifiLabel;
        private IUIText m_NetSpeed;
        private IUIText m_DownloadBytes;

        public UIFormHotUpdate()
            :base()
        {
            m_WifiLabel = null;
            m_NetSpeed = null;
            m_DownloadBytes = null;
        }

        public void SetNetSpeedInfo(uint bytesPerSecond)
        {
            if (null != m_ProgressInfo)
            {
                string speedInfo = string.Empty;
                if (bytesPerSecond > 0)
                {
                    if (bytesPerSecond > 1024 * 1024)
                        speedInfo = string.Format("下载速度 {0} MB/s", ((bytesPerSecond * 1.0f) / (1024 * 1024)).ToString("0.0"));
                    else
                        speedInfo = string.Format("下载速度 {0} KB/s", ((bytesPerSecond * 1.0f) / 1024).ToString("0.0"));
                }
                m_NetSpeed.SetText(speedInfo);
            }
        }

        public void SetDownloadBytes(long downloadBytes,long totalBytes )
        {
            if (null != m_ProgressInfo)
            {
                string bytesInfo = string.Empty;
                if (totalBytes > 0)
                {
                    if (totalBytes > 1024 * 1024)
                        bytesInfo = string.Format("已下载 {0} MB/共 {1} MB", ((downloadBytes * 1.0f) / (1024 * 1024)).ToString("0.0"), ((totalBytes * 1.0f) / (1024 * 1024)).ToString("0.0"));
                    else
                        bytesInfo = string.Format("已下载 {0} KB/共 {1} KB", ((downloadBytes * 1.0f) / 1024).ToString("0.0"), ((totalBytes * 1.0f) / (1024 * 1024)).ToString("0.0"));
                }
                m_DownloadBytes.SetText(bytesInfo);
            }
        }

        public void SetNetRechabilityStateIcon(bool isWifiUsed)
        {
            if (null != m_WifiLabel)
                m_WifiLabel.SetVisible(isWifiUsed);
        }

        protected sealed override void _InitWidgets()
        {
            base._InitWidgets();
            m_WifiLabel = m_NativeUIForm.QureyUIControl<IUIImage>("Progress/WifiLabel");
            m_NetSpeed = m_NativeUIForm.QureyUIControl<IUIText>("Progress/NetSpeed");
            m_DownloadBytes = m_NativeUIForm.QureyUIControl<IUIText>("Progress/DownloadBytes");
        }

        protected override void _DeinitWidgets()
        {
            base._DeinitWidgets();
        }
    }
}