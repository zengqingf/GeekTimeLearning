

using System.Collections.Generic;

namespace Tenmove.Runtime.EmbedUI
{
    public partial class MessageBox
    {
        public class Manager
        {
            private readonly ITMUIManager m_UIManager;
            private readonly LinkedList<MessageBoxInstance> m_MessageBoxList;
            private readonly Stack<MessageBoxInstance> m_MessageBoxPool;

            private int m_MessageBoxCountInPool;
            private uint m_MessageBoxAllocCount;

            private class MessageBoxInstance
            {
                private readonly Manager m_MessageBoxManager;
                private readonly UIFormMessageBox m_MessageBox;
                private uint m_MessageBoxID;

                public MessageBoxInstance(Manager msgBoxManager)
                {
                    Debugger.Assert(null != msgBoxManager, "Message box manager can not be null!");
                    m_MessageBoxManager = msgBoxManager;

                    m_MessageBox = m_MessageBoxManager.m_UIManager.CreateForm<UIFormMessageBox>("Base/UI/Prefabs/MessageBox/EmbedUIMessageBox.prefab", new UIFormParams() { UILayer = UIFormLayer.Top });
                    m_MessageBox.OnClickHandled += _OnMessageBoxHandled;
                    m_MessageBoxID = ~0u;
                }

                public UIFormMessageBox MessageBox
                {
                    get { return m_MessageBox; }
                }

                public uint ID
                {
                    get { return m_MessageBoxID; }
                }

                public void OnReuse(uint id)
                {
                    m_MessageBoxID = id;
                }

                public void OnRecycle()
                {

                }

                public void OnRelease()
                {
                    m_MessageBox.OnClickHandled -= _OnMessageBoxHandled;
                    m_MessageBoxManager.m_UIManager.DestroyForm(m_MessageBox);
                }

                private void _OnMessageBoxHandled()
                {
                    m_MessageBoxManager.DestoryMessageBox(ID);
                }
            }

            public Manager()
            {
                m_UIManager = ModuleManager.GetModule<ITMUIManager>();

                m_MessageBoxList = new LinkedList<MessageBoxInstance>();
                m_MessageBoxPool = new Stack<MessageBoxInstance>();
                m_MessageBoxAllocCount = 0;

                m_MessageBoxCountInPool = 5;
            }

            public int MessageBoxCountInPool
            {
                get { return m_MessageBoxCountInPool; }
            }

            public void SetMessageBoxCountInPool(int messageBoxCount)
            {
                if (messageBoxCount < 0)
                    messageBoxCount = 0;

                m_MessageBoxCountInPool = messageBoxCount;
            }

            public uint CreateMessageBox(Params msgBoxParams)
            {
                MessageBoxInstance inst = _AcquireMessageBox();

                inst.OnReuse(_AllocMessageBoxID());
                inst.MessageBox.Bind(msgBoxParams);
                inst.MessageBox.Show();

                m_MessageBoxList.AddLast(inst);

                return inst.ID;
            }

            public void DestoryMessageBox(uint msgBoxID)
            {
                LinkedListNode<MessageBoxInstance> cur = m_MessageBoxList.First;
                while(null != cur)
                {
                    if(cur.Value.ID == msgBoxID)
                    {
                        cur.Value.MessageBox.Unbind();
                        cur.Value.MessageBox.Hide();

                        _RecycleMessageBox(cur.Value);
                        m_MessageBoxList.Remove(cur);
                        return;
                    }
                }
            }

            private MessageBoxInstance _AcquireMessageBox()
            {
                if (m_MessageBoxPool.Count > 0)
                    return m_MessageBoxPool.Pop();

                return new MessageBoxInstance(this);
            }

            private void _RecycleMessageBox(MessageBoxInstance messageBox)
            {
                if(null != messageBox)
                {
                    if (m_MessageBoxPool.Count < m_MessageBoxAllocCount)
                        m_MessageBoxPool.Push(messageBox);
                    else
                        messageBox.OnRelease();
                }
            }

            private uint _AllocMessageBoxID()
            {
                return m_MessageBoxAllocCount++;
            }
        }       
    }
}