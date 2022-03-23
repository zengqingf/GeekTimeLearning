using SDKClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class ScreenShot : MonoBehaviour
{
    public Button b1;

    public new Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        b1.onClick.AddListener(() =>
        {
            StartCoroutine(SS());
        });
        Debug.LogError(PluginManager.Instance.GetSystemVersion());
        Debug.LogError(SDKInterface.Instance.TryGetCurrVersionAPI());
        Debug.LogError(PluginManager.Instance.IsLowVersion(21,9));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator SS()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture tmp = RenderTexture.GetTemporary(Screen.width, Screen.height);

        //var a = camera.targetTexture;
        //RenderTexture.active = camera.targetTexture;
        //camera.Render();

        Texture2D image = new Texture2D(Screen.width, Screen.height);
        image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        image.Apply();

        //RenderTexture.active = tmp;
        //RenderTexture.ReleaseTemporary(tmp);

        byte[] bytes = image.EncodeToJPG();
        File.WriteAllBytes("a.jpg", bytes);
        //SDKInterface.Instance.SaveImage2Album(bytes);
    }

}
