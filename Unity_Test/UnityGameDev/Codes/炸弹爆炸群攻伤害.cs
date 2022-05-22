
/*
ref: https://www.cnblogs.com/jqg-aliang/p/4605944.html
群攻伤害指在一定范围内同时对多游戏对象按距离衰减计算伤害。
Unity对此有相关的接口，可以很方便的实现这个功能。

Collider[] Physics.OverlapSphere ( Vector3 position, float radius,int layerMask ) ;
 　　Vector3 position：爆炸目标点
 　　float radius：爆炸半径
 　　int layerMask：影响层（指的是对某一层进行爆炸计算）

rigidbody.AddExplosionForce(power, explosionPos, radius, 3.0F);//对刚体施加爆炸力，物理的视觉效果，具体我就不介绍了


这个函数的返回值是Collider[]，意指获取position为中心，radius为半径，layerMask层中的碰撞体集合。
获取这个数组之后，就可以进行伤害计算了。
如果写得比较正规一点，你需要两个类，一个是生命体，一个是武器类。
*/

//武器类部分代码（主要是武器属性的声明）
public class ObjectWeapon  {
    private float range;//爆炸范围
 
    public float Range
    {
        get { return range; }
        set { range = value; }
    }
    private float power;//力量
 
    public float Power
    {
        get { return power; }
        set { power = value; }
    }
    private float damage;//伤害值
 
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    private bool isAP;//是否穿甲
 
    public bool IsAP
    {
        get { return isAP; }
        set { isAP = value; }
    }
}

//抽象生命体类
public interface Life  {
    void Death();
    void HPManager(float life);//生命数值计算
    bool IsAP();
}


public class TestCode
{
    public void Bomb(ObjectWeapon weapon,Vector3 point,int layer)//爆炸函数
    {
        Collider[] others = Physics.OverlapSphere(point, weapon.Range,lay);//获取所有碰撞体
        Rigidbody other;//刚体，通过添加力实现爆炸的视觉效果
        Life life;//生命体
        for (int i = 0; i < others.Length; i++) { 
            //others[i].
            if ((other=others[i].GetComponent<Rigidbody>())) {//检测刚体
                other.AddExplosionForce(weapon.Power,point,weapon.Range,10);//这个函数会自动根据距离给刚体衰减的力
            }
            if ((life = others[i].GetComponent<Life>()) != null && (!life.IsAP() || weapon.IsAP))//life.IsAP()意指生命体是否具有‘装甲’，weapon.IsAP判断武器是否‘穿甲’
            { 
                //如果装甲，则需要能够穿甲的武器才能计算伤害， 
    　　　　　　 //通过计算武器的杀伤范围与物体和爆炸点的距离来计算伤害，实现距离衰减
                life.HPManager(weapon.Damage * (1-Vector3.Distance(others[i].transform.position, point) / weapon.Range));//根据距离衰减判断伤害值  
                //通过物体到爆炸点的距离与武器爆炸范围的比值，实现衰减效果
            }
        } 
    }
}