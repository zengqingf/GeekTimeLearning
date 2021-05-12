using System;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// ref: https://imzlp.com/posts/13765/
/// </summary>
namespace UE4_INI_Helper
{
    public class IniReader
    {
        private string iniPath;

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public IniReader(string iniPath)
        {
            this.iniPath = iniPath;
        }

        public string ReadValue(string section, string key)
        {
            StringBuilder ReaderBuffer = new StringBuilder(255);
            int ret = GetPrivateProfileString(section, key, "", ReaderBuffer, 255, this.iniPath);
            return ReaderBuffer.ToString();
        }
    }
}
