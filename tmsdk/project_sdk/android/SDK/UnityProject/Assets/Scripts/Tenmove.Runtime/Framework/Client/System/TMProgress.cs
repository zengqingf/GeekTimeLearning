

using System;
using System.Collections;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public sealed class ProgressUpdateEventArgs : BaseEventArgs
    {
        public ProgressUpdateEventArgs(float progress)
        {
            Progress = progress;
        }

        public float Progress
        {
            get;
            private set;
        }
    }

    public interface ITMProgress
    {
        event EventHandler<ProgressUpdateEventArgs> ProgressUpdateEventHandler;

        float CurrentProgress
        {
            get;
        }
    }

    public interface ITMProgressSegment
    {
        ///bool StepProgress();
        //IEnumerator StepProgress();

        void SetProgress(float progress,uint watermark);
    }

    public delegate IEnumerator ProcessFunc(ITMProgressSegment progSeg);
    public delegate void FinishFunc();

    public interface ITMProgressOperation : ITMProgress
    {
        ITMProgressSegment AddProgressSegment(float weight, ProcessFunc func);
        void AddFinishCallback(FinishFunc func);
        IEnumerator StepProgress();
    }

    public class Progress : ITMProgressOperation
    {
        public class ProgressSegment : ITMProgressSegment
        {
            private readonly Progress m_Parent;
            private readonly float m_Weight;
            private readonly ProcessFunc m_ProcessFunc;
            private IEnumerator m_Process;

            private float m_Progress;
            private uint m_WaterMark;

            public ProgressSegment(Progress parent,float weight, ProcessFunc func)
            {
                m_WaterMark = 0xdeadbeef;
                m_Parent = parent;
                m_Weight = weight;
                m_ProcessFunc = func;
                m_Process = null;

                m_Progress = 0;
            }

            public float Weight
            {
                get { return m_Weight; }
            }

            public void SetProgress(float progress,uint watermark)
            {
                m_WaterMark = watermark;
                m_Progress = Utility.Math.Clamp(progress, 0, 1);
                _UpdateProgressPercent();
            }

            public IEnumerator StepProgress()
            {
                if(null == m_Process)
                    m_Process = m_ProcessFunc(this);

                yield return m_Process;
            }

            private void _UpdateProgressPercent()
            {
                /// 内部机制上保证总体progress 在真正的协程迭代完毕后才能为100%所以这里最高99%，除非整个协程迭代完毕，为了防止代码中过早的设置为100%导致后面的协程被中断掉 没有执行完毕。
                m_Parent.m_Progress = Utility.Math.Clamp((m_Parent.m_FinishedWeight + m_Progress * m_Weight) / m_Parent.m_TotalWeight, 0, 0.99f);

                //Debugger.LogInfo("Progress:{0}%", (int)(m_Parent.m_Progress * 100));
                if (null != m_Parent.m_ProgressUpdateEventHandler)
                    m_Parent.m_ProgressUpdateEventHandler(this, new ProgressUpdateEventArgs(m_Parent.m_Progress));
            }
        }

        private event EventHandler<ProgressUpdateEventArgs> m_ProgressUpdateEventHandler = null;

        private readonly List<ProgressSegment> m_Segments;
        private readonly List<FinishFunc> m_OnFinish;
        private float m_TotalWeight;
        private float m_FinishedWeight;
        private float m_Progress;

        public Progress()
        {
            m_Segments = new List<ProgressSegment>();
            m_OnFinish = new List<FinishFunc>();
            m_TotalWeight = 0.0f;
            m_FinishedWeight = 0.0f;
            m_Progress = 0.0f;
        }


        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdateEventHandler
        {
            add { m_ProgressUpdateEventHandler += value; }
            remove { m_ProgressUpdateEventHandler -= value; }
        }

        public float CurrentProgress
        {
            get
            {
                return m_Progress;
            }
        }

        public ITMProgressSegment AddProgressSegment(float weight,ProcessFunc func)
        {
            if(weight <= 0)
            {
                Debugger.LogWarning("Progress segment weight must be a positive value!");
                weight = 1.0f;
            }

            ProgressSegment newSegment = new ProgressSegment(this,weight, func);
            m_TotalWeight += weight;
            m_Segments.Add(newSegment);
            return newSegment;
        }

        public void AddFinishCallback(FinishFunc func)
        {
            if(null != func)
                m_OnFinish.Add(func);
        }

        public IEnumerator StepProgress()
        {
            m_FinishedWeight = 0;
            for (int i = 0,icnt = m_Segments.Count;i<icnt;++i)
            {
                ProgressSegment curSegement = m_Segments[i];
                yield return curSegement.StepProgress();
                m_FinishedWeight += curSegement.Weight;
            }

            m_Progress = Utility.Math.Clamp(m_FinishedWeight / m_TotalWeight, 0, 1);
            if (null != m_ProgressUpdateEventHandler)
                m_ProgressUpdateEventHandler(this, new ProgressUpdateEventArgs(m_Progress));

            for (int i = 0,icnt = m_OnFinish.Count;i<icnt;++i)
                m_OnFinish[i]();

            m_ProgressUpdateEventHandler = null;
            ///Delegate[] delegates = m_ProgressUpdateEventHandler.GetInvocationList();
            ///for(int i =0,icnt = delegates.Length;i<icnt;++i)
            ///    m_ProgressUpdateEventHandler -= delegates[i] as EventHandler<ProgressUpdateEventArgs>;
        }
    }


}