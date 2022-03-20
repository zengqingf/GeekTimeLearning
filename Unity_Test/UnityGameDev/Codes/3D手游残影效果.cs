//ref: https://blog.csdn.net/qq18052887/article/details/51094142
//1.间隔一定时间创建一个残影模型
GameObject go = GameObject.Instantiate(origin, pos, dir) as GameObject;
//2.对残影模型采用特殊的shader,要简单高效
    public class MotionGhost
    {
        public GameObject m_GameObject;
        public List<Material> m_Materials;
        public float m_DurationTime;
        public float m_FadeTime;
        public float m_Time;

        public MotionGhost(GameObject go, Color color, float durationTime, float fadeTime)
        {
            m_GameObject = go;
            m_DurationTime = durationTime;
            m_FadeTime = fadeTime;
            m_Time = durationTime;
 
            if (MotionGhostMgr.Instance.m_MaterialMotionGhost == null)
                MotionGhostMgr.Instance.m_MaterialMotionGhost = Resources.Load("Material/MotionGhost");
 
            m_Materials = new List<Material>();
            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
            {
                if (renderer as MeshRenderer || renderer as SkinnedMeshRenderer)
                {
                    // 身体和武器
                    Material[] newMaterials = new Material[1];
                    newMaterials[0] = GameObject.Instantiate(MotionGhostMgr.Instance.m_MaterialMotionGhost) as Material;               
                    if (newMaterials[0].HasProperty("_RimColor"))
                        newMaterials[0].SetColor("_RimColor", color);
                    renderer.materials = newMaterials;
 
                    m_Materials.Add(renderer.material);
                }
                else
                {
                    // 隐藏粒子
                    renderer.enabled = false;
                }
            }
        }

        //3.残影淡入淡出的处理
        public void Tick()
        {
            for (int i = m_MotionGhosts.Count - 1; i >= 0; --i)
            {
                m_MotionGhosts[i].m_Time -= Time.deltaTime;
 
                // 时间到了删除掉
                if (m_MotionGhosts[i].m_Time <= 0)
                {
                    GameObject.Destroy(m_MotionGhosts[i].m_GameObject);
                    m_MotionGhosts.RemoveAt(i);
                    --m_Counter;
                    continue;
                }
 
                // 开始材质渐变
                if (m_MotionGhosts[i].m_Time < m_MotionGhosts[i].m_FadeTime)
                {
                    float alpha = m_MotionGhosts[i].m_Time / m_MotionGhosts[i].m_FadeTime;
                    foreach (Material material in m_MotionGhosts[i].m_Materials)
                    {
                        if (material.HasProperty("_RimColor"))
                        {
                            Color color = material.GetColor("_RimColor");
                            color *= alpha;
                            material.SetColor("_RimColor", color);
                        }
                    }
                }             
            }
        }
    }
