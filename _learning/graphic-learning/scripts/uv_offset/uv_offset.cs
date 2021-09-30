using UnityEngine;

//https://blog.csdn.net/maomaoxiaohuo/article/details/51052420
public class UVOffset: MonoBehaviour {

    // U轴方向滚动速度
    public float USpeed = 1.0f;
    // U轴方向平铺个数
    public int UCount = 1;

    // V轴方向滚动速度
    public float VSpeed = 1.0f;
    // V轴方向平铺个数
    public int VCount = 1;

    private Material mat = null;

    private void Awake()
    {
        if (this.renderer != null)
            this.mat = this.renderer.material;
    }

    private void Update()
    {
        if (this.mat == null)
            return;

        Vector2 offset = this.mat.GetTextureOffset("_MainTex");
        Vector2 tiling  = this.mat.GetTextureScale("_MainTex");

        offset.x += Time.deltaTime * USpeed;
        offset.y += Time.deltaTime * VSpeed;
        tiling.x = UCount;
        tiling.y = VCount;

        this.mat.SetTextureOffset("_MainTex", offset);
        this.mat.SetTextureScale("_MainTex", tiling);
    }
}
