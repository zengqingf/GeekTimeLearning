

using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Random
        {
            static private System.Random m_Random = new System.Random(); 

            public static Vec2 GetRandomPointOnCircle()
            {
                int degree = m_Random.Next(0,360);
                float radian = Math.DegreeToRadian(degree);
                return new Vec2(Math.Sin(radian), Math.Cos(radian));
            }
        }
    }
}