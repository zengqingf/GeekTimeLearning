using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SDKClient;

public class WebTest : MonoBehaviour
{
    public RectTransform rt;
    public Image i1;
    public Image i2;
    public Text t;
    public Button show;
    public Button hide;
    public Button keyhide;
    public UniWebViewUtility u;

    // Start is called before the first frame update
    void Start()
    {
        
        //u.webViewChanged= i2;
        show.onClick.AddListener(() =>
        {
            u.InitWebView("https://www.gamersky.com");
            //u.ResetWebViewShow(false, 0.5f);

        });
        hide.onClick.AddListener(() =>
        {
            u.UnInitWebView();

        });
        keyhide.onClick.AddListener(() =>
        {
            //u.ResetWebViewShow(true, 500);

        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
