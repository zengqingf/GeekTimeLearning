/*
一：System.Windows.Forms.dll的使用：
System.Windows.Forms.dll要想在unity下使用，要先把System.Windows.Forms.dll放到unity的plugin文件下，这里存放了unity工程需要的DLL，注意System.Windows.Forms.dll需要是.net3.5以下的框架。该DLL可以在unity的安装目录下的Editor\Data\Mono\lib\mono\2.0\文件夹下找到。使用的方式也很简单，
*/

using UnityEngine;
using System;
using System.Windows.Forms;
using winforms = System.Windows.Forms;
public class test : MonoBehaviour
{
    void Start()
    {
        MessageBox.Show("此设备故障！请及时检查！", this.GetType().Name, winforms.MessageBoxButtons.OK, winforms.MessageBoxIcon.Error);
    }
}


/*
二：调用win32的系统方法
该方法是调用win32的系统方法，
*/
public class Messagebox
{
    [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr handle, String message, String title, int type);
}

public class test : MonoBehaviour
{
    void Start()
    {
    Messagebox.MessageBox(IntPtr.Zero,"还没有该场景！","xxx",0);
    }
}

/*
三：自己封装UGUI控件的方式。
*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Common;

public class UIMessage : MonoBehaviour {
    public Text Title;
    public Text Content;//这个是Content下的text
    public Button Sure;
    public Button Cancle;
    void Start()
    {
        Sure.onClick.AddListener(MessageBox.Sure);
        Cancle.onClick.AddListener(MessageBox.Cancle);
       Title.text =MessageBox.TitleStr;
       Content.text = MessageBox.ContentStr;
    }
}
using UnityEngine;
using System.Collections;
namespace Common
{
    public delegate void Confim();
    public class MessageBox 
    {
       static GameObject Messagebox;
       static int Result = -1;
        public static Confim confim;
        public static string TitleStr;
        public static string ContentStr;
        public static void Show(string content)
        {          
            ContentStr = "    " + content;
            Messagebox = (GameObject)Resources.Load("Prefab/Background");
            Messagebox= GameObject.Instantiate(Messagebox,GameObject.Find("Canvas").transform) as GameObject;
            Messagebox.transform.localScale = new Vector3(1, 1, 1);            
            Messagebox.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            Messagebox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            Messagebox.GetComponent<RectTransform>().offsetMax = Vector2.zero;      
            Time.timeScale = 1;      
        }
        public static void Show(string title,string content)
        {
            TitleStr = title;
            ContentStr ="    "+ content;
            Messagebox = (GameObject)Resources.Load("Prefab/Background");
            Messagebox = GameObject.Instantiate(Messagebox, GameObject.Find("Canvas").transform) as GameObject;
            Messagebox.transform.localScale = new Vector3(1, 1, 1);
            Messagebox.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            Messagebox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            Messagebox.GetComponent<RectTransform>().offsetMax = Vector2.zero;    
            Time.timeScale = 1;     
        }
        public static void Sure()
        {
            if (confim!= null)
            {
                confim();
                GameObject.Destroy(Messagebox);
                TitleStr = "标题";
                ContentStr = null;
                Time.timeScale = 0;
            }
        }     
        public static void Cancle()
        {
            Result = 2;
            GameObject.Destroy(Messagebox);
            TitleStr = "标题";
            ContentStr = null;
            Time.timeScale = 0;
        }


        public void test()
        {
            MessageBox.Show("XXX窗口","输出了XXXX");
            MessageBox.confim=()=>{
            //这里写你自己点击确定按钮后的操作
                Debug.Log("shabile");
            };
        }
    }
}
