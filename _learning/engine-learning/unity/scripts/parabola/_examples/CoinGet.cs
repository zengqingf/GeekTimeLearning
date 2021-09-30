public GameObject coin;

/// <summary>
/// 克隆抛物线型金币的方法
/// </summary>
/// <param name="pos">克隆第一个金币的位置</param>
/// <param name="num">克隆金币数量</param>
/// <param name="spacing">两两金币间的距离</param>
void LoadCoinMethod(Vector3 pos,int num,float spacing)
{
        Vector2 inityz = new Vector2(pos.z, pos.y);//记录抛物线第一个金币位置
        float sysmmetryAxis = inityz.x + (num / 2 + ((num % 2 == 0) ? 0.5f : 1)) * spacing;//计算抛物线对称轴
        float hight = inityz.y + Mathf.Pow((pos.z + spacing - sysmmetryAxis), 2) / num;//抛物线的零次参数
        for (int i = 0; i < num; i++)
        {
            pos.z += spacing;
            pos.y = hight - Mathf.Pow((pos.z - sysmmetryAxis), 2) / num;
            GameObject tempcoin = (GameObject)Instantiate(coin,pos,Quaternion.identity);
        }
}